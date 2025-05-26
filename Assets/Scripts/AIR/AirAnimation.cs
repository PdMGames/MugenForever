using UnityEngine;
using System.Collections.Generic;

namespace MugenForever.AIR
{
    public class AirAnimation
    {
        public int ActionNumber { get; set; }
        public List<AirFrame> Frames { get; private set; }

        public AirAnimation(int actionNumber)
        {
            ActionNumber = actionNumber;
            Frames = new List<AirFrame>();
        }
    }
}
