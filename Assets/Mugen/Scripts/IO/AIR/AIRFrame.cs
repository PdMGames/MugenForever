namespace MugenForever.IO.AIR
{
    internal class AIRFrame
    {
        public int Group;
        public int Index;
        public int AxisX;
        public int AxisY;
        public float Time;
        public FlipType Flip;
        public bool StartLoop;
        public BoxCollision[] BoxCollisionDefaults;
        public BoxCollision[] BoxAttackDefaults;
        public BoxCollision[] BoxCollisions;
        public BoxCollision[] BoxAttacks;

        public enum FlipType
        {
            NONE,
            H,
            V,
            HV
        }

        public class BoxCollision
        {
            public int Width;
            public int Height;
            public int AxisX;
            public int AxisY;
        }
    }
}
