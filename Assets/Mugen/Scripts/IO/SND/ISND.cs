using System.Collections.Generic;
using UnityEngine;

namespace MugenForever.IO.SND
{
    internal interface ISND
    {
        Dictionary<int, Dictionary<int, AudioClip>> Sounds { get; }
    }
}
