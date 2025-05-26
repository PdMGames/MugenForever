using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic; // Para List<>

namespace MugenForever.CNS
{
    public static class CnsParser
    {
        // Regex para Statedef: ex "[Statedef 123]" ou "[Statedef -1]"
        private static Regex statedefRegex = new Regex(@"^\s*\[\s*Statedef\s+(-?\d+)\s*\]\s*$", RegexOptions.IgnoreCase);
        // Regex para SCTRL: ex "type = ChangeAnim"
        private static Regex sctrlTypeRegex = new Regex(@"^\s*type\s*=\s*(\S+)\s*$", RegexOptions.IgnoreCase);
        // Regex para Triggers: ex "trigger1 = AnimTime = 0" ou "triggerall = time > 0"
        private static Regex triggerRegex = new Regex(@"^\s*trigger(all|\d*)\s*=\s*(.+)$", RegexOptions.IgnoreCase);
        // Regex para parâmetros gerais de SCTRL ou Statedef: ex "value = 200" ou "ctrl = 1"
        private static Regex paramRegex = new Regex(@"^\s*([a-zA-Z0-9_.]+)\s*=\s*(.+)$", RegexOptions.IgnoreCase);

        public static CnsData ParseCnsFile(string filePath)
        {
            CnsData cnsData = new CnsData();
            StateDefinition currentStateDef = null;
            StateController currentSctrl = null;
            // int sctrlCounter = 0; // Não usado na lógica atual, mas poderia ser para IDs de SCTRL anônimos

            if (!File.Exists(filePath))
            {
                Debug.LogError($"[CnsParser.ParseCnsFile] Error: File not found at {filePath}");
                return cnsData; // Retorna dados vazios
            }

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string processedLine = line;
                int commentStartIndex = processedLine.IndexOf(';');
                if (commentStartIndex != -1)
                {
                    processedLine = processedLine.Substring(0, commentStartIndex);
                }
                processedLine = processedLine.Trim();

                if (string.IsNullOrEmpty(processedLine))
                {
                    continue;
                }

                Match statedefMatch = statedefRegex.Match(processedLine);
                if (statedefMatch.Success)
                {
                    int statedefId = int.Parse(statedefMatch.Groups[1].Value);
                    currentStateDef = new StateDefinition(statedefId);
                    if (!cnsData.StateDefinitions.ContainsKey(statedefId))
                    {
                        cnsData.StateDefinitions.Add(statedefId, currentStateDef);
                    }
                    else
                    {
                        // MUGEN geralmente usa a última definição encontrada se houver duplicatas.
                        cnsData.StateDefinitions[statedefId] = currentStateDef;
                        Debug.LogWarning($"[CnsParser.ParseCnsFile] Warning: Duplicate Statedef ID {statedefId} in {filePath}. Overwriting with the later definition.");
                    }
                    currentSctrl = null; // Reseta o SCTRL atual ao encontrar um novo Statedef
                    // sctrlCounter = 0;
                    continue; // Passa para a próxima linha
                }

                // Se não estivermos dentro de um Statedef, ignoramos a linha (ex: [Data], [Info])
                if (currentStateDef == null)
                {
                    // Poderíamos adicionar lógica aqui para parsear [Data] ou [Constants] no futuro.
                    // Debug.LogWarning($"[CnsParser.ParseCnsFile] Line outside Statedef ignored: {processedLine}");
                    continue;
                }

                // Dentro de um Statedef
                Match typeMatch = sctrlTypeRegex.Match(processedLine);
                if (typeMatch.Success)
                {
                    // Esta linha define um novo StateController
                    currentSctrl = new StateController();
                    currentSctrl.Type = typeMatch.Groups[1].Value.Trim();
                    currentStateDef.Controllers.Add(currentSctrl);
                    // sctrlCounter++;
                    continue; // Passa para a próxima linha
                }

                // Se já temos um SCTRL definido e ativo para o Statedef atual
                if (currentSctrl != null)
                {
                    Match trigMatch = triggerRegex.Match(processedLine);
                    if (trigMatch.Success)
                    {
                        // Adiciona a expressão completa do trigger (a parte após o '=')
                        currentSctrl.Triggers.Add(trigMatch.Groups[2].Value.Trim());
                        continue; // Passa para a próxima linha
                    }

                    Match parMatchSctrl = paramRegex.Match(processedLine);
                    if (parMatchSctrl.Success)
                    {
                        // É um parâmetro para o SCTRL atual
                        string paramName = parMatchSctrl.Groups[1].Value.Trim();
                        string paramValue = parMatchSctrl.Groups[2].Value.Trim();
                        currentSctrl.Parameters.Add(new StateControllerParameter(paramName, paramValue));
                        continue; // Passa para a próxima linha
                    }
                    // Se não for trigger nem parâmetro reconhecido de SCTRL, pode ser uma linha mal formatada ou não suportada.
                    // Debug.LogWarning($"[CnsParser.ParseCnsFile] Unrecognized line within SCTRL context: '{processedLine}' in Statedef {currentStateDef.Id}");
                }
                else // Se currentSctrl é null, a linha deve ser um parâmetro do Statedef
                {
                    Match paramMatchStatedef = paramRegex.Match(processedLine);
                    if (paramMatchStatedef.Success)
                    {
                        string key = paramMatchStatedef.Groups[1].Value.Trim();
                        string value = paramMatchStatedef.Groups[2].Value.Trim();
                        currentStateDef.Parameters[key] = value; // Adiciona/atualiza o parâmetro no dicionário do Statedef

                        // Atualiza os campos específicos para conveniência
                        if (key.Equals("type", System.StringComparison.OrdinalIgnoreCase))
                            currentStateDef.Type = value;
                        else if (key.Equals("movetype", System.StringComparison.OrdinalIgnoreCase))
                            currentStateDef.MoveType = value;
                        else if (key.Equals("physics", System.StringComparison.OrdinalIgnoreCase))
                            currentStateDef.Physics = value;
                        // Outros parâmetros comuns do Statedef (anim, ctrl, velset, etc.)
                        // são apenas armazenados no dicionário Parameters por enquanto.
                        continue; // Passa para a próxima linha
                    }
                    // Se não for um parâmetro reconhecido do Statedef, pode ser uma linha mal formatada ou não suportada.
                    // Debug.LogWarning($"[CnsParser.ParseCnsFile] Unrecognized line within Statedef (but not SCTRL) context: '{processedLine}' in Statedef {currentStateDef.Id}");
                }
            }

            // Debug.Log($"[CnsParser.ParseCnsFile] Finished parsing {filePath}. Found {cnsData.StateDefinitions.Count} StateDefinitions.");
            return cnsData;
        }
    }
}
