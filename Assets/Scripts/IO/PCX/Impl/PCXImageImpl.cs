using MugenForever.IO.PAL;
using System;
using System.IO;
using System.Net;
using System.Numerics;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Profiling;
using static MugenForever.IO.PCX.PCXImageImpl;
using static UnityEditor.PlayerSettings.SplashScreen;
using BinaryReader = MugenForever.Util.BinaryReader;

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
namespace MugenForever.IO.PCX
{
    internal class PCXImageImpl : IPCXImage
    {
        private readonly int _headerSize = 128;
        private Texture2D _texture2D;

        public PCXImageImpl(Stream data, bool isTransparent = true, IPalette palette = null)
        {

            BinaryReader.ReadJump(data, 0, SeekOrigin.Begin);

            var header = ReadHeader(data);

            BuildImageToTexture(data, header, isTransparent, palette);

            Debug.Log(header);

        }

        private void BuildImageToTexture(Stream data, PCXHeader header, bool isTransparent, IPalette palette)
        {

            BinaryReader.ReadJump(data, _headerSize, SeekOrigin.Begin);

            _texture2D = (header.Version == 5 &&  header.BitsPerPixel == 8 && (header.NPlanes == 3 || header.NPlanes == 4))? 
                         LoadTrueColor(data, header) : 
                         LoadIndexed(data, header, isTransparent, palette);

            _texture2D.filterMode = FilterMode.Point;
            _texture2D.wrapMode = TextureWrapMode.Clamp;

        }

        private Texture2D LoadTrueColor(Stream data, PCXHeader header)
        {
            Texture2D texture = new(header.Width, header.Height, TextureFormat.RGB24, false);

            for (int y = header.Height-1; y >= 0; y--)
            {

                int scanlineLength = header.BytesPerLine * header.NPlanes;
                byte[] scanline = new byte[scanlineLength];
                RLEDecode(data, scanline, header.IsCompressed);

                for (int x = 0; x < header.Width; x++)
                {
                    byte r = scanline[x];
                    byte g = scanline[x + header.Width];
                    byte b = scanline[x + (header.Width * 2)];
                    texture.SetPixel(x, y, new Color32(r, g, b, 255));
                }

            }

            texture.Apply();
            return texture;
        }
        
        private Texture2D LoadIndexed(Stream data, PCXHeader header, bool isTransparent, IPalette palette)
        {
            Texture2D texture = new(header.Width, header.Height, TextureFormat.RGBA32, false);


            if (palette == null) {
                
                palette = new PaletteImpl();

                if (header.BitsPerPixel ==1 && header.NPlanes == 4)
                {
                    palette.Load(new MemoryStream(header.PaletteOfImage), header.PaletteOfImage.Length);
                }
                else if(header.BitsPerPixel == 8 && header.NPlanes == 1)
                {
                    data.Seek(((IPalette.SIZE+1) * -1), SeekOrigin.End);

                    if(!IsPaletteAttached(data)) 
                        throw new System.Exception("Não foi possível ler a paleta de cores");

                    palette.Load(data);

                    Array.Reverse(palette.PalleteColor);

                    // volta para o inicio do arquivo após leitura do header
                    data.Seek(_headerSize, SeekOrigin.Begin);
                }
            }

            if(isTransparent)
                palette.PalleteColor[0] = new Color32(0, 0, 0, 0);

            byte[] scanline = new byte[header.BytesPerLine];

            for (int y = header.Height-1; y >= 0; y--)
            {
                RLEDecode(data, scanline, header.IsCompressed);
                
                for (int x = 0; x < header.Width; x++)
                {
                    texture.SetPixel(x, y, palette.PalleteColor[scanline[x]]);
                }

            }

            texture.Apply();
            return texture;
        }

        private void RLEDecode(Stream data, byte[] scanline, bool isCompress)
        {
            if (isCompress)
            {
                int index = 0;
                while (index < scanline.Length && data.Position < data.Length)
                {
                    int count = 1;
                    byte value = BinaryReader.ReadByte(data);

                    // verificar se há a necessidade de duplicar o próximo byte
                    // verificar se os 2 primeiros bytes foram informados isso significa que os proximos 6 bytes representa o total de repicação
                    // mask 11000000
                    if ((value & 0xC0) == 0xC0)
                    {
                        // recupera o total de replicação
                        // mask 00111111
                        count = value & 0x3F;
                        value = BinaryReader.ReadByte(data);
                    }

                    for (int i = 0; i < count; i++)
                        if(index < scanline.Length) scanline[index++] = value;
                }
            }
            else
            {
                scanline = BinaryReader.ReadBytes(data, scanline.Length);
            }
        }

        private PCXHeader ReadHeader(Stream stream)
        {
            PCXHeader header = new ()
            {
                Manufacturer        = BinaryReader.ReadInt(stream, 1),
                Version             = BinaryReader.ReadInt(stream, 1),
                IsCompressed        = BinaryReader.ReadBool(stream),
                BitsPerPixel        = BinaryReader.ReadInt(stream, 1),

                //window image dimensions
                XStart              = BinaryReader.ReadInt(stream, 2),
                YStart              = BinaryReader.ReadInt(stream, 2),
                XEnd                = BinaryReader.ReadInt(stream, 2),
                YEnd                = BinaryReader.ReadInt(stream, 2),

                XDpi                = BinaryReader.ReadInt(stream, 2),
                YDpi                = BinaryReader.ReadInt(stream, 2),

                PaletteOfImage      = BinaryReader.ReadBytes(stream, 48),
                Reserved            = BinaryReader.ReadInt(stream, 1),
                NPlanes             = BinaryReader.ReadInt(stream, 1),
                BytesPerLine        = BinaryReader.ReadInt(stream, 2),
                PaletteInfo         = BinaryReader.ReadInt(stream, 2),
                XScreenSize         = BinaryReader.ReadInt(stream, 2),
                YScreenSize         = BinaryReader.ReadInt(stream, 2),
                Filler              = BinaryReader.ReadBytes(stream, 54),
            };

            header.Width                = (header.XEnd - header.XStart) + 1;
            header.Height               = (header.YEnd - header.YStart) + 1;
            header.TotalBytes           = header.NPlanes * header.BytesPerLine;
            header.NumberTotalPixel     = header.Width * header.Height; 

            return header;
        }

        private bool IsPaletteAttached(Stream data)
        {
            data.Seek((IPalette.SIZE + 1) * -1, SeekOrigin.End);
            var paletteAttached = data.ReadByte();
            return paletteAttached == IPalette.CODE_ATTACHED;
        }

        public partial class PCXHeader
        {
            public int Manufacturer;
            public int Version;
            public bool IsCompressed;
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
            public int NumberTotalPixel;
        }

        public Texture2D Texture2D => _texture2D;

        //
    }
}