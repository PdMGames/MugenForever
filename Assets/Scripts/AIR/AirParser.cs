using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions; // Para parsing de linhas

namespace MugenForever.AIR
{
    public static class AirParser
    {
        public static AirData ParseAirFile(string filePath)
        {
            AirData airData = new AirData();
            AirAnimation currentAnimation = null;

            if (!File.Exists(filePath))
            {
                Debug.LogError($"AIR_PARSE: File not found at {filePath}");
                return airData; // Retorna dados vazios
            }

            string[] lines = File.ReadAllLines(filePath);

            // Regex para identificar o início de uma Action. Ex: "[Begin Action 123]" (ignora case)
            // Também captura se há espaços antes/depois dos colchetes e dentro deles.
            Regex actionBeginRegex = new Regex(@"^\s*\[\s*Begin\s+Action\s+(\d+)\s*\]\s*$", RegexOptions.IgnoreCase);

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Ignora linhas vazias ou comentários (MUGEN usa ';', mas '//' é comum em outros contextos)
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("//"))
                {
                    continue;
                }

                Match actionMatch = actionBeginRegex.Match(trimmedLine);
                if (actionMatch.Success)
                {
                    int actionNumber = int.Parse(actionMatch.Groups[1].Value);
                    // Se já existe uma animação com este número, o comportamento MUGEN é usar a primeira.
                    // No entanto, para fins de log, podemos avisar sobre duplicatas.
                    if (airData.Animations.ContainsKey(actionNumber))
                    {
                        Debug.LogWarning($"AIR_PARSE: Duplicate Action number {actionNumber} found in file {filePath}. Using the first instance that was defined.");
                        currentAnimation = null; // Impede que frames sejam adicionados à duplicata se ela for ignorada.
                                                 // Ou, se quisermos usar a última, poderíamos fazer:
                                                 // currentAnimation = new AirAnimation(actionNumber);
                                                 // airData.Animations[actionNumber] = currentAnimation;
                                                 // Mas vamos seguir a lógica de "usar a primeira" por enquanto, então não substituímos.
                                                 // Para garantir que não adicionemos frames à action errada se currentAnimation já for essa duplicata:
                        if(airData.Animations.TryGetValue(actionNumber, out AirAnimation existingAnimation)) {
                            currentAnimation = existingAnimation; // Aponta para a original
                        } else {
                             // Isso não deveria acontecer se ContainsKey é true, mas por segurança:
                            currentAnimation = new AirAnimation(actionNumber);
                            airData.Animations.Add(actionNumber, currentAnimation);
                        }

                    }
                    else
                    {
                        currentAnimation = new AirAnimation(actionNumber);
                        airData.Animations.Add(actionNumber, currentAnimation);
                    }
                    continue; // Passa para a próxima linha após processar a definição da Action
                }

                // Se não for uma definição de Action, e já estivermos dentro de uma (currentAnimation != null),
                // então a linha pode ser um frame, Loopstart, ou definição de Clsn (que ignoraremos por agora).
                if (currentAnimation != null)
                {
                    // Ignorar definições de Clsn por enquanto
                    if (trimmedLine.ToLowerInvariant().StartsWith("clsn"))
                    {
                        continue;
                    }
                    // Ignorar Loopstart por enquanto (afetaria a lógica de animação, não o parsing de frames individuais)
                    if (trimmedLine.ToLowerInvariant() == "loopstart")
                    {
                        continue;
                    }

                    // Tenta parsear a linha como um quadro de animação.
                    // Formato: Group,Image, X,Y, Duration [,FlipFlags] [,BlendingFlags]
                    string[] parts = trimmedLine.Split(',');
                    if (parts.Length >= 5) // Precisa de pelo menos Grupo, Imagem, X, Y, Duração
                    {
                        try
                        {
                            AirFrame frame = new AirFrame();
                            frame.GroupNumber = int.Parse(parts[0].Trim());
                            frame.ImageNumber = int.Parse(parts[1].Trim());
                            // parts[2] é X offset, parts[3] é Y offset - ignoramos por enquanto
                            frame.Duration = int.Parse(parts[4].Trim());

                            // Checa por flags de flip (opcional, pode ser o sexto elemento)
                            if (parts.Length > 5 && !string.IsNullOrEmpty(parts[5].Trim()))
                            {
                                string flipFlags = parts[5].Trim().ToUpperInvariant();
                                if (flipFlags.Contains("H"))
                                {
                                    frame.HorizontalFlip = true;
                                }
                                if (flipFlags.Contains("V"))
                                {
                                    frame.VerticalFlip = true;
                                }
                                // Ignorar outros flags como 'A', 'S', 'ASxxxDxxx' por enquanto
                            }
                            currentAnimation.Frames.Add(frame);
                        }
                        catch (System.FormatException fe)
                        {
                            // Não logar erro para cada linha que não é um frame, pois podem ser Clsn, etc.
                            // Debug.LogWarning($"AIR_PARSE: Could not parse as frame line: '{trimmedLine}' in Action {currentAnimation.ActionNumber}. Error: {fe.Message}");
                        }
                        catch (System.Exception ex)
                        {
                             Debug.LogError($"AIR_PARSE: Generic error parsing frame line: '{trimmedLine}' in Action {currentAnimation.ActionNumber}. Error: {ex.Message}");
                        }
                    }
                    // else: Linha não tem partes suficientes para ser um frame, pode ser um comentário mal formatado ou outra coisa.
                }
            }
            // Debug.Log($"AIR_PARSE: Finished parsing {filePath}. Found {airData.Animations.Count} actions.");
            return airData;
        }
    }
}
