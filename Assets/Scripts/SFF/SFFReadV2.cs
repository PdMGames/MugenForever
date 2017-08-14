using System;
using System.IO;
using UnityEngine;

namespace MugenForever.SFF
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
    public class SFFReadV2 : SFFInterface
    {
        public SFFInfo read(FileStream fileSream)
        {

            UnityEngine.Debug.Log(String.Format("SFFRead: {0}", GetType().Name));

            // Setando o ponteiro para o inico do arquivo
            fileSream.Seek(0, SeekOrigin.Begin);
            SFFInfo sffInfo = new SFFInfo();
            
            byte[] name = new byte[12];
            fileSream.Read(name, 0, 12);
            // recupera nome fabricante
            sffInfo.name = System.Text.Encoding.UTF8.GetString(name);
            UnityEngine.Debug.Log(sffInfo.name);

            byte[] version = new byte[4];
            sffInfo.version = String.Format("{3}.{2}.{1}.{0}", fileSream.ReadByte(), fileSream.ReadByte(), fileSream.ReadByte(), fileSream.ReadByte());
            UnityEngine.Debug.Log(sffInfo.version);

            /*byte[] totalGroup = new byte[4];
            fileSream.Read(totalGroup, 0, 4);
            sffInfo.totalGroup = BitConverter.ToInt32(totalGroup, 0);
            System.Console.WriteLine(sffInfo.totalGroup);*/
            
            // Movendo ponteiro
            fileSream.Seek(36, SeekOrigin.Begin);

            byte[] offsetSubfile = new byte[4];
            fileSream.Read(offsetSubfile, 0, 4);
            sffInfo.offsetSubFile = BitConverter.ToInt32(offsetSubfile, 0);
            UnityEngine.Debug.Log(sffInfo.offsetSubFile);

            byte[] totalImage = new byte[4];
            fileSream.Read(totalImage, 0, 4);
            sffInfo.totalImage = BitConverter.ToInt32(totalImage, 0);
            UnityEngine.Debug.Log(sffInfo.totalImage);

            return sffInfo;
        }

    }
}
