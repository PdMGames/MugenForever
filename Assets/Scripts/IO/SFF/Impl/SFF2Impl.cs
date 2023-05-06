using Assets.Scripts.IO.SFF;
using MugenForever.IO.PAL;
using MugenForever.IO.PCX;
using System;
using System.Collections.Generic;
using System.IO;

namespace MugenForever.IO.SFF
{
    internal class SFF2Impl : ISFF, ISFF2
    {
        public SFF2Impl(Stream data) {
            
        }

        public class SFFHeader
        {

        }

        public string Version => throw new NotImplementedException();
        public Dictionary<int, Dictionary<int, SFFSprite>> Spriters => throw new NotImplementedException();
        public Dictionary<int, Dictionary<int, IPalette>> Palettes => throw new NotImplementedException();

    }
}
