using MineMake.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MineMake
{
    public class Chunk
    {
        private Dictionary<int, string[,,]> sections = new Dictionary<int, string[,,]>();
        public IReadOnlyDictionary<int, string[,,]> RawSections => sections;
        private int[] biomes;
        public int[] RawBiomes { get => biomes; }
        public int ChunkPosX { get; }
        public int ChunkPosZ { get; }
        internal Chunk(int chunkPosX, int chunkPosZ, Biome defaultBiome)
        {
            ChunkPosX = chunkPosX;
            ChunkPosZ = chunkPosZ;
            biomes = Enumerable.Repeat((int)defaultBiome, 1024).ToArray();
        }

        public void SetBlock(int x, int y, int z, string namespacedName)
        {
            CheckBlockBounds(x, y, z);
            var section = GetSection(y);
            section[x, y % 16, z] = namespacedName;
        }

        public string GetBlock(int x, int y, int z)
        {
            CheckBlockBounds(x, y, z);
            return GetSection(y)[x, y % 16, z];
        }

        public void SetBiome(int x, int y, int z, Biome biome)
        {
            CheckBiomeBounds(x, y, z);
            biomes[y * 16 + z * 4 + x] = (int)biome;
        }

        public Biome GetBiome(int x, int y, int z)
        {
            CheckBiomeBounds(x, y, z);
            return (Biome)biomes[y * 16 + z * 4 + x];
        }

        private void CheckBiomeBounds(int x, int y, int z)
        {
            if (y < 0 || y > 64)
                throw new Exception("Y is outside of thw world");
            if (x < 0 || z < 0 || x > 3 || z > 3)
                throw new Exception("X/Z are outside of the chunk");
        }

        private void CheckBlockBounds(int x, int y, int z)
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
