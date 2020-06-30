using System;

namespace GameCore
{
    [Serializable]
    public struct GridRect
    {
        public int x;
        public int z;
        public GridPos size;

        public int width => size.x;
        public int height => size.z;
        public GridPos center => new GridPos(x + width / 2, z + height / 2);
        public int x_min => x;
        public int x_max => x + width - 1;
        public int z_min => z;
        public int z_max => z + height - 1;

        public GridRect(int x, int z, int width, int height)
        {
            this.x = x;
            this.z = z;
            size = new GridPos(width, height);
        }
    }
}