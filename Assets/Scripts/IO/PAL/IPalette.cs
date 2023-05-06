using System.IO;
using UnityEngine;

namespace MugenForever.IO.PAL
{
    internal interface IPalette
    {
        public static int SIZE = 768;
        public static int CODE_ATTACHED = 12;
        public Color32[] PalleteColor { get; }
        public void Load(Stream pallete);
    }
}
