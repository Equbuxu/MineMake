using fNbt;
using MineMake.Enums;
using System.Collections.Generic;

namespace MineMake.Blocks
{
    public static class BlockFactory
    {
        /// <summary>
        /// Logs, Pillars, 
        /// </summary>
        public static Block CreateAxisRotatedBlock(string namespacedName, AxisRotation axis)
        {
            return new Block()
            {
                NamespacedName = namespacedName,
                Properties = new Dictionary<string, string>()
                {
                    ["axis"] = AxisToString(axis),
                }
            };
        }

        /// <summary>
        /// Chest, Trapped Chest
        /// </summary>
        /// <param name="namespacedName">Container namespaced name</param>
        /// <param name="items">slot, (namespaced name, count)</param>
        /// <returns></returns>
        public static Block CreateChest(string namespacedName, FacingRotation facing, Dictionary<byte, (string, byte)> items)
        {
            NbtList itemList = new NbtList("Items");
            foreach (var item in items)
            {
                NbtCompound comp = new NbtCompound()
                {
                    new NbtByte("Count", item.Value.Item2),
                    new NbtByte("Slot", item.Key),
                    new NbtString("id", item.Value.Item1)
                };
                itemList.Add(comp);
            }
            return new Block()
            {
                NamespacedName = namespacedName,
                Properties = new Dictionary<string, string>()
                {
                    ["facing"] = FacingToString(facing),
                },
                TileEntity = new NbtCompound()
                {
                    itemList,
                },
            };
        }

        private static string AxisToString(AxisRotation axis)
        {
            switch (axis)
            {
                default:
                case AxisRotation.x: return "x";
                case AxisRotation.y: return "y";
                case AxisRotation.z: return "z";
            }
        }

        private static string FacingToString(FacingRotation facing)
        {
            switch (facing)
            {
                default:
                case FacingRotation.EastPosX: return "east";
                case FacingRotation.WestNegX: return "west";
                case FacingRotation.SouthPosZ: return "south";
                case FacingRotation.NorthNegZ: return "north";
                case FacingRotation.Up: return "up";
                case FacingRotation.Down: return "down";
            }
        }
    }
}
