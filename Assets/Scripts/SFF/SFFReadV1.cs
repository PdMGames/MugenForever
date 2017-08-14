using System;
using System.IO;
namespace MugenForever.SFF
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
    public class SFFReadV1 : SFFInterface
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
            fileSream.Seek(20, SeekOrigin.Begin);

            byte[] totalImage = new byte[4];
            fileSream.Read(totalImage, 0, 4);
            sffInfo.totalImage = BitConverter.ToInt32(totalImage, 0);
            UnityEngine.Debug.Log(sffInfo.totalImage);

            byte[] offsetSubfile = new byte[4];
            fileSream.Read(offsetSubfile, 0, 4);
            sffInfo.offsetSubFile = BitConverter.ToInt32(offsetSubfile, 0);
            UnityEngine.Debug.Log(sffInfo.offsetSubFile);

            return sffInfo;
        }

    }
}
