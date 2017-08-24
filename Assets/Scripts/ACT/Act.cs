using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MugenForever.Act
{
    public class Act : MugenForever.Reader.Binary
    {
        public string signature;
        public string version;
        public string compatVerLoad;
        public int totalGroups;
        public int totalImage;
        public int offsetSubFile;
        public int sizeSubFileHeader;
        public int paletteType;
        public string comments;

        public List<SffSprite> sprites;
        public Dictionary<int, Dictionary<int, SffSprite>> spriteList;

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

            // get the sff version
            byte[] version = new byte[16];
            fsSource.Read(version, 0, 16);

            string verlo3 = version[15].ToString();

            if ( verlo3 == "2")
            {
                SffV2 sff = gameObject.AddComponent<SffV2>();
                sff.fileName = pathFile;
            }
            else if (verlo3 == "1")
            {
                SffV1 sff = gameObject.AddComponent<SffV1>();
                sff.fileName = pathFile;
            }
            else
            {
                throw new UnityException("Sff version " + verlo3 + " not supported!");
            }

            //remove this component
            Destroy(this);
        }
    }
}
