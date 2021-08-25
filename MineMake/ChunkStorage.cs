using System;
using System.Collections.Generic;

namespace MineMake
{
    internal class ChunkStorage
    {
        internal Dictionary<(int, int), Chunk> RawChunks { get; } = new Dictionary<(int, int), Chunk>();

        internal Chunk GetChunk(int x, int z)
        {
            if (!RawChunks.ContainsKey((x, z)))
                RawChunks[(x, z)] = new Chunk(x, z);
            return RawChunks[(x, z)];
        }

        internal Chunk MaybeGetChunk(int x, int z)
        {
            if (!RawChunks.ContainsKey((x, z)))
                return null;
            return RawChunks[(x, z)];
        }

        internal static (int, int, int) GetChunkCoordinates(int x, int y, int z) => ((int)Math.Floor(x / 16.0), (int)Math.Floor(y / 16.0), (int)Math.Floor(z / 16.0));
    }
}
