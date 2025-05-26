using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MugenForever.Sff
{
    /***
     SFF header 2.00
    0    0    12   "ElecbyteSpr\0" signature
    12    C     1   verlo3; 0
    13    D     1   verlo2; 0
    14    E     1   verlo1; 0
    15    F     1   verhi; 2
    16   10     4   reserved; 0
    20   14     4   reserved; 0
    24   18     1   compatverlo3; 0
    25   19     1   compatverlo1; 0
    26   1A     1   compatverlo2; 0
    27   1B     1   compatverhi; 2
    28   1C     4   reserved; 0
    32   20     4   reserved; 0
    36   24     4   offset where first sprite node header data is located
    40   28     4   Total number of sprites
    44   2C     4   offset where first palette node header data is located
    48   30     4   Total number of palettes
    52   34     4   ldata offset
    56   38     4   ldata length
    60   3C     4   tdata offset
    64   40     4   tdata length
    68   44     4   reserved; 0
    72   48     4   reserved; 0
    76   4C   436   unused
    ----
    Sprite node
    dec  hex  size   meaning
    0    0     2   groupno
    2    2     2   itemno
    4    4     2   width
    6    6     2   height
    8    8     2   axisx
    10    A     2   axisy
    12    C     2   Index number of the linked sprite (if linked)
    14    E     1   fmt
    15    F     1   coldepth
    16   10     4   offset into ldata or tdata
    20   14     4   Sprite data length (0: linked)
    24   18     2   palette index
    26   1A     2   flags
    ----
    fmt
    0 raw
    1 invalid (no use)
    2 RLE8
    3 RLE5
    4 LZ5
    ----
    flags
    0    unset: literal (use ldata); set: translate (use tdata; decompress on load)
    1-15 unused
     ***/
    public class SffV2 : Sff
    {
        public byte[] pcx;
        public byte[] png;
        public Texture2D texture;

        public override void ReadFromFile(string pathFile)
        {
            FileStream fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);

            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fileStream);
            // fileStream.Seek(0, SeekOrigin.Begin); // BinaryReader starts at the beginning

            signature = new string(binaryReader.ReadChars(12));
            version = String.Format("{3}.{2}.{1}.{0}", binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());

            // Pula 
            binaryReader.BaseStream.Seek(8, SeekOrigin.Current); // Skip 8 reserved bytes

            // totalGroups = binaryReader.ReadInt32(); // This was commented out in original
            compatVerLoad = String.Format("{3}.{2}.{1}.{0}", binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
            
            // Jump reserved bytes
            binaryReader.BaseStream.Seek(8, SeekOrigin.Current); // Skip 8 reserved bytes

            offsetSubFile = binaryReader.ReadInt32();
            totalImage = binaryReader.ReadInt32();

            offsetPaletteFile = binaryReader.ReadInt32();
            totalPalette = binaryReader.ReadInt32();

            offsetLData = binaryReader.ReadInt32();
            sizeLData = binaryReader.ReadInt32();

            offsetTData = binaryReader.ReadInt32();
            sizeTData = binaryReader.ReadInt32();

            // Jump reserved bytes
            binaryReader.BaseStream.Seek(8, SeekOrigin.Current); // Skip 8 reserved bytes

            comments = new string(binaryReader.ReadChars(436));

            binaryReader.BaseStream.Seek(offsetSubFile, SeekOrigin.Begin); // Seek to the first sprite node

            sprites = new List<SffSprite>();
            spriteList = new Dictionary<int, Dictionary<int, SffSprite>>();

            for (int i = 0; i < totalImage; i++) // Corrected loop to iterate all images
            {
                SffSprite spr = new SffSprite();

                spr.groupNumber = binaryReader.ReadInt16();
                spr.imageNumber = binaryReader.ReadInt16();

                spr.width = binaryReader.ReadInt16();
                spr.height = binaryReader.ReadInt16();

                spr.axisX = binaryReader.ReadInt16();
                spr.axisY = binaryReader.ReadInt16();                
                
                spr.indexPreviousLinked = binaryReader.ReadInt16();
                spr.index = i + 1; // Assuming index is 1-based and corresponds to loop iteration

                spr.fmt = binaryReader.ReadByte();
                
                CompressorType compressorType = CompressorType.RAW;
                if (Enum.IsDefined(typeof(CompressorType), spr.fmt))
                    compressorType = (CompressorType)Enum.ToObject(typeof(CompressorType), spr.fmt);

                Compressor compressor = CompressorFactory.getCompressor(compressorType);

                spr.coldepth = binaryReader.ReadByte();
                spr.offsetData = binaryReader.ReadInt32();
                spr.subfileLength = binaryReader.ReadInt32();
                spr.paletteIndex = binaryReader.ReadInt16();
                spr.flag = binaryReader.ReadInt16();

                if (spr.subfileLength == 0 && spr.indexPreviousLinked != 0 && sprites.Exists(sff => sff.index == spr.indexPreviousLinked))
                {
                    // This is a linked sprite. Copy data from the source sprite.
                    // Note: SffSprite might need to be a class, and pcx might need deep copy if it's mutable.
                    SffSprite sourceSprite = sprites.Find(sff => sff.index == spr.indexPreviousLinked);
                    if (sourceSprite != null)
                    {
                        spr.pcx = sourceSprite.pcx; // This should be fine if Pcx holds immutable data or is correctly shared
                        spr.image = sourceSprite.image; // Share the Texture2D
                        spr.width = sourceSprite.width;
                        spr.height = sourceSprite.height;
                        // other relevant fields can be copied if necessary
                    }
                    else
                    {
                        // Potentially log a warning if a linked sprite points to a non-existing index
                        Debug.LogWarningFormat("SFFv2: Linked sprite {0},{1} points to non-existent index {2}", spr.groupNumber, spr.imageNumber, spr.indexPreviousLinked);
                    }
                }
                else if (spr.subfileLength > 0)
                {
                    long currentPosition = binaryReader.BaseStream.Position; // Save position after reading sprite node
                    
                    long imageDataFileOffset = ((spr.flag & 0x01) == 0) ? offsetLData : offsetTData;
                    imageDataFileOffset += spr.offsetData;
                    
                    binaryReader.BaseStream.Seek(imageDataFileOffset, SeekOrigin.Begin);
                    byte[] imageBytes = binaryReader.ReadBytes(spr.subfileLength);

                    if (compressor != null)
                    {
                        imageBytes = compressor.descompress(imageBytes);
                    }

                    // Assuming spr.pcx is initialized (e.g., in SffSprite constructor)
                    // It's safer to initialize it here if not: if(spr.pcx == null) spr.pcx = new Pcx();
                    if (spr.pcx == null) spr.pcx = new Pcx(); // Ensure pcx object exists

                    spr.pcx.load(new MemoryStream(imageBytes)); // Pcx class needs to handle this
                    spr.image = spr.pcx.image; // Texture should be created within Pcx.load

                    binaryReader.BaseStream.Seek(currentPosition, SeekOrigin.Begin); // Restore position to read next sprite node
                }
                // If subfileLength is 0 and not linked, it's an empty sprite, pcx and image remain null/default.

                spr.name = spr.groupNumber + "-" + spr.imageNumber; // Removed " " + spr.comments as spr.comments is for the SFF file, not individual sprites

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

            }
        }
    }
}
