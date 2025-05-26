using UnityEngine;
using System.Collections.Generic;

namespace MugenForever.AIR
{
    public class AirData
    {
        public Dictionary<int, AirAnimation> Animations { get; private set; } // ActionNumber -> AirAnimation

        public AirData()
        {
            Animations = new Dictionary<int, AirAnimation>();
        }
    }
}
