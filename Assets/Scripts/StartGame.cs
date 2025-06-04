using UnityEngine;
using System.IO; // Para Path, File
using System.Linq; // Para .Find() na lista de sprites
using MugenForever; // Para DefChar
using MugenForever.Sff; // Para Sff, SffInfo, SffSprite
using MugenForever.AIR; // Para AirData, AirParser
using MugenForever.CMD; // Para CmdData, CmdParser
using MugenForever.Character; // Para CharacterAnimator, CommandDetector

namespace MugenForever
{
    public class StartGame : MonoBehaviour
    {
        void Start()
        {
            string charDefPath = Path.Combine(Application.dataPath, "..", "mugen_2010", "chars", "kfm", "kfm.def");
            charDefPath = Path.GetFullPath(charDefPath);

            Debug.Log($"Tentando carregar DEF de: {charDefPath}");

            if (!File.Exists(charDefPath))
            {
                Debug.LogError($"Arquivo DEF não encontrado em: {charDefPath}. Verifique o caminho. Application.dataPath é '{Application.dataPath}'.");
                string alternativePath = Path.Combine(Application.dataPath, "mugen_2010", "chars", "kfm", "kfm.def");
                alternativePath = Path.GetFullPath(alternativePath);
                if (File.Exists(alternativePath)) {
                    Debug.LogWarning($"Tentando caminho alternativo para DEF: {alternativePath}");
                    charDefPath = alternativePath;
                } else {
                    Debug.LogError($"Caminho alternativo {alternativePath} também não encontrado.");
                    return;
                }
            }

            DefChar defChar = new DefChar();
            bool defLoaded = defChar.Load(charDefPath);

            if (!defLoaded || string.IsNullOrEmpty(defChar.displayname))
            {
                Debug.LogError($"Falha ao carregar dados do arquivo DEF: {charDefPath}.");
                return;
            }

            Debug.Log($"Personagem: {defChar.displayname}, Autor: {defChar.author}, Versão MUGEN: {defChar.mugenversion}");
            string charFolder = Path.GetDirectoryName(charDefPath);

            // Carregar SFF
            if (string.IsNullOrEmpty(defChar.sprite))
            {
                Debug.LogError("Nome do arquivo SFF (sprite) não encontrado no .DEF.");
                return;
            }
            string sffRelativePath = defChar.sprite;
            string sffFilePath = Path.GetFullPath(Path.Combine(charFolder, sffRelativePath));
            Debug.Log($"Caminho do SFF obtido do DEF: '{sffRelativePath}'. Calculado: '{sffFilePath}'");
            if (!File.Exists(sffFilePath))
            {
                Debug.LogError($"Arquivo SFF não encontrado em: {sffFilePath}.");
                return;
            }
            SffInfo sffInfo = Sff.Read(sffFilePath);
            if (sffInfo == null || sffInfo.sprites == null || sffInfo.sprites.Count == 0)
            {
                Debug.LogError($"Falha ao ler SFF de '{sffFilePath}' ou SFF não contém sprites.");
                return;
            }
            string sffVersionString = $"{sffInfo.verHi}.{sffInfo.verLo1}.{sffInfo.verLo2}.{sffInfo.verLo3}";
            Debug.Log($"SFF '{sffFilePath}' carregado. Sprites: {sffInfo.sprites.Count}. Versão: {sffVersionString}");

            // Carregar AIR
            if (string.IsNullOrEmpty(defChar.anim))
            {
                Debug.LogError($"Nome do arquivo AIR (anim) não encontrado no .DEF para {defChar.displayname}.");
                return;
            }
            string airRelativePath = defChar.anim;
            string airFilePath = Path.GetFullPath(Path.Combine(charFolder, airRelativePath));
            Debug.Log($"Caminho do AIR obtido do DEF: '{airRelativePath}'. Calculado: '{airFilePath}'");
            if (!File.Exists(airFilePath))
            {
                Debug.LogError($"Arquivo AIR não encontrado em: {airFilePath}.");
                return;
            }
            AirData airData = AirParser.ParseAirFile(airFilePath);
            if (airData == null || airData.Animations.Count == 0)
            {
                Debug.LogError($"Falha ao parsear AIR de '{airFilePath}' ou AIR não contém animações.");
                return;
            }
            Debug.Log($"AIR '{airFilePath}' carregado. Actions: {airData.Animations.Count}");

            // Carregar CMD
            CmdData cmdData = null;
            if (string.IsNullOrEmpty(defChar.cmd))
            {
                Debug.LogWarning($"Nome do arquivo CMD não encontrado no .DEF para {defChar.displayname}.");
            }
            else
            {
                string cmdRelativePath = defChar.cmd;
                string cmdFilePath = Path.GetFullPath(Path.Combine(charFolder, cmdRelativePath));
                Debug.Log($"Caminho do CMD obtido do DEF: '{cmdRelativePath}'. Calculado: '{cmdFilePath}'");
                if (!File.Exists(cmdFilePath))
                {
                    Debug.LogError($"Arquivo CMD '{cmdFilePath}' não encontrado.");
                }
                else
                {
                    cmdData = CmdParser.ParseCmdFile(cmdFilePath);
                    if (cmdData == null || (cmdData.Commands != null && cmdData.Commands.Count == 0))
                    {
                        Debug.LogWarning($"Arquivo CMD '{cmdFilePath}' parseado, mas não contém comandos ou falhou no parse.");
                    }
                    else if (cmdData != null)
                    {
                        Debug.Log($"CMD '{cmdFilePath}' carregado. Comandos: {cmdData.Commands.Count}");
                    }
                }
            }

            // Preparar Fallback Sprite (antes de criar o GameObject principal)
            SffSprite fallbackSprite = sffInfo.sprites.Find(s => s.groupNumber == 0 && s.imageNumber == 0);
            if (fallbackSprite == null && sffInfo.sprites.Count > 0)
            {
                fallbackSprite = sffInfo.sprites[0];
            }
            // Garante que o fallbackSprite tenha seu UnityEngine.Sprite gerado se ele for ser usado.
            if (fallbackSprite != null && fallbackSprite.sprite == null && fallbackSprite.pcx != null && fallbackSprite.pcx.DecodedPixels != null)
            {
                fallbackSprite.GenerateUnitySprite();
            }

            // Criar GameObject e adicionar componentes
            GameObject charDisplayObject = new GameObject($"CharDisplay_{Path.GetFileNameWithoutExtension(charDefPath)}");
            SpriteRenderer renderer = charDisplayObject.AddComponent<SpriteRenderer>();

            CharacterAnimator animator = charDisplayObject.AddComponent<CharacterAnimator>();
            animator.Initialize(sffInfo, airData);

            CommandDetector commandDetector = charDisplayObject.AddComponent<CommandDetector>();
            commandDetector.Initialize(cmdData);

            // Tentar tocar a animação de "parado" (Action 0)
            if (!animator.PlayAnimation(0))
            {
                Debug.LogError($"Não foi possível iniciar a animação padrão (Action 0) para {defChar.displayname}.");
                if (fallbackSprite != null && fallbackSprite.sprite != null) {
                     renderer.sprite = fallbackSprite.sprite;
                     if(renderer.sprite.texture != null) renderer.sprite.texture.filterMode = FilterMode.Point;
                     Debug.LogWarning("Fallback para sprite estático porque a Action 0 não pôde ser reproduzida.");
                } else {
                     Debug.LogError("Falha ao reproduzir Action 0 e nenhum sprite de fallback utilizável disponível.");
                }
            } else {
                Debug.Log($"Animação Action 0 iniciada para {defChar.displayname}.");
            }

            // Configurar câmera
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                charDisplayObject.transform.position = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10.0f));
                Vector3 currentPos = charDisplayObject.transform.position;
                currentPos.z = 0;
                charDisplayObject.transform.position = currentPos;
            }
            else
            {
                charDisplayObject.transform.position = new Vector3(0, 0, 0);
                Debug.LogWarning("Câmera principal (Camera.main) não encontrada. Posicionando o personagem em (0,0,0).");
            }

            // Log final corrigido
            Debug.Log($"Personagem '{defChar.displayname}' ('{Path.GetFileName(charDefPath)}') carregado e GameObject '{charDisplayObject.name}' configurado com Animator e Detector de Comandos.");
        }
    }
}
