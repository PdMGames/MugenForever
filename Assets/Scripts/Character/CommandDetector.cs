using UnityEngine;
using MugenForever.CMD;
using MugenForever.Core; // Para InputManager e MugenInputKey
using System.Collections.Generic; // Para List e Dictionary
using System.Linq; // Para OrderByDescending

namespace MugenForever.Character
{
    public class CommandDetector : MonoBehaviour
    {
        private CmdData characterCmdData;

        private struct InputEvent
        {
            public MugenInputKey Key;
            public float TimePressed;

            public InputEvent(MugenInputKey key, float timePressed)
            {
                Key = key;
                TimePressed = timePressed;
            }
            public override string ToString() => $"{Key} at {TimePressed:F2}";
        }
        private List<InputEvent> inputBuffer = new List<InputEvent>();
        private float inputBufferDuration = 0.75f; // Em segundos (tempo que um input permanece no buffer)

        private Dictionary<string, float> commandCooldowns = new Dictionary<string, float>();
        private float globalCommandCooldownTime = 0.2f; // Cooldown em segundos após um comando ser detectado

        public void Initialize(CmdData cmdData)
        {
            characterCmdData = cmdData;
            if (characterCmdData != null && characterCmdData.Commands != null && characterCmdData.Commands.Count > 0)
            {
                // Ordena os comandos por tamanho da sequência, decrescente, para priorizar os mais longos/complexos
                characterCmdData.Commands = characterCmdData.Commands.OrderByDescending(c => c.Sequence.Count).ToList();
                Debug.Log($"[CommandDetector] Initialized for {gameObject.name}. {characterCmdData.Commands.Count} commands loaded and sorted.");
            }
            else
            {
                Debug.LogWarning($"[CommandDetector] Initialized for {gameObject.name} with no commands (CmdData is null or has no commands).");
            }
        }

        void Update()
        {
            if (characterCmdData == null || characterCmdData.Commands == null || characterCmdData.Commands.Count == 0)
            {
                return; // Nenhum dado de comando para processar
            }

            UpdateInputBuffer();
            DetectCommands();
        }

        private void UpdateInputBuffer()
        {
            // Remover inputs antigos
            inputBuffer.RemoveAll(evt => Time.time - evt.TimePressed > inputBufferDuration);

            // Adicionar novos inputs pressionados
            foreach (MugenInputKey key in System.Enum.GetValues(typeof(MugenInputKey)))
            {
                if (InputManager.IsPressed(key))
                {
                    inputBuffer.Add(new InputEvent(key, Time.time));
                    // Debug.Log($"[InputBuffer] Added: {key} at {Time.time}");
                }
            }
        }

