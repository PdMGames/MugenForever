using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Read SFF V1
/// The file specification
/// <include file='xml_include_tag.doc' path='Docs/SFFV1_SPEC.md' />
/// </summary>
namespace MugenForever.Sff
{
    public class SffV1 : Sff
    {
        [ContextMenu("Load From File")]
        public void LoadInEditor()
        {
            string file = EditorUtility.OpenFilePanel("Select Mugen SFF Char File", "", "sff");

            if (file.Length != 0)
            {
                ReadFromFile(file);
            }
        }

        public override void ReadFromFile(string pathFile)
        {
            FileStream fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);
            // set the pointer in start of file
            fileStream.Seek(0, SeekOrigin.Begin);

            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fileStream);

            signature = new string(binaryReader.ReadChars(12));//.ReadString(fileStream, 12);
            version = String.Format("{3}.{2}.{1}.{0}", binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
            totalGroups = binaryReader.ReadInt32();
            totalImage = binaryReader.ReadInt32();
            offsetSubFile = binaryReader.ReadInt32();
            sizeSubFileHeader = binaryReader.ReadInt32();
            paletteType = binaryReader.ReadByte();
            binaryReader.BaseStream.Seek(3,SeekOrigin.Current);
            //ReadJump(fileStream, 3); //blank space
            int lenghtComments = 512 - Int32.Parse(binaryReader.BaseStream.Position.ToString());
            comments = new string(binaryReader.ReadChars(lenghtComments));

            sprites = new List<SffSprite>();
            spriteList = new Dictionary<int, Dictionary<int, SffSprite>>();

            for (int i = 0; i < totalImage; i++)
            {
                SffSprite spr = new SffSprite();

                spr.nextFileOffset = binaryReader.ReadInt32();
                spr.subfileLength = binaryReader.ReadInt32();

                spr.axisX = binaryReader.ReadInt16();
                spr.axisY = binaryReader.ReadInt16();
                spr.groupNumber = binaryReader.ReadInt16();
                spr.imageNumber = binaryReader.ReadInt16();
                spr.indexPreviousLinked = binaryReader.ReadInt16();
                spr.index = i + 1;

                spr.samePaletteOfPreviousImage = binaryReader.ReadByte() == 1;
                spr.comments = new string(binaryReader.ReadChars(13));

                if (spr.subfileLength == 0 && spr.indexPreviousLinked != 0 && sprites.Exists(sff => sff.index == spr.indexPreviousLinked))
                {
                    //spr.subfileLength = sprites[spr.indexPreviousLinked].subfileLength;
                    spr.pcx = sprites[spr.indexPreviousLinked].pcx;
                }
                else
                {
                    byte[] imageBytes = binaryReader.ReadBytes(spr.subfileLength);
                    spr.pcx.load(new MemoryStream(imageBytes));

                    //spr.image.LoadRawTextureData(spr.pcx.image.GetRawTextureData());

                    /*if (spr.pcx.image != null)
                    {
                        BinaryWriter w = new BinaryWriter(File.OpenWrite(String.Format("image.g-{0}.i-{1}.png", spr.groupNumber, spr.imageNumber)));

                        // Writer raw data                
                        w.Write(spr.pcx.image.GetRawTextureData());
                        w.Flush();
                        w.Close();
                    }*/
                }
                spr.image = spr.pcx.image;

                spr.name = String.Format("[{0}][{1}][{2}]", spr.index.ToString("000"), spr.groupNumber, spr.imageNumber);

                spr.sprite = new Sprite();
                

                
                 

                //create dictionary for group
                if (spriteList.ContainsKey(spr.groupNumber))
                {
                    Dictionary<int, SffSprite> dicSPR = spriteList[spr.groupNumber];
                    sprites.Add(spr);
                    dicSPR.Add(spr.imageNumber, spr);
                }
                else
                {
                    sprites.Add(spr);
                    Dictionary<int, SffSprite> dicSpr = new Dictionary<int, SffSprite>();
                    dicSpr.Add(spr.imageNumber, spr);
                    spriteList.Add(spr.groupNumber, dicSpr);
                }

                // Jump for next image
                binaryReader.BaseStream.Seek(spr.nextFileOffset, SeekOrigin.Begin);
            }
        }
    }
}
