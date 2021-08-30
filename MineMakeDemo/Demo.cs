using MineMake;
using MineMake.Blocks;
using MineMake.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace MineMakeDemo
{
    public class Demo
    {
        public Demo()
        {
            World world = new World()
            {
                WorldName = "MineMake Demo World",
                GameMode = GameMode.Creative,
                Difficulty = Difficulty.Normal,
                AllowCheats = true,
                Seed = 0,
                SpawnY = 80,
            };

            var xLog = BlockFactory.CreateAxisRotatedBlock(BlockNames.OakLog, AxisRotation.x);
            var zLog = BlockFactory.CreateAxisRotatedBlock(BlockNames.OakLog, AxisRotation.z);
            var yLog = BlockFactory.CreateAxisRotatedBlock(BlockNames.OakLog, AxisRotation.y);

            var chest = BlockFactory.CreateChest(BlockNames.Chest, FacingRotation.NorthNegZ, new Dictionary<byte, (string, byte)>()
            {
                [10] = ("minecraft:iron_pickaxe", 1),
                [13] = ("minecraft:bedrock", 64),
                [16] = ("minecraft:snowball", 16)
            });

            world.Overworld.SetBlock(24, 60, 8, xLog);
            world.Overworld.SetBlock(-8, 60, 8, xLog);
            world.Overworld.SetBlock(8, 60, 24, zLog);
            world.Overworld.SetBlock(8, 60, -8, zLog);
            world.Overworld.SetBlock(8, 55, 8, yLog);
            world.Overworld.SetBlock(8, 60, 8, chest);

            world.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @".minecraft\saves"));
        }
    }
}
