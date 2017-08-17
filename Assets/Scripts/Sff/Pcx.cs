using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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
    public class Pcx : MugenForever.Reader.Binary
    {
        public int manufacturer;
        public int version;
        public int encoding;
        public int bitsPerPixel;
        public int window;
        public int horizontalDpi;
        public int verticalDpi;
        public int[] colorMap;
        public int reserved;
        public int nPlanes;
        public int bytesPerLine;
        public int paletteInfo;
        public int hScreenSize;
        public int vScreenSize;
        public int filler;

        [ContextMenu("Load From File")]
        public void LoadInEditor()
        {
            string file = EditorUtility.OpenFilePanel("Select PCX image", "", "pcx");

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

            /*
             12         HDpi          2     Horizontal Resolution of image in DPI* 
14         VDpi          2     Vertical Resolution of image in DPI* 
16         Colormap     48     Color palette setting, see text 
64         Reserved      1     Should be set to 0. 
65         NPlanes       1     Number of color planes 
66         BytesPerLine  2     Number of bytes to allocate for a scanline plane.  MUST be an EVEN number.  Do NOT calculate from Xmax-Xmin. 
68         PaletteInfo   2     How to interpret palette- 1 = Color/BW, 2 = Grayscale (ignored in PB IV/ IV +) 
70         HscreenSize   2     Horizontal screen size in pixels. New field found only in PB IV/IV Plus 
72         VscreenSize   2     Vertical screen size in pixels. New field found only in PB IV/IV Plus 
74         Filler       54     Blank to fill out 128 byte header.  Set all bytes to 0 */

            manufacturer = ReadInt(fileStream, 1);
            version = ReadInt(fileStream, 1);
            encoding = ReadInt(fileStream,1);
            bitsPerPixel = ReadInt(fileStream, 1);
            window = ReadInt(fileStream, 8); //Image Dimensions: Xmin,Ymin,Xmax,Ymax */
            horizontalDpi = ReadInt(fileStream, 2);
            verticalDpi = ReadInt(fileStream, 2);
            // 16 bit colormap, pallete
            ReadJump(fileStream, 48);
            reserved = ReadInt(fileStream, 1);
            nPlanes = ReadInt(fileStream, 1);
            bytesPerLine = ReadInt(fileStream, 2);
            paletteInfo = ReadInt(fileStream, 2);
            hScreenSize = ReadInt(fileStream, 2);
            vScreenSize = ReadInt(fileStream, 2);
            filler = ReadInt(fileStream, 54);
        }
    }
}
