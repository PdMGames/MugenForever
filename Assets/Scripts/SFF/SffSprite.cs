using UnityEngine;

namespace MugenForever.Sff
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
    [System.Serializable]
    public class SffSprite
    {
        /// <summary>
        /// Name of Sprite
        /// </summary>
        public string name;

        /// <summary>
        /// Index position
        /// </summary>
        public long index;

        /// <summary>
        /// Next image offset
        /// </summary>
        public int nextFileOffset;

        /// <summary>
        /// size of image: 0 is linked
        /// </summary>
        public int subfileLength;

        /// <summary>
        /// Position X
        /// </summary>
        public int axisX;

        /// <summary>
        /// Position Y
        /// </summary>
        public int axisY;

        /// <summary>
        /// Number of group
        /// </summary>
        public int groupNumber;

        /// <summary>
        /// Number of image
        /// </summary>
        public int imageNumber;

        /// <summary>
        /// Position of image linked sprite only
        /// </summary>
        public int indexPreviousLinked;

        /// <summary>
        /// Same palette of image previous
        /// </summary>
        public bool samePaletteOfPreviousImage;

        /// <summary>
        /// Comments
        /// </summary>
        public string comments;
        
        /// <summary>
        /// Image raw
        /// </summary>
        public Texture2D image;

        /// <summary>
        /// Sprite source
        /// </summary>
        public Sprite sprite;

        public Pcx pcx = new Pcx();

        // version sffv2 <<

        /// <summary>
        /// Width of image
        /// </summary>
        public int width;

        /// <summary>
        /// Height of image
        /// </summary>
        public int height;

        /// <summary>
        /// ????
        /// </summary>
        public int coldepth;

        /// <summary>
        /// Format
        /// 0 raw
        /// 1 invalid (no use)
        /// 2 RLE8
        /// 3 RLE5
        /// 4 LZ5
        /// </summary>
        public int fmt;

        /// <summary>
        ///  offset ldata or tdata
        /// </summary>
        public int offsetData;

        /// <summary>
        /// Index of palette
        /// </summary>
        public int paletteIndex;

        /// <summary>
        /// 0    unset: literal (use ldata); set: translate (use tdata; decompress on load)
        /// 1-15 unused
        /// </summary>
        public int flag;

        // >> version sffv2

    }
}
