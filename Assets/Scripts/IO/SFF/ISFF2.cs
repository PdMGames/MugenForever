using MugenForever.IO.PAL;
using System.Collections.Generic;

namespace MugenForever.IO.SFF
{
    internal interface ISFF2 : ISFF
    {
        public Dictionary<int, Dictionary<int, IPalette>> Palettes { get; }
    }
}
