using System.IO;
using UnityEngine;

namespace MugenForever.IO.PAL
{
    internal interface IPalette
    {
        public const int SIZE = 768;
        public const int CODE_ATTACHED = 12;
        public Color32[] PalleteColor { get; }
        public void Load(Stream pallete);
        public void Load(Stream pallete, int size);

    }
}
