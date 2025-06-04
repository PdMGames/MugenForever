using UnityEngine;

namespace MugenForever.AIR
{
    public class AirFrame
    {
        public int GroupNumber { get; set; }
        public int ImageNumber { get; set; }
        public int Duration { get; set; } // Em game ticks
        public bool HorizontalFlip { get; set; }
        public bool VerticalFlip { get; set; }
        // Adicionaremos caixas de colisão e outros dados depois
        // public Rect[] CollisionBoxes { get; set; }
    }
}
