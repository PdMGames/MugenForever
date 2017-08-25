using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MugenForever.Act
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
        public Color32 color = new Color32(255,0,0,255);

        public RGBA(int red, int green, int blue, int alpha)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;

            color.r = BitConverter.GetBytes(this.red)[0];
            color.g = BitConverter.GetBytes(this.green)[0];
            color.b = BitConverter.GetBytes(this.blue)[0];
        }
    }
}
