using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MugenForever.IO.PXC
{
    internal interface IPalette
    {
        public Color32[] PalleteColor { get; }

        public void Load(Stream pallete);
    }
}
