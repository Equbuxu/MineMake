using System;
using System.Collections.Generic;

namespace MineMake
{
    public class Chunk
    {
        private Dictionary<int, string[,,]> sections = new Dictionary<int, string[,,]>();
        public IReadOnlyDictionary<int, string[,,]> RawSections => sections;

        public int ChunkPosX { get; }
        public int ChunkPosZ { get; }
        internal Chunk(int chunkPosX, int chunkPosZ)
        {
            ChunkPosX = chunkPosX;
            ChunkPosZ = chunkPosZ;
        }

        public void SetBlock(int x, int y, int z, string namespacedName)
        {
            CheckBounds(x, y, z);
            var section = GetSection(y);
            section[x, y % 16, z] = namespacedName;
        }

        public string GetBlock(int x, int y, int z)
        {
            CheckBounds(x, y, z);
            return GetSection(y)[x, y % 16, z];
        }

        private void CheckBounds(int x, int y, int z)
        {
            if (y < 0 || y > 255)
                throw new Exception("Y is outside of the world");
            if (x < 0 || z < 0 || x >= 16 || z >= 16)
                throw new Exception("X/Z are ouside the chunk");
        }

        private string[,,] GetSection(int blockY)
        {
            int sectionY = blockY / 16;
            if (!sections.ContainsKey(sectionY))
                sections[sectionY] = CreateSection();
            return sections[sectionY];
        }

        private string[,,] CreateSection()
        {
            var section = new string[16, 16, 16];
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 16; k++)
                    {
                        section[i, j, k] = Blocks.Air;
                    }
                }
            }
            return section;
        }

        public static (int, int, int) GetCoordsInChunk(int x, int y, int z) => (MathHelper.Mod(x, 16), y, MathHelper.Mod(z, 16));
    }
}
