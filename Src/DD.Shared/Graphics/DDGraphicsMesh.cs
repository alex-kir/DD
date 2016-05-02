using System;
using System.Linq;

namespace DD.Graphics
{
    public class DDGraphicsMesh
    {
        public DDTexture Texture;
        internal readonly int vertextCount;
        internal readonly int positionSize;
        internal readonly float [] positions;
        internal readonly int uvSize = 2;
        internal readonly float [] uvs;

        internal readonly float [] colors_white;
        internal readonly float [] colors_black;

        internal readonly ushort [] indices;

        public DDGraphicsMesh(int positionSize, float [] positions, float [] uvs, DDColor white, DDColor black)
        {
            vertextCount = positions.Length / positionSize;

            this.positionSize = positionSize;
            this.positions = positions;
            this.uvs = uvs;
            this.colors_white = new float[vertextCount * 4];
            this.colors_black = new float[vertextCount * 4];
            this.indices = new ushort[vertextCount];

            for (int i = 0; i < vertextCount; i++)
            {
                this.colors_white[i * 4 + 0] = white.R;
                this.colors_white[i * 4 + 1] = white.G;
                this.colors_white[i * 4 + 2] = white.B;
                this.colors_white[i * 4 + 3] = white.A;

                this.colors_black[i * 4 + 0] = black.R;
                this.colors_black[i * 4 + 1] = black.G;
                this.colors_black[i * 4 + 2] = black.B;
                this.colors_black[i * 4 + 3] = black.A;

                this.indices[i] = (ushort)i;
            }
        }

        public void SetWhiteColor(DDColor white)
        {
            for (int i = 0; i < vertextCount; i++)
            {
                this.colors_white[i * 4 + 0] = white.R;
                this.colors_white[i * 4 + 1] = white.G;
                this.colors_white[i * 4 + 2] = white.B;
                this.colors_white[i * 4 + 3] = white.A;
            }
        }

        public void SetBlackColor(DDColor black)
        {
            for (int i = 0; i < vertextCount; i++)
            {
                this.colors_black[i * 4 + 0] = black.R;
                this.colors_black[i * 4 + 1] = black.G;
                this.colors_black[i * 4 + 2] = black.B;
                this.colors_black[i * 4 + 3] = black.A;
            }
        }
    }
}

