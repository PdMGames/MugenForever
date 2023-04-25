using MugenForever.IO.PXC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

// using Color = System.Drawing.Color;

/**
ZSoft .PCX FILE HEADER FORMAT
Byte      Item          Size   Description/Comments 
0         Manufacturer  1      Constant Flag, 10 = ZSoft .pcx 
1         Version       1      Version information 
                                    0 = Version 2.5 of PC Paintbrush 
                                    2 = Version 2.8 w/palette information 
                                    3 = Version 2.8 w/o palette information 
                                    4 = PC Paintbrush for Windows(Plus for Windows uses Ver 5) 
                                    5 = Version 3.0 and > of PC Paintbrush and PC Paintbrush +, includes Publisher's Paintbrush . Includes 24-bit .PCX files 
2         Encoding      1       1 = .PCX run length encoding 
3         BitsPerPixel  1     Number of bits to represent a pixel (per Plane) - 1, 2, 4, or 8 
4         Window        8     Image Dimensions: Xmin,Ymin,Xmax,Ymax 
12        HDpi          2     Horizontal Resolution of image in DPI* 
14        VDpi          2     Vertical Resolution of image in DPI* 
16        Colormap      48    Color palette setting, see text 
64        Reserved      1     Should be set to 0. 
65        NPlanes       1     Number of color planes 
66        BytesPerLine  2     Number of bytes to allocate for a scanline plane.  MUST be an EVEN number.  Do NOT calculate from Xmax-Xmin. 
68        PaletteInfo   2     How to interpret palette- 1 = Color/BW, 2 = Grayscale (ignored in PB IV/ IV +) 
70        HscreenSize   2     Horizontal screen size in pixels. New field found only in PB IV/IV Plus 
72        VscreenSize   2     Vertical screen size in pixels. New field found only in PB IV/IV Plus 
74        Filler        54    Blank to fill out 128 byte header.  Set all bytes to 0 
*/
namespace MugenForever.IO.PCX {
    public class PCXImageImpl : IPCXImage
    {
        private readonly PCXHeader _header;
        private readonly Stream _data;
        private readonly int _headerSize = 128;
        private Texture2D _texture2D;

        public PCXImageImpl(Stream pcx) : this(pcx, null)
        {
            
        }

        PCXImageImpl(Stream pcx, IPalette pallete)
        {
            _data = pcx;
            BinaryReader binaryReader = new(_data);

            _header = ReadHeader(binaryReader);

            pallete ??= new PaletteImpl(_data);
            
            binaryReader.BaseStream.Seek(_headerSize, SeekOrigin.Begin);

            BuildImageToTexture(_data, pallete);

            Debug.Log(_header);

        }

        private void BuildImageToTexture(Stream data, IPalette pallete)
        {
            Texture2D texture = new(_header.Width, _header.Height, TextureFormat.ARGB32, false);
            BinaryReader binaryReader = new(data);

            // size x and y
            var xmove = 0;
            var ymove = _header.Height;

            // set transparent canal.
            pallete.PalleteColor[0] = new Color32(0,0,0,0);

            //Array.Reverse(pallete.PalleteColor);

            // read line y
            while (ymove >= 0)
            {
                // verificar se há a necessidade de duplicar o próximo byte
                int index = binaryReader.ReadByte();
                int count = 1;

                // verificar se os 2 primeiros bytes foram informados isso significa que os proximos 6 bytes representa o total de repicação
                // mask 11000000 
                // recupera o total de replicação
                // mask 00111111
                if ((index & 0xC0) == 0xC0)
                {
                    count = index & 0x3F;
                    index = binaryReader.ReadByte();
                }

                for (int i = 0; i < count; i++)
                {
                    if (xmove < _header.Width)
                        texture.SetPixel(xmove, ymove, pallete.PalleteColor[index]);

                    xmove++;

                    if (xmove == _header.BytesPerLine)
                    {
                        xmove = 0;
                        ymove--;
                        break;
                    }
                }


            }

            texture.Apply();
            _texture2D = texture;
        }

        //<<summary>>
        //    Read Header 128bytes
        //<<summary>
        private PCXHeader ReadHeader(BinaryReader binaryReader)
        {
            PCXHeader header = new()
            {
                Manufacturer = binaryReader.ReadByte(),
                Version = binaryReader.ReadByte(),
                Encoding = binaryReader.ReadByte(),
                BitsPerPixel = binaryReader.ReadByte(),

                //window image dimensions
                XStart = binaryReader.ReadInt16(),
                YStart = binaryReader.ReadInt16(),
                XEnd = binaryReader.ReadInt16(),
                YEnd = binaryReader.ReadInt16(),

                XDpi = binaryReader.ReadInt16(),
                YDpi = binaryReader.ReadInt16(),

                PaletteOfImage = binaryReader.ReadBytes(48),
                Reserved = binaryReader.ReadByte(),
                NPlanes = binaryReader.ReadByte(),
                BytesPerLine = binaryReader.ReadInt16(),
                PaletteInfo = binaryReader.ReadInt16(),
                XScreenSize = binaryReader.ReadInt16(),
                YScreenSize = binaryReader.ReadInt16(),
                Filler = binaryReader.ReadBytes(54),
            };

            header.Width = (header.XEnd - header.XStart) + 1;
            header.Height = (header.YEnd - header.YStart) + 1;
            header.TotalBytes = header.NPlanes * header.BytesPerLine;

            return header;
        }

        public partial class PCXHeader
        {
            public int Manufacturer;
            public int Version;
            public int Encoding;
            public int BitsPerPixel;
            public int XStart;
            public int YStart;
            public int XEnd;
            public int YEnd;
            public int XDpi;
            public int YDpi;
            public int Reserved;
            public int NPlanes;
            public int BytesPerLine;
            public int PaletteInfo;
            public byte[] PaletteOfImage;
            public int XScreenSize;
            public int YScreenSize;
            public byte[] Filler;
            public int Width;
            public int Height;
            public int sizeHeader = 128;
            public int TotalBytes;
        }

        public PCXHeader Header { get { return _header; } }
        public Texture2D Texture2D { get { return _texture2D; } }
    }
}