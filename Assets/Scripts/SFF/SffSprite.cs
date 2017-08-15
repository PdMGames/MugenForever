using System;
using System.Collections.Generic;
using System.IO;
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
        public string name;
        public int nextFileOffset;
        public int subfileLength;
        public int axisX;
        public int axisY;
        public int groupNumber;
        public int imageNumber;
        public int indexPreviousLinked;
        public bool samePaleteOfPreviousImage;
        public string comments;
        public Sprite sprite;
    }
}
