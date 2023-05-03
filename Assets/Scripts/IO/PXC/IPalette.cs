using System.IO;
using UnityEngine;

namespace MugenForever.IO.PXC
{
    internal interface IPalette
    {
        public Color32[] PalleteColor { get; }

        public void Load(Stream pallete);
    }
}
