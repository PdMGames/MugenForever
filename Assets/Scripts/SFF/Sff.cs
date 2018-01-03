using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MugenForever.Sff
{
    public class Sff : MugenForever.Reader.Binary
    {
        /// <summary>
        /// Sprites
        /// </summary>
        public List<SffSprite> sprites;
        public Dictionary<int, Dictionary<int, SffSprite>> spriteList;

        /// <summary>
        /// Signature of creator
        /// </summary>
        public string signature;

        /// <summary>
        /// Version of SFF
        /// </summary>
        public string version;

        /// <summary>
        /// Version min for load this SFF
        /// </summary>
        public string compatVerLoad;

        /// <summary>
        /// Total of groups
        /// </summary>
        public int totalGroups;

        /// <summary>
        /// Total images
        /// </summary>
        public int totalImage;

        /// <summary>
        /// Offset subfile
        /// </summary>
        public int offsetSubFile;

        /// <summary>
        /// size of first subfile 
        /// </summary>
        public int sizeSubFileHeader;
        public int paletteType;       
        public string comments;

        // version SFFV2 <<
        // [L]iteral data
        public int offsetLData;
        public int sizeLData;
        
        // [T]ranslate data
        public int offsetTData;
        public int sizeTData;

        public int totalPalette;
        public int offsetPaletteFile;
        // >> version SFFV2

        

        /// <summary>
        /// Palettes
        /// </summary>
        public List<Palette.Act> palletes;
        public Dictionary<int, Dictionary<int, Palette.Act>> palletList;

        public void Start()
        {
            if (!string.IsNullOrEmpty(fileName))
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

            if (verlo3 == "2")
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
