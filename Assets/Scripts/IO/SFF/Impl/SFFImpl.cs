using Assets.Scripts.IO.SFF;
using MugenForever.IO.PAL;
using MugenForever.IO.PCX;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BinaryReader = MugenForever.Util.BinaryReader;

/*
| SFF file structure
|--------------------------------------------------*\
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
 */
namespace MugenForever.IO.SFF
{

    internal class SFFImpl : ISFF
    {

        private string _version;
        private Dictionary<int, Dictionary<int, SFFSprite>> _sprites;

        public SFFImpl(Stream data, IPalette palette) {

            SFFHeader header = ReadHeader(data);

            _version = string.Format("{0}.{1}.{2}.{3}", header.Verlo3, header.Verlo2, header.Verlo, header.Verhi);

            ProcessSubHeader(data, header, palette);

            Debug.Log("Sff read");
        }

        private SFFHeader ReadHeader(Stream data) {
            SFFHeader header = new()
            {
                Signature               = BinaryReader.ReadString(data, 12),
                Verhi                   = BinaryReader.ReadInt(data, 1),
                Verlo                   = BinaryReader.ReadInt(data, 1),
                Verlo2                  = BinaryReader.ReadInt(data, 1),
                Verlo3                  = BinaryReader.ReadInt(data, 1),
                TotalGroup              = BinaryReader.ReadInt(data, 4),
                TotalImage              = BinaryReader.ReadInt(data, 4),
                OffsetFirstSubfile      = BinaryReader.ReadInt(data, 4),
                SizeSubHeader           = BinaryReader.ReadInt(data, 4),
                PaletteType             = BinaryReader.ReadInt(data, 1),
                Comments                = BinaryReader.ReadString(data, 479),
            };

            return header;
        }

        private SFFSubHeader ReadSubHeader(Stream data, IPalette palette)
        {
            SFFSubHeader subHeader = new()
            {
                OffsetNextSubfile       = BinaryReader.ReadInt(data, 4),
                Size                    = BinaryReader.ReadInt(data, 4),
                AxisX                   = BinaryReader.ReadInt(data, 2),
                AxisY                   = BinaryReader.ReadInt(data, 2),
                Group                   = BinaryReader.ReadInt(data, 2),
                Index                   = BinaryReader.ReadInt(data, 2),
                LinkIndex               = BinaryReader.ReadInt(data, 2),
                IsPalettePrevious       = BinaryReader.ReadBool(data),
                Comments                = BinaryReader.ReadString(data, 13),
            };

            Stream pcxData = new MemoryStream(BinaryReader.ReadBytes(data, subHeader.Size));            
            
            //9000 pal attached
            if(subHeader.Group == 9000 && subHeader.Index == 1)
            {
                BinaryReader.ReadJump(pcxData, (IPalette.SIZE * -1), SeekOrigin.End);
                palette = new PaletteImpl(pcxData);
            }

            /*using FileStream st = new("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\teste.pcx", FileMode.Create);
            st.Write(BinaryReader.ReadBytes(pcxData, (int)pcxData.Length));
            st.Close();*/

            subHeader.PCX = (subHeader.LinkIndex == 0) ? new PCXImageImpl(pcxData, true, palette) : null;

            
            return subHeader;
        }

        private void ProcessSubHeader(Stream data, SFFHeader header, IPalette palette)
        {
            _sprites = new();
            int nextOffset = header.OffsetFirstSubfile;
            //IPCXImage[] pcxLink = new IPCXImage[header.TotalImage];
            Array.Reverse(palette.PalleteColor);

            for (int i=0; i<header.TotalImage; i++)
            {
                BinaryReader.ReadJump(data, nextOffset, SeekOrigin.Begin);
                SFFSubHeader subHeader = ReadSubHeader(data, palette);
                nextOffset = subHeader.OffsetNextSubfile;

                SFFSprite sprite = new() {
                    AxisX       = subHeader.AxisX,
                    AxisY       = subHeader.AxisY,
                    Group       = subHeader.Group,
                    Index       = subHeader.Index,
                    PCX         = subHeader.PCX,
                };

                // link image
                if (subHeader.Size == 0 && subHeader.LinkIndex != 0)
                {
                    sprite.IsLinkedImage = true;
                    sprite.PCX = _sprites[subHeader.Group][subHeader.Index - 1].PCX;
                }

                if (_sprites.ContainsKey(subHeader.Group))
                {
                    _sprites[subHeader.Group].Add(subHeader.Index, sprite);
                }
                else
                {
                    Dictionary<int, SFFSprite> sprites = new()
                    {
                        { subHeader.Index, sprite }
                    };

                    _sprites.Add(subHeader.Group, sprites);
                }

                //pcxLink[i] = sprite.PCX;
            }
        }

        public class SFFHeader
        {
            public string Signature;
            public int Verhi;
            public int Verlo;
            public int Verlo2;
            public int Verlo3;
            public int TotalGroup;
            public int TotalImage;
            public int OffsetFirstSubfile;
            public int SizeSubHeader;
            public int PaletteType;
            public string Comments;
        }

        public class SFFSubHeader
        {
            public int OffsetNextSubfile;
            public int Size;
            public int AxisX;
            public int AxisY;
            public int Group;
            public int Index;
            public int LinkIndex;
            public bool IsPalettePrevious;
            public string Comments;
            public IPCXImage PCX;
        }

        public string Version => _version;

        public Dictionary<int, Dictionary<int, SFFSprite>> Spriters => _sprites;
    }
}
