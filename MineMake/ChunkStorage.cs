using MineMake.Enums;
using System;
using System.Collections.Generic;

namespace MineMake
{
    internal class ChunkStorage
    {
        internal Dictionary<(int, int), Chunk> RawChunks { get; } = new Dictionary<(int, int), Chunk>();
        private Biome defaultBiome;

        internal ChunkStorage(Biome defaultBiome)
        {
            this.defaultBiome = defaultBiome;
        }

        internal Chunk GetChunkForBlock(int x, int y, int z)
        {
            var (chunkX, chunkZ) = GetChunkCoordinates(x, z);
            return GetChunk(chunkX, chunkZ);
        }

        internal Chunk MaybeGetChunkForBlock(int x, int y, int z)
        {
            var (chunkX, chunkZ) = GetChunkCoordinates(x, z);
            return MaybeGetChunk(chunkX, chunkZ);
        }

        internal Chunk GetChunk(int x, int z)
        {
            if (!RawChunks.ContainsKey((x, z)))
                RawChunks[(x, z)] = new Chunk(x, z, defaultBiome);
            return RawChunks[(x, z)];
        }

        internal Chunk MaybeGetChunk(int x, int z)
        {
            if (!RawChunks.ContainsKey((x, z)))
                return null;
            return RawChunks[(x, z)];
        }

        private static (int, int) GetChunkCoordinates(int x, int z) => ((int)Math.Floor(x / 16.0), (int)Math.Floor(z / 16.0));
    }
}
