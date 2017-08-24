using System;
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
        public override void ReadFromFile(string pathFile)
        {
            FileStream fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);

            // Setando o ponteiro para o inico do arquivo
            fileStream.Seek(0, SeekOrigin.Begin);

            signature = ReadString(fileStream, 12);
            version = String.Format("{3}.{2}.{1}.{0}", ReadInt(fileStream, 1), ReadInt(fileStream, 1), ReadInt(fileStream, 1), ReadInt(fileStream, 1));

            // Pula 
            ReadJump(fileStream, 8);

            //  totalGroups = ReadInt(fileStream, 4);
            compatVerLoad = String.Format("{3}.{2}.{1}.{0}", ReadInt(fileStream, 1), ReadInt(fileStream, 1), ReadInt(fileStream, 1), ReadInt(fileStream, 1));
            
            // Pula 
            ReadJump(fileStream, 8);

            offsetSubFile = ReadInt(fileStream, 4);
            totalImage = ReadInt(fileStream, 4);
            
            /*sizeSubFileHeader = ReadInt(fileStream, 4);
            paletteType = ReadInt(fileStream, 1);
            ReadJump(fileStream, 3); //blank space
            comments = ReadString(fileStream, 476); //comments

            // Movendo ponteiro
            fileStream.Seek(36, SeekOrigin.Begin);
            totalImage = ReadInt(fileStream, 4);
            offsetSubFile = ReadInt(fileStream, 4); ;*/
        }
    }
}
