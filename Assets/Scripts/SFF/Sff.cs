using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine; // Para Debug.Log e UnityException
// using UnityEditor; // Não deve ser usado em runtime
using System.Collections.Generic; // Para List<>

namespace MugenForever.Sff
{
    // Sff agora é uma classe utilitária estática, não um MonoBehaviour
    public static class Sff 
    {
        // fileName não é mais um campo de instância, será passado como parâmetro
        // os campos de dados do SFF (sprites, version, etc.) estarão em SffInfo

        public static SffInfo Read(string pathFile)
        {
            if (string.IsNullOrEmpty(pathFile) || !File.Exists(pathFile))
            {
                Debug.LogError($"[Sff.Read] Error: File not found or path is null/empty: {pathFile}");
                return null;
            }

            SffInfo sffInfo = new SffInfo();

            try
            {
                using (FileStream fsSource = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
                {
                    if (fsSource.Length < 16) // Tamanho mínimo para ler a versão
                    {
                        Debug.LogError($"[Sff.Read] Error: File is too short to be a valid SFF file: {pathFile}");
                        return null;
                    }

                    // Lê os primeiros 16 bytes para determinar a versão
                    byte[] versionHeader = new byte[16];
                    fsSource.Read(versionHeader, 0, 16);
                    // A versão está no 16º byte (índice 15), que é o verhi para SFFv2
                    // e parte da string de assinatura para SFFv1, mas SFFv1 verhi está no byte 15 do cabeçalho de 512 bytes.
                    // Precisamos de uma maneira mais robusta de verificar a versão.
                    // SFFv1: "ElecbyteSpr\0" + verlo3, verlo2, verlo1, verhi (0,0,0,1)
                    // SFFv2: "ElecbyteSpr\0" + verlo3, verlo2, verlo1, verhi (0,0,0,2)
                    // O byte no índice 15 (fsSource.Read(versionBytes, 15, 1)) é 'verhi'
                    // Vamos ler a assinatura para confirmar.
                    
                    fsSource.Seek(0, SeekOrigin.Begin); // Voltar ao início para os leitores V1/V2
                    char[] signatureChars = new char[12];
                    using(BinaryReader sigReader = new BinaryReader(fsSource, System.Text.Encoding.ASCII, true)) // true to leave stream open
                    {
                        signatureChars = sigReader.ReadChars(12);
                    }
                    string signature = new string(signatureChars);

                    if (signature != "ElecbyteSpr\0") {
                        Debug.LogError($"[Sff.Read] Invalid SFF signature: {signature} in file {pathFile}");
                        return null;
                    }

                    // byte verHi = versionHeader[15]; // Este é o verhi (2 para SFFv2, 1 para SFFv1)
                    byte verHi = versionHeader[15]; // Byte 15 (0-indexed) é o verhi

                    if (verHi == 2) // SFFv2
                    {
                        SffV2 sffV2Reader = new SffV2();
                        sffV2Reader.ReadFromFile(pathFile, sffInfo); // Passa sffInfo para ser populado
                        // sffInfo.verHi, verLo1 etc. devem ser populados por SffV2.ReadFromFile
                    }
                    else if (verHi == 1) // SFFv1
                    {
                        SffV1 sffV1Reader = new SffV1();
                        sffV1Reader.ReadFromFile(pathFile, sffInfo); // Passa sffInfo para ser populado
                        // sffInfo.verHi, verLo1 etc. devem ser populados por SffV1.ReadFromFile
                    }
                    else
                    {
                        // Fallback para uma leitura mais simples do header se verHi não for claro
                        // ou se a lógica acima estiver incorreta para SFFv1 (onde verhi é 0x01, não '1')
                        // A especificação SFFv1 diz que os 4 bytes de versão são 1.0.0.0 (verhi = 1)
                        // A especificação SFFv2 diz que os 4 bytes de versão são 2.0.0.0 (verhi = 2)
                        // O byte em file offset 15 é o verhi.
                        Debug.LogError($"[Sff.Read] Error: Unknown SFF version or verHi byte not standard. verHi byte: {verHi} in file {pathFile}");
                        // throw new UnityException("Sff version " + verHi + " not supported!");
                        return null;
                    }
                } // FileStream é fechado aqui
                return sffInfo;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Sff.Read] Exception while reading SFF file '{pathFile}': {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }
    }
}
