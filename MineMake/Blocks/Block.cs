using fNbt;
using System.Collections.Generic;

namespace MineMake.Blocks
{
    public class Block
    {
        public NbtCompound TileEntity { get; set; } = null;
        public Dictionary<string, string> Properties { get; set; } = null;
        public string NamespacedName { get; set; } = BlockNames.Air;
    }
}
