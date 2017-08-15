using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MugenForever.Sff
{
    /***
    SFF header 1.00
    Version 1.01
    HEADER (512 bytes)
    ------
    Bytes
    00-11 "ElecbyteSpr\0" signature	[12]
    12-15 1 verhi, 1 verlo, 1 verlo2, 1 verlo3	[04]
    16-19 Number of groups	[04]
    20-24 Number of images	[04]
    24-27 File offset where first subfile is located	[04]
    28-31 Size of subheader in bytes	[04]
    32 Palette type (1=SPRPALTYPE_SHARED or 0=SPRPALTYPE_INDIV)	[01]
    33-35 Blank; set to zero	[03]
    36-511 Blank; can be used for comments	[476]
    SUBFILEHEADER (32 bytes)
    -------
    Bytes
    00-03 File offset where next subfile in the "linked list" is	[04]
    located. Null if last subfile
    04-07 Subfile length (not including header)	[04]
    Length is 0 if it is a linked sprite
    08-09 Image axis X coordinate	[02]
    10-11 Image axis Y coordinate	[02]
    12-13 Group number	[02]
    14-15 Image number (in the group)	[02]
    16-17 Index of previous copy of sprite (linked sprites only)	[02]
    This is the actual
    18 True if palette is same as previous image	[01]
    19-31 Blank; can be used for comments	[14]
    32- PCX graphic data. If palette data is available, it is the last
    768 bytes.
     ***/
    public class SffV1 : Sff
    {
        public override void readFromFile(string pathFile)
        {
            FileStream fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);
            // set the pointer in start of file
            fileStream.Seek(0, SeekOrigin.Begin);

            signature = ReadString(fileStream, 12);
            version = String.Format("{3}.{2}.{1}.{0}", fileStream.ReadByte(), fileStream.ReadByte(), fileStream.ReadByte(), fileStream.ReadByte());
            totalGroups= ReadInt(fileStream, 4);
            totalImage = ReadInt(fileStream,4);
            offsetSubFile = ReadInt(fileStream, 4);
            sizeSubFileHeader = ReadInt(fileStream, 4);
            paletteType = ReadInt(fileStream, 1);
            //paletteType = fileStream.ReadByte();
            //ReadJump(fileStream, 1); //paletteType
            ReadJump(fileStream, 3); //blank space
            comments = ReadString(fileStream, 476); //comments

            Debug.Log(fileStream.Position);

            sprites = new Dictionary<int, Dictionary<int, SffSprite>>();

            for (int i = 0; i < totalImage; i++)
            {


                /**
                    00-03 File offset where next subfile in the "linked list" is	[04]
                    located. Null if last subfile
                    04-07 Subfile length (not including header)	[04]
                    Length is 0 if it is a linked sprite
                    08-09 Image axis X coordinate	[02]
                    10-11 Image axis Y coordinate	[02]
                    12-13 Group number	[02]
                    14-15 Image number (in the group)	[02]
                    16-17 Index of previous copy of sprite (linked sprites only)	[02]
                    This is the actual
                    18 True if palette is same as previous image	[01]
                    19-31 Blank; can be used for comments	[14]
                    32- PCX graphic data. If palette data is available, it is the last
                    768 bytes
                */

                SffSprite spr = new SffSprite();

                Debug.Log("LOOP START Posicao: " + fileStream.Position);

                spr.nextFileOffset = ReadInt(fileStream, 4);
                spr.subfileLength = ReadInt(fileStream, 4);

                spr.axisX = ReadInt(fileStream, 2);
                spr.axisY = ReadInt(fileStream, 2);
                spr.groupNumber = ReadInt(fileStream, 2);
                spr.imageNumber = ReadInt(fileStream, 2);
                spr.indexPreviousLinked = ReadInt(fileStream, 2);

                if (spr.subfileLength == 0 && spr.indexPreviousLinked != 0 && sprites.ContainsKey(spr.groupNumber))
                    spr.subfileLength = sprites[spr.groupNumber][spr.imageNumber].subfileLength;

                spr.samePaleteOfPreviousImage = ReadBool(fileStream);
                spr.comments = ReadString(fileStream, 13);
                
                byte[] imageBytes = ReadBytes(fileStream, spr.subfileLength);

                BinaryWriter w = new BinaryWriter(File.OpenWrite(String.Format("imagem.g-{0}.i-{1}.pcx", spr.groupNumber, spr.imageNumber)));

                // Writer raw data                
                w.Write(imageBytes);
                w.Flush();
                w.Close();

                
                if (sprites.ContainsKey(spr.groupNumber))
                {
                    Dictionary<int, SffSprite> dicSPR = sprites[spr.groupNumber];
                    dicSPR.Add(spr.imageNumber, spr);
                }
                else
                {
                    Dictionary<int, SffSprite> dicSPR = new Dictionary<int, SffSprite>();
                    dicSPR.Add(spr.imageNumber, spr);
                    sprites.Add(spr.groupNumber, dicSPR);
                }
                
            }

            Debug.Log(sprites[1][2].comments);
        }
    }
}
