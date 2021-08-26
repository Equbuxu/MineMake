using MineMake.Enums;
using System.Drawing;

namespace MineMake
{
    public class World
    {
        public string WorldName { get; set; } = "New World";
        public GameMode GameMode { get; set; } = GameMode.Creative;
        public Difficulty Difficulty { get; set; } = Difficulty.Normal;
        public bool AllowCheats { get; set; } = true;
        public long Seed = 0;
        public int SpawnX { get; set; } = 0;
        public int SpawnY { get; set; } = 80;
        public int SpawnZ { get; set; } = 0;
        public Bitmap Icon { get; set; } = null;

        public Dimension Overworld { get; } = new Dimension(Biome.Plains);
        public Dimension TheNether { get; } = new Dimension(Biome.NetherWastes);
        public Dimension TheEnd { get; } = new Dimension(Biome.TheEnd);

        public void Save(string savesFolderPath)
        {
            WorldSaver.SaveWorld(this, savesFolderPath);
        }
    }
}
