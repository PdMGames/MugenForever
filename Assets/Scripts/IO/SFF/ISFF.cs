using Assets.Scripts.IO.SFF;
using System.Collections.Generic;

namespace MugenForever.IO.SFF
{

    internal interface ISFF
    {
        public string Version { get; }
        public Dictionary<int, Dictionary<int, SFFSprite>> Spriters { get; }
    }
}
