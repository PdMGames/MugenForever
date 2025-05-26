using UnityEngine;
using MugenForever.Sff; // Contém Sff, SffInfo, SffSprite
using MugenForever.Def; // Contém DefChar
using System.IO;        // Para Path.Combine, Path.GetDirectoryName, Path.GetFileNameWithoutExtension, File.Exists
using System.Linq;      // Para sffInfo.sprites.Find

namespace MugenForever
{
    public class StartGame : MonoBehaviour
    {
        void Start()
        {
            // Caminho para o arquivo .def do personagem.
            // Este caminho assume que a pasta "mugen_2010" está na raiz do projeto Unity,
            // ao lado da pasta "Assets".
            string charDefPath = Path.Combine(Application.dataPath, "..", "mugen_2010", "chars", "kfm", "kfm.def");
            // Normalizar o caminho para remover ".." e garantir que seja absoluto e correto para o SO
            charDefPath = Path.GetFullPath(charDefPath);

            Debug.Log($"Tentando carregar DEF de: {charDefPath}");

            if (!File.Exists(charDefPath))
            {
                Debug.LogError($"Arquivo DEF não encontrado em: {charDefPath}. Verifique o caminho. Application.dataPath é '{Application.dataPath}'.");
                // Tentar um caminho alternativo, se 'mugen_2010' estiver dentro de Assets (menos provável pela estrutura do repo)
                string alternativePath = Path.Combine(Application.dataPath, "mugen_2010", "chars", "kfm", "kfm.def");
                alternativePath = Path.GetFullPath(alternativePath); // Normalizar também
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
                Debug.LogError($"Falha ao carregar dados do arquivo DEF: {charDefPath}. 'defChar.Load' retornou falso ou não populou 'displayname'.");
                return;
            }

            Debug.Log($"Personagem: {defChar.displayname}, Autor: {defChar.author}, Versão MUGEN: {defChar.mugenversion}");

            if (string.IsNullOrEmpty(defChar.sprite))
            {
                Debug.LogError("Nome do arquivo SFF (sprite) não encontrado ou vazio no arquivo .DEF.");
                return;
            }

            string sffRelativePath = defChar.sprite;
            string charFolder = Path.GetDirectoryName(charDefPath); 
            string sffFilePath = Path.GetFullPath(Path.Combine(charFolder, sffRelativePath)); 
            
            Debug.Log($"Caminho do SFF obtido do DEF: '{sffRelativePath}'. Caminho completo calculado: '{sffFilePath}'");

            if (!File.Exists(sffFilePath))
            {
                Debug.LogError($"Arquivo SFF não encontrado em: {sffFilePath}. Verifique se o caminho está correto e o arquivo existe. Pasta do personagem: '{charFolder}'");
                return;
            }

            SffInfo sffInfo = Sff.Read(sffFilePath);

            if (sffInfo == null)
            {
                Debug.LogError($"Falha ao ler SFF de '{sffFilePath}'. Objeto sffInfo é nulo.");
                return;
            }
            if (sffInfo.sprites == null || sffInfo.sprites.Count == 0)
            {
                Debug.LogError($"SFF de '{sffFilePath}' não contém sprites ou a lista de sprites é nula.");
                return;
            }
            // Construir a string de versão a partir dos componentes em sffInfo
            string sffVersionString = $"{sffInfo.verHi}.{sffInfo.verLo1}.{sffInfo.verLo2}.{sffInfo.verLo3}";
            Debug.Log($"SFF '{sffFilePath}' carregado. Total de sprites: {sffInfo.sprites.Count}. Versão SFF: {sffVersionString}");

        // Carregar Arquivo AIR
        if (string.IsNullOrEmpty(defChar.anim))
        {
            Debug.LogError($"Nome do arquivo AIR (anim) não encontrado ou vazio no arquivo .DEF para {defChar.displayname}.");
            return;
        }

        string airRelativePath = defChar.anim;
        // charFolder já foi definido como Path.GetDirectoryName(charDefPath)
        string airFilePath = Path.GetFullPath(Path.Combine(charFolder, airRelativePath)); 

        Debug.Log($"Caminho do AIR obtido do DEF: '{airRelativePath}'. Caminho completo calculado: '{airFilePath}'");

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
        Debug.Log($"AIR '{airFilePath}' carregado. Total de Actions: {airData.Animations.Count}");

        // Carregar Arquivo CMD
        CmdData cmdData = null; 
        if (string.IsNullOrEmpty(defChar.cmd))
        {
            Debug.LogWarning($"Nome do arquivo CMD não encontrado ou vazio no arquivo .DEF para {defChar.displayname}. O personagem pode não ter comandos especiais.");
        }
        else
        {
            string cmdRelativePath = defChar.cmd;
            string cmdFilePath = Path.GetFullPath(Path.Combine(charFolder, cmdRelativePath)); 

            Debug.Log($"Caminho do CMD obtido do DEF: '{cmdRelativePath}'. Caminho completo calculado: '{cmdFilePath}'");

            if (!File.Exists(cmdFilePath))
            {
                Debug.LogError($"Arquivo CMD '{cmdFilePath}' não encontrado.");
            }
            else
            {
                cmdData = CmdParser.ParseCmdFile(cmdFilePath);
                if (cmdData == null || (cmdData.Commands != null && cmdData.Commands.Count == 0))
                {
                    Debug.LogWarning($"Arquivo CMD '{cmdFilePath}' parseado, mas não contém comandos ou falhou no parse (verificar logs do CmdParser). Personagem pode não ter comandos definidos.");
                }
                else if (cmdData != null) 
                {
                    Debug.Log($"CMD '{cmdFilePath}' carregado. Total de Comandos: {cmdData.Commands.Count}");
                }
            }
        }

            // Lógica para encontrar um sprite de fallback se a animação falhar
            SffSprite fallbackSprite = sffInfo.sprites.Find(s => s.groupNumber == 0 && s.imageNumber == 0);
            if (fallbackSprite == null && sffInfo.sprites.Count > 0)
            {
                fallbackSprite = sffInfo.sprites[0]; 
            }
            if (fallbackSprite != null && fallbackSprite.sprite == null && fallbackSprite.pcx != null && fallbackSprite.pcx.DecodedPixels != null)
            {
                 // Tenta gerar o sprite para o fallback se ainda não foi gerado
                fallbackSprite.GenerateUnitySprite();
            }


            GameObject charDisplayObject = new GameObject($"CharDisplay_{Path.GetFileNameWithoutExtension(charDefPath)}");
            SpriteRenderer renderer = charDisplayObject.AddComponent<SpriteRenderer>();

            CharacterAnimator animator = charDisplayObject.AddComponent<CharacterAnimator>();
            animator.Initialize(sffInfo, airData);

            CommandDetector commandDetector = charDisplayObject.AddComponent<CommandDetector>();
            commandDetector.Initialize(cmdData); // Passa cmdData, que pode ser null

            // Tenta tocar a animação de "parado" (Action 0)
            if (!animator.PlayAnimation(0)) // Action 0 é geralmente a animação de "stand"
            {
                Debug.LogError($"Não foi possível iniciar a animação padrão (Action 0) para {defChar.displayname}.");
                // Se PlayAnimation(0) falhar, usar o fallbackSprite
                if (fallbackSprite != null && fallbackSprite.sprite != null) {
                     renderer.sprite = fallbackSprite.sprite;
                     if(renderer.sprite.texture != null) renderer.sprite.texture.filterMode = FilterMode.Point; // Aplicar filterMode ao fallback
                     Debug.LogWarning("Fallback para sprite estático porque a Action 0 não pôde ser reproduzida.");
                } else {
                     Debug.LogError("Falha ao reproduzir Action 0 e nenhum sprite de fallback utilizável disponível.");
                }
            } else {
                Debug.Log($"Animação Action 0 iniciada para {defChar.displayname}.");
            }
            
            // O CharacterAnimator agora define o filterMode em UpdateSpriteForCurrentFrame.
            // if (renderer.sprite.texture != null) 
            // {
            //     renderer.sprite.texture.filterMode = FilterMode.Point;
            // }

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

            Debug.Log($"Sprite '{spriteToDisplay.groupNumber}-{spriteToDisplay.imageNumber}' do personagem '{defChar.displayname}' ('{Path.GetFileName(charDefPath)}') carregado e GameObject '{charDisplayObject.name}' criado com sucesso!");
        }
    }
}
