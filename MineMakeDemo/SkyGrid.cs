using MineMake;
using MineMake.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MineMakeDemo
{
    class SkyGrid
    {
        private static Random rnd = new Random();
        private static List<string> survivalBlocks = new List<string>()
        {
            "minecraft:stone",
            /*"minecraft:oak_wood",
            "minecraft:birch_wood",
            "minecraft:white_wool",
            "minecraft:lava",
            "minecraft:water",
            "minecraft:melon",
            "minecraft:pumpkin"*/
        };

        private static List<string> junkBlocks = new List<string>()
        {
            /*"minecraft:deepslate",
            "minecraft:granite",
            "minecraft:diorite",
            "minecraft:andesite",
            "minecraft:dirt",
            "minecraft:sand",
            "minecraft:red_sand",
            "minecraft:gravel",
            "minecraft:grass_block",
            "minecraft:cobblestone",
            "minecraft:cobbled_deepslate",
            "minecraft:sandstone",
            "minecraft:obsidian",
            "minecraft:bookshelf",*/
            "minecraft:tnt",
        };

        private static List<string> oreBlocks = new List<string>()
        {
            /*"minecraft:coal_ore",
            "minecraft:iron_ore",
            "minecraft:copper_ore",
            "minecraft:gold_ore",
            "minecraft:redstone_ore",
            "minecraft:emerald_ore",
            "minecraft:lapis_ore",
            "minecraft:diamond_ore",*/
            "minecraft:ancient_debris",
        };


        public static void Main()
        {
            World world = new World()
            {
                WorldName = "SkyGrid 1.17.1",
                GameMode = GameMode.Survival,
                Difficulty = Difficulty.Normal,
                AllowCheats = true,
                Seed = 0,
                SpawnY = 64,
                Icon = new Bitmap(64, 64)
            };
            using (Graphics gr = Graphics.FromImage(world.Icon))
            {
                gr.Clear(Color.Blue);
                gr.FillRectangle(Brushes.Gray, 16, 16, 32, 32);
            }

            CreateSkyGrid(world.Overworld);
            CreateSkyGrid(world.TheNether);
            CreateSkyGrid(world.TheEnd);
            //world.SetBiome(0, 0, 0, Biome.SnowyTaiga);
            //world.SetBiome(15, 0, 0, Biome.Badlands);

            world.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @".minecraft\saves"));
        }

        private static void CreateSkyGrid(Dimension dimension)
        {
            const int radiusChunks = 4;
            const int radiusEmptyChunks = 4;
            const int totalRadius = radiusChunks + radiusEmptyChunks;

            for (int i = -totalRadius; i <= totalRadius; i++)
            {
                for (int j = -totalRadius; j <= totalRadius; j++)
                {
                    dimension.ReserveChunk(i, j);
                }
            }

            const int radiusSkygrid = radiusChunks * 16;
            for (int i = -radiusSkygrid; i <= radiusSkygrid; i += 4)
            {
                for (int j = -radiusSkygrid; j <= radiusSkygrid; j += 4)
                {
                    for (int k = 0; k < 256; k += 4)
                    {
                        var block = GetRandomBlock();
                        dimension.SetBlock(i, k, j, block);
                    }
                }
            }
        }

        private static string GetRandomBlock()
        {
            double category = rnd.NextDouble();
            if (category < 0.2)
                return oreBlocks[rnd.Next(oreBlocks.Count)];
            else if (category < 0.7)
                return junkBlocks[rnd.Next(junkBlocks.Count)];
            else
                return survivalBlocks[rnd.Next(survivalBlocks.Count)];
        }
    }
}
