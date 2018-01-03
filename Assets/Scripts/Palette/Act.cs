using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MugenForever.Palette
{
    public class Act : MugenForever.Reader.Binary
    {
        public List<RGBA> rbgs = new List<RGBA>();
        public Dictionary<long, RGBA> rgbList = new Dictionary<long, RGBA>();

        public void Start()
        {
            if ( !string.IsNullOrEmpty(fileName))
            {
                ReadFromFile(fileName);
            }
        }

        public override void ReadFromFile(string pathFile)
        {
            //read the file from disk
            FileStream fsSource = new FileStream(pathFile, FileMode.Open, FileAccess.Read);

            // loop invertido ... alterar caso seja necessário quando efetuar a leitura do PCX
            for (long i=fsSource.Length, index=0;i>0; i-=3, index++) {
                RGBA rgb = new RGBA(ReadInt(fsSource, 1), ReadInt(fsSource, 1), ReadInt(fsSource, 1), 255);
                rbgs.Add(rgb);
                rgbList.Add(index, rgb);
            }

            fsSource.Close();
        }
    }

    [System.Serializable]
    public class RGBA
    {
        public int red;
        public int green;
        public int blue;
        public int alpha;
        public Color32 color;

        public RGBA(int red, int green, int blue, int alpha)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;

            // CRIANDO COR
            color = new Color32(
                BitConverter.GetBytes(this.red)[0],
                BitConverter.GetBytes(this.green)[0],
                BitConverter.GetBytes(this.blue)[0],
                BitConverter.GetBytes(this.alpha)[0]
            );

        }
    }
}
