using System.IO;
using UnityEngine;
using ImageMagick;
using System.Collections.Generic;
using System;

namespace MugenForever.Sff
{
    /**
     * ZSoft .PCX FILE HEADER FORMAT
Byte      Item          Size   Description/Comments 
 0         Manufacturer 1      Constant Flag, 10 = ZSoft .pcx 
 1         Version      1      Version information 
                               0 = Version 2.5 of PC Paintbrush 
                               2 = Version 2.8 w/palette information 
                               3 = Version 2.8 w/o palette information 
                               4 = PC Paintbrush for Windows(Plus for Windows uses Ver 5) 
                               5 = Version 3.0 and > of PC Paintbrush and PC Paintbrush +, includes Publisher's Paintbrush . Includes 24-bit .PCX files 
 2         Encoding      1     1 = .PCX run length encoding 
 3         BitsPerPixel  1     Number of bits to represent a pixel (per Plane) - 1, 2, 4, or 8 
 4         Window        8     Image Dimensions: Xmin,Ymin,Xmax,Ymax 
12         HDpi          2     Horizontal Resolution of image in DPI* 
14         VDpi          2     Vertical Resolution of image in DPI* 
16         Colormap     48     Color palette setting, see text 
64         Reserved      1     Should be set to 0. 
65         NPlanes       1     Number of color planes 
66         BytesPerLine  2     Number of bytes to allocate for a scanline plane.  MUST be an EVEN number.  Do NOT calculate from Xmax-Xmin. 
68         PaletteInfo   2     How to interpret palette- 1 = Color/BW, 2 = Grayscale (ignored in PB IV/ IV +) 
70         HscreenSize   2     Horizontal screen size in pixels. New field found only in PB IV/IV Plus 
72         VscreenSize   2     Vertical screen size in pixels. New field found only in PB IV/IV Plus 
74         Filler       54     Blank to fill out 128 byte header.  Set all bytes to 0 
    */
    [System.Serializable]
    public class Pcx
    {

        public Texture2D image;
        public PcxHeader header;

        public void ReadFromFile(string pathFile)
        {
            FileStream fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);
            // set the pointer in start of file
            fileStream.Seek(0, SeekOrigin.Begin);
            load(fileStream);
        }

        public void load(Stream imageStream)
        {

            //        typedef struct _PcxHeader
            //        {
            //            BYTE	Identifier;        /* PCX Id Number (Always 0x0A) */
            //            BYTE	Version;           /* Version Number */
            //            BYTE	Encoding;          /* Encoding Format */
            //            BYTE	BitsPerPixel;      /* Bits per Pixel */
            //            WORD	XStart;            /* Left of image */
            //            WORD	YStart;            /* Top of Image */
            //            WORD	XEnd;              /* Right of Image
            //            WORD	YEnd;              /* Bottom of image */
            //            WORD	HorzRes;           /* Horizontal Resolution */
            //            WORD	VertRes;           /* Vertical Resolution */
            //            BYTE	Palette[48];       /* 16-Color EGA Palette */
            //            BYTE	Reserved1;         /* Reserved (Always 0) */
            //            BYTE	NumBitPlanes;      /* Number of Bit Planes */
            //            WORD	BytesPerLine;      /* Bytes per Scan-line */
            //            WORD	PaletteType;       /* Palette Type */
            //            WORD	HorzScreenSize;    /* Horizontal Screen Size */
            //            WORD	VertScreenSize;    /* Vertical Screen Size */
            //            BYTE	Reserved2[54];     /* Reserved (Always 0) */
            //        } PCXHEAD;

            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(imageStream);

            header = new PcxHeader();
            header.manufacturer = binaryReader.ReadByte();
            header.version = binaryReader.ReadByte();
            header.encoding = binaryReader.ReadByte();
            header.bitsPerPixel = binaryReader.ReadByte();
            //window = ReadBytes(imageStream, 8); //Image Dimensions: Xmin,Ymin,Xmax,Ymax
            header.xStart = binaryReader.ReadInt16();
            header.yStart = binaryReader.ReadInt16();
            header.xEnd = binaryReader.ReadInt16();
            header.yEnd = binaryReader.ReadInt16();
            header.width = (header.xEnd - header.xStart) + 1;
            header.height = (header.yEnd - header.yStart) + 1;
            header.horizontalDpi = binaryReader.ReadInt16();
            header.verticalDpi = binaryReader.ReadInt16();
            header.paletteOfImage = binaryReader.ReadBytes(48);
            binaryReader.ReadByte(); // reserver
            header.nPlanes = binaryReader.ReadByte();
            header.bytesPerLine = binaryReader.ReadInt16();
            header.paletteInfo = binaryReader.ReadInt16();
            header.hScreenSize = binaryReader.ReadInt16();
            header.vScreenSize = binaryReader.ReadInt16();

            if (imageStream.CanRead) { 
                 using (MagickImage imageMagick = new MagickImage(imageStream))
                 {
                     imageMagick.ColorType = ColorType.Palette;

                     // Setando Transparencia
                     imageMagick.Alpha(AlphaOption.Set);

                     // Recuperando a Paletta selecionada
                     Palette.Act ob = GameObject.FindGameObjectWithTag("Player1").GetComponent<Palette.Act>();    
                     imageMagick.Format = MagickFormat.Png;
                     int index = 0;
                     Palette.RGBA[] rbgs = ob.rbgs.ToArray();
                     Array.Reverse(rbgs); // inverter a ordem das cores

                     // Aplicando paletta
                     foreach (Palette.RGBA rgba in rbgs)
                     {
                         MagickColor mgColor = new MagickColor(
                             BitConverter.GetBytes(rgba.red)[0],
                             BitConverter.GetBytes(rgba.green)[0],
                             BitConverter.GetBytes(rgba.blue)[0],
                             BitConverter.GetBytes(rgba.alpha)[0]
                         );
                         imageMagick.SetColormap(index++, mgColor);
                     }

                     //Setando transparencia para a primeira cor base
                     imageMagick.SetColormap(0, MagickColors.Transparent);

                     // convertendo a imagem para Textura2D
                     image = new Texture2D(imageMagick.Width, imageMagick.Height, TextureFormat.RGB24, true);
                     image.LoadImage(imageMagick.ToByteArray());

                 }
             }
             

            //pngStream.Close();
        }

        [System.Serializable]
        public class PcxHeader
        {
            public int manufacturer;
            public int version;
            public int encoding;
            public int bitsPerPixel;
            public int xStart;
            public int xEnd;
            public int yStart;
            public int yEnd;
            public int horizontalDpi;
            public int verticalDpi;
            public int[] colorMap;
            public int reserved;
            public int nPlanes;
            public int bytesPerLine;
            public int paletteInfo;
            public byte[] paletteOfImage;
            public int hScreenSize;
            public int vScreenSize;
            public int filler;
            public int width;
            public int height;
            public int sizeHeader = 128;
        }
    }
}
