using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MugenForever.SFF
{
    public class SFFFactory : MonoBehaviour
    {

        public static SFFInfo read(string pathFile)
        {
            SFFInfo sffInfo = null;
            using (FileStream fsSource = (new FileStream(pathFile, FileMode.Open, FileAccess.Read)))
            {

                // Recuperando a versão do SFF
                byte[] version = new byte[16];
                fsSource.Read(version, 0, 16);

                // Instanciando classe responsável pelo leitura da versão do SFF
                Type typeClass = Type.GetType("MugenForever.SFF.SFFReadV" + version[15]);
                SFFInterface sffImpl = (SFFInterface)Activator.CreateInstance(typeClass);

                // Lendo e Retornando o SFF informação
                return sffImpl.read(fsSource);

            }

        }

    }
}