using MineMake.Enums;
using System.Collections.Generic;

namespace MineMake
{
    public class Dimension
    {
        private ChunkStorage storage;
        public IReadOnlyDictionary<(int, int), Chunk> Chunks => storage.RawChunks;

        internal Dimension(Biome defaultBiome)
        {
            storage = new ChunkStorage(defaultBiome);
        }

        public void SetBlock(int x, int y, int z, string namespacedName)
        {
            var chunk = storage.GetChunkForBlock(x, y, z);
            var (inX, inY, inZ) = Chunk.GetCoordsInChunk(x, y, z);
            chunk.SetBlock(inX, inY, inZ, namespacedName);
        }

        public string GetBlock(int x, int y, int z)
        {
            var chunk = storage.MaybeGetChunkForBlock(x, y, z);
            if (chunk == null)
                return Blocks.Air;
            var (inX, inY, inZ) = Chunk.GetCoordsInChunk(x, y, z);
            return chunk.GetBlock(inX, inY, inZ);
        }

        /// <summary>
        /// Creates an empty chunk at the specified chunk coordinates
        /// </summary>
        public void ReserveChunk(int x, int z)
        {
            storage.GetChunk(x, z);
        }

        /// <summary>
        /// Sets a biome in a 4x4x4 cube located at the specified (block) coordinates
        /// Minecraft seems to ignore 3D biomes in the overworld by using only the bottommost 4x4 layer
        /// </summary>
        public void SetBiome(int x, int y, int z, Biome biome)
        {
            var chunk = storage.GetChunkForBlock(x, y, z);
            var (inX, inY, inZ) = Chunk.GetCoordsInChunk(x, y, z);
            chunk.SetBiome(inX / 4, inY / 4, inZ / 4, biome);
        }

        /// <summary>
        /// Get biome from a 4x4x4 cube located at the specified (block) coordinates.
        /// </summary>
        public Biome GetBiome(int x, int y, int z)
        {
            var chunk = storage.GetChunkForBlock(x, y, z);
            if (chunk is null)
                return Biome.Ocean;
            var (inX, inY, inZ) = Chunk.GetCoordsInChunk(x, y, z);
            return chunk.GetBiome(inX / 4, inY / 4, inZ / 4);
        }
    }
}
