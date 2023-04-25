using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MugenForever.IO.PCX.PCXImageImpl;

namespace MugenForever.IO.PXC
{
    internal interface IPCXImage
    {
        public PCXHeader Header { get; }
        public Texture2D Texture2D { get; }
    }
}
