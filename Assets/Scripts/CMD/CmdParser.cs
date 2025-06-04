using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic; // Para List<>

namespace MugenForever.CMD
{
    public static class CmdParser
    {
        // Regex para [Command]
        private static Regex commandBlockRegex = new Regex(@"^\s*\[\s*Command\s*\]\s*$", RegexOptions.IgnoreCase);
        private static Regex nameRegex = new Regex(@"^\s*name\s*=\s*""(.+)""\s*$", RegexOptions.IgnoreCase);
        private static Regex commandSequenceRegex = new Regex(@"^\s*command\s*=\s*(.+)$", RegexOptions.IgnoreCase);
        private static Regex timeRegex = new Regex(@"^\s*time\s*=\s*(\d+)\s*$", RegexOptions.IgnoreCase);
        private static Regex timeNRex = new Regex(@"^\s*time\.(\d+)\s*=\s*(\d+)\s*$", RegexOptions.IgnoreCase); // Para time.N
        private static Regex bufferTimeRegex = new Regex(@"^\s*buffer\.time\s*=\s*(\d+)\s*$", RegexOptions.IgnoreCase);
        // Embora stateno seja tipicamente usado no CNS, alguns arquivos CMD podem ter essa informação para referência ou ferramentas.
        private static Regex statenoRegex = new Regex(@"^\s*stateno\s*=\s*(-?\d+)\s*$", RegexOptions.IgnoreCase);

        public static CmdData ParseCmdFile(string filePath)
        {
            CmdData cmdData = new CmdData();
            CommandDefinition currentCommand = null;
            bool inCommandBlockDeclaration = false; // Flag para indicar que estamos logo após um [Command]

            if (!File.Exists(filePath))
            {
                Debug.LogError($"[CmdParser.ParseCmdFile] Error: File not found at {filePath}");
                return cmdData; // Retorna dados vazios
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

                if (commandBlockRegex.IsMatch(processedLine))
                {
                    // Finaliza o comando anterior se estiver completo
                    if (currentCommand != null && !string.IsNullOrEmpty(currentCommand.Name) && currentCommand.Sequence.Count > 0)
                    {
                        cmdData.Commands.Add(currentCommand);
                    }
                    currentCommand = null;
                    inCommandBlockDeclaration = true; // Próxima linha esperada é 'name = "..."'
                    continue;
                }

                // Ignora linhas que não estão dentro de um bloco iniciado por [Command]
                // ou se a declaração do bloco não foi seguida por 'name = ...'
                if (!inCommandBlockDeclaration && currentCommand == null) continue;


                Match nameMatch = nameRegex.Match(processedLine);
                if (nameMatch.Success)
                {
                    if (inCommandBlockDeclaration || currentCommand != null) // Se já estamos em um bloco ou acabamos de declarar um
                    {
                        // Se currentCommand já existe e tem nome, adiciona-o antes de criar um novo.
                        // Isso trata casos onde [Command] pode não ser explicitamente declarado para cada 'name='.
                        if (currentCommand != null && !string.IsNullOrEmpty(currentCommand.Name) && currentCommand.Sequence.Count > 0)
                        {
                             cmdData.Commands.Add(currentCommand);
                        }
                        currentCommand = new CommandDefinition(nameMatch.Groups[1].Value.Trim());
                        inCommandBlockDeclaration = false; // 'name' foi encontrado, saímos do modo de declaração de bloco
                    }
                    else
                    {
                        // Linha 'name=' encontrada fora de um contexto [Command] esperado. Pode ser um erro de formatação no arquivo.
                        Debug.LogWarning($"[CmdParser.ParseCmdFile] 'name' parameter found outside a [Command] block or without a preceding [Command] block: {processedLine}");
                    }
                    continue;
                }

                // As linhas seguintes só são processadas se já tivermos um currentCommand (ou seja, 'name' já foi definido)
                if (currentCommand == null) continue;

                Match seqMatch = commandSequenceRegex.Match(processedLine);
                if (seqMatch.Success)
                {
                    string[] inputs = seqMatch.Groups[1].Value.Trim().Split(',');
                    foreach (string rawInput in inputs)
                    {
                        string inputKey = rawInput.Trim();
                        if (string.IsNullOrEmpty(inputKey)) continue;

                        bool isHold = false;
                        bool isRelease = false;
                        // int releaseTicks = 0; // Para ~<ticks>S

                        if (inputKey.StartsWith("/"))
                        {
                            isHold = true;
                            inputKey = inputKey.Substring(1);
                        }
                        else if (inputKey.StartsWith("~"))
                        {
                            isRelease = true;
                            inputKey = inputKey.Substring(1);
                            // Opcional: Parsear ticks para release, ex: "~30a" (soltar 'a' por 30 ticks)
                            // Match releaseTickMatch = Regex.Match(inputKey, @"^(\d+)(.+)$");
                            // if (releaseTickMatch.Success)
                            // {
                            //    releaseTicks = int.Parse(releaseTickMatch.Groups[1].Value);
                            //    inputKey = releaseTickMatch.Groups[2].Value.Trim();
                            // }
                        }
                        currentCommand.Sequence.Add(new CommandInput(inputKey, isHold, isRelease));
                    }
                    continue;
                }

                Match timeMatch = timeRegex.Match(processedLine);
                if (timeMatch.Success)
                {
                    currentCommand.Time = int.Parse(timeMatch.Groups[1].Value);
                    continue;
                }

                Match timeNMatch = timeNRex.Match(processedLine);
                if (timeNMatch.Success)
                {
                    int inputIndex = int.Parse(timeNMatch.Groups[1].Value);
                    int timeVal = int.Parse(timeNMatch.Groups[2].Value);
                    currentCommand.InputTimes[inputIndex] = timeVal; // Armazena com índice 1-based
                    continue;
                }

                Match bufferMatch = bufferTimeRegex.Match(processedLine);
                if (bufferMatch.Success)
                {
                    currentCommand.BufferTime = int.Parse(bufferMatch.Groups[1].Value);
                    continue;
                }

                Match statenoMatch = statenoRegex.Match(processedLine);
                if (statenoMatch.Success)
                {
                    currentCommand.StateNumber = int.Parse(statenoMatch.Groups[1].Value);
                    // A presença de 'stateno' não finaliza o comando aqui, pois os parâmetros podem vir em qualquer ordem.
                    continue;
                }
            }

            // Adiciona o último comando que estava sendo processado, se válido
            if (currentCommand != null && !string.IsNullOrEmpty(currentCommand.Name) && currentCommand.Sequence.Count > 0)
            {
                cmdData.Commands.Add(currentCommand);
            }

            // Debug.Log($"[CmdParser.ParseCmdFile] Finished parsing {filePath}. Found {cmdData.Commands.Count} commands.");
            return cmdData;
        }
    }
}
