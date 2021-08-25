using MineMake.Enums;
using System.Collections.Generic;
using System.Drawing;

namespace MineMake
{
    public class World
    {
        private ChunkStorage storage = new ChunkStorage();
        public IReadOnlyDictionary<(int, int), Chunk> Chunks => storage.RawChunks;
        public string WorldName { get; set; } = "New World";
        public GameMode GameMode { get; set; } = GameMode.Creative;
        public Difficulty Difficulty { get; set; } = Difficulty.Normal;
        public bool AllowCheats { get; set; } = true;
        public long Seed = 0;
        public int SpawnX { get; set; } = 0;
        public int SpawnY { get; set; } = 80;
        public int SpawnZ { get; set; } = 0;
        public Bitmap Icon { get; set; } = null;

        public void ReserveChunk(int x, int z)
        {
            storage.GetChunk(x, z);
        }

        public void SetBlock(int x, int y, int z, string namespacedName)
        {
            var (chunkX, chunkY, chunkZ) = ChunkStorage.GetChunkCoordinates(x, y, z);
            var chunk = storage.GetChunk(chunkX, chunkZ);
            var (inX, inY, inZ) = Chunk.GetCoordsInChunk(x, y, z);
            chunk.SetBlock(inX, inY, inZ, namespacedName);
        }

        public string GetBlock(int x, int y, int z)
        {
            var (chunkX, chunkY, chunkZ) = ChunkStorage.GetChunkCoordinates(x, y, z);
            var chunk = storage.MaybeGetChunk(chunkX, chunkZ);
            if (chunk == null)
                return Blocks.Air;
            var (inX, inY, inZ) = Chunk.GetCoordsInChunk(x, y, z);
            return chunk.GetBlock(inX, inY, inZ);
        }

        public void Save(string savesFolderPath)
        {
            WorldSaver.SaveWorld(this, savesFolderPath);
        }
    }
}