        private void DetectCommands()
        {
            // Atualizar cooldowns
            List<string> cooledDownCommands = new List<string>();
            foreach (var entryKey in commandCooldowns.Keys.ToList()) // Usar ToList() para poder modificar o dicionário
            {
                commandCooldowns[entryKey] -= Time.deltaTime;
                if (commandCooldowns[entryKey] <= 0)
                {
                    cooledDownCommands.Add(entryKey);
                }
            }
            foreach (string cmdName in cooledDownCommands)
            {
                commandCooldowns.Remove(cmdName);
            }

            if (inputBuffer.Count == 0) return;

            foreach (CommandDefinition commandDef in characterCmdData.Commands)
            {
                if (commandCooldowns.ContainsKey(commandDef.Name))
                {
                    continue; // Comando em cooldown
                }

                if (commandDef.Sequence.Count == 0 || commandDef.Sequence.Count > inputBuffer.Count)
                {
                    continue; // Não há inputs suficientes no buffer para este comando
                }

                int bufferIndex = inputBuffer.Count - 1;
                int commandSeqIndex = commandDef.Sequence.Count - 1;
                int matchedInputs = 0;
                float firstMatchTimeInSequence = 0f; // Tempo do primeiro input da sequência de comando no buffer
                float lastMatchTimeInSequence = inputBuffer[bufferIndex].TimePressed; // Tempo do último input da potencial correspondência

                // Lista para armazenar os InputEvents do buffer que correspondem à sequência do comando atual
                // List<InputEvent> potentialMatchEvents = new List<InputEvent>(); // Não usado ativamente na lógica atual, mas poderia ser para debug ou time.N

                while (bufferIndex >= 0 && commandSeqIndex >= 0)
                {
                    CommandInput cmdInput = commandDef.Sequence[commandSeqIndex];
                    InputEvent bufferEvent = inputBuffer[bufferIndex];
                    // potentialMatchEvents.Add(bufferEvent); // Se fosse usado

                    if (TryParseMugenInputKey(cmdInput.ButtonName, out List<MugenInputKey> expectedKeys, out bool isCmdInputSimultaneous))
                    {
                        bool currentInputStepMatch = false;
                        // Simplificação: "DF" = D ou F. "a+b" = a ou b. isCmdInputSimultaneous é ignorado por enquanto na lógica de match.
                        if (expectedKeys.Contains(bufferEvent.Key))
                        {
                            if (cmdInput.IsHold)
                            {
                                // Para um input de comando como "/D" ou "/DF" (onde D foi o que deu match no buffer),
                                // verifica se a tecla específica que deu match (bufferEvent.Key) está sendo mantida.
                                if (InputManager.IsHeld(bufferEvent.Key))
                                {
                                    currentInputStepMatch = true;
                                }
                            }
                            else if (cmdInput.IsRelease)
                            {
                                // Ignorado por enquanto, conforme instrução.
                                currentInputStepMatch = true; // Permitindo match temporariamente
                            }
                            else // Input normal (pressionado)
                            {
                                currentInputStepMatch = true;
                            }
                        }

                        if (currentInputStepMatch)
                        {
                            matchedInputs++;
                            if (commandSeqIndex == 0)
                            {
                                firstMatchTimeInSequence = bufferEvent.TimePressed;
                            }
                            commandSeqIndex--;
                        }
                        // Se não houve match para este passo do comando, e a sequência do comando não permite "pular" (ex: time.N muito grande),
                        // então este comando não pode ser formado com este final de buffer.
                        // A lógica atual continua tentando com bufferIndex--.
                        // Para uma detecção mais estrita, se currentInputStepMatch for false, poderíamos quebrar o loop interno
                        // dependendo da lógica de 'time.N' (não implementado aqui).
                    }
                    else
                    {
                        matchedInputs = 0;
                        break;
                    }
                    bufferIndex--;
                }
                // if(potentialMatchEvents.Count > 0) potentialMatchEvents.Reverse();

                if (matchedInputs == commandDef.Sequence.Count)
                {
                    float commandExecutionTimeInBuffer = lastMatchTimeInSequence - firstMatchTimeInSequence;
                    float commandDefMaxTimeSec = (commandDef.Time > 0) ? (commandDef.Time / 60.0f) : inputBufferDuration;

                    // TODO: Implementar verificação de time.N aqui, usando `commandDef.InputTimes` e os tempos em `potentialMatchEvents`

                    if (commandExecutionTimeInBuffer <= commandDefMaxTimeSec)
                    {
                        Debug.Log($"COMMAND DETECTED: '{commandDef.Name}'! (Would trigger State: {commandDef.StateNumber}) Executed in {commandExecutionTimeInBuffer:F2}s (max: {commandDefMaxTimeSec:F2}s)");
                        commandCooldowns[commandDef.Name] = globalCommandCooldownTime;
                        inputBuffer.Clear();
                        // TODO: Disparar um evento ou chamar um método na StateMachine para processar commandDef.StateNumber
                        break;
                    }
                }
            }
        }

        private bool TryParseMugenInputKey(string buttonName, out List<MugenInputKey> keys, out bool isSimultaneous)
        {
            keys = new List<MugenInputKey>();
            isSimultaneous = false;
            buttonName = buttonName.Trim().ToUpper();

            if (buttonName.Contains("+"))
            {
                isSimultaneous = true;
                string[] parts = buttonName.Split('+');
                bool allPartsValid = true;
                foreach (string part in parts)
                {
                    string trimmedPart = part.Trim();
                    if (TryParseSingleInput(trimmedPart, out MugenInputKey keyPart))
                    {
                        keys.Add(keyPart);
                    }
                    else { allPartsValid = false; break; }
                }
                return allPartsValid && keys.Count > 0;
            }
            else
            {
                if (buttonName == "DB") { keys.Add(MugenInputKey.Down); keys.Add(MugenInputKey.Left); return true; }
                if (buttonName == "DF") { keys.Add(MugenInputKey.Down); keys.Add(MugenInputKey.Right); return true; }
                if (buttonName == "UB") { keys.Add(MugenInputKey.Up); keys.Add(MugenInputKey.Left); return true; }
                if (buttonName == "UF") { keys.Add(MugenInputKey.Up); keys.Add(MugenInputKey.Right); return true; }

                if (TryParseSingleInput(buttonName, out MugenInputKey key))
                {
                    keys.Add(key);
                    return true;
                }
            }

            // Log apenas se não for um direcional composto já tratado, para evitar spam.
            // Os direcionais compostos são tratados acima e retornam true se forem um deles.
            // Se chegar aqui e não for um single input válido, então é um erro real.
            if(keys.Count == 0) // Evita logar para "DF" se ele falhar no TryParseSingleInput mas for tratado como composto.
                 Debug.LogWarning($"[TryParseMugenInputKey] Failed to parse button name: {buttonName}");
            return false;
        }

        // Helper para parsear um único input (não combinado por '+')
        private bool TryParseSingleInput(string singleInputName, out MugenInputKey key)
        {
            singleInputName = singleInputName.Trim().ToUpper();
            switch (singleInputName)
            {
                case "F": key = MugenInputKey.Right; return true;
                case "B": key = MugenInputKey.Left; return true;
                case "U": key = MugenInputKey.Up; return true;
                case "D": key = MugenInputKey.Down; return true;
            }
            return System.Enum.TryParse<MugenInputKey>(singleInputName, true, out key);
        }
    }
}
