using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MineMake
{
    class WorldSaver
    {
        private const int dataVersion = 2730;
        private const int version = 19133;
        private const string versionName = "1.17.1";
        public static void SaveWorld(World world, string path)
        {
            var worldFolderPath = Path.Combine(path, world.WorldName);
            if (Directory.Exists(worldFolderPath))
                Directory.Delete(worldFolderPath, true);
            Directory.CreateDirectory(worldFolderPath);

            var levelDat = CreateLevelDat(world);
            levelDat.SaveToFile(Path.Combine(worldFolderPath, "level.dat"), NbtCompression.GZip);
            world.Icon?.Save(Path.Combine(worldFolderPath, "icon.png"));

            string theNetherDir = Path.Combine(worldFolderPath, "DIM-1");
            string theEndDir = Path.Combine(worldFolderPath, "DIM1");

            string overworldRegion = Path.Combine(worldFolderPath, "region");
            string theNetherRegion = Path.Combine(theNetherDir, "region");
            string theEndRegion = Path.Combine(theEndDir, "region");

            Directory.CreateDirectory(overworldRegion);
            Directory.CreateDirectory(theNetherRegion);
            Directory.CreateDirectory(theEndRegion);

            EncodeDimension(world.Overworld, overworldRegion);
            EncodeDimension(world.TheNether, theNetherRegion);
            EncodeDimension(world.TheEnd, theEndRegion);
        }

        private static NbtFile CreateLevelDat(World world)
        {
            var levelDat = new NbtCompound("Data") {
                new NbtCompound("Version")
                {
                    new NbtInt("Id", dataVersion),
                    new NbtString("Name", versionName),
                    new NbtByte("Snapshot", 0)
                },
                CreateWorldGenSettings(world),
                new NbtByte("allowCommands", world.AllowCheats ? (byte)1 : (byte)0),
                new NbtInt("GameType", (int)world.GameMode),
                new NbtInt("DataVersion", dataVersion),
                new NbtByte("Difficulty", (byte)world.Difficulty),
                new NbtByte("initialized", 1),
                new NbtLong("LastPlayed", DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                new NbtString("LevelName", world.WorldName),
                new NbtFloat("SpawnAngle", 0),
                new NbtInt("SpawnX", world.SpawnX),
                new NbtInt("SpawnY", world.SpawnY),
                new NbtInt("SpawnZ", world.SpawnZ),
                new NbtInt("version", version),
            };
            var file = new NbtFile();
            file.RootTag.Add(levelDat);
            return file;
        }

        private static void EncodeDimension(Dimension dim, string regionPath)
        {
            var regionFiles = SeparateFiles(dim);
            foreach (var file in regionFiles)
            {
                EncodeFile(file.Key.Item1, file.Key.Item2, regionPath, file.Value);
            }
        }

        private static NbtCompound CreateWorldGenSettings(World world)
        {
            return new NbtCompound("WorldGenSettings")
            {
                new NbtCompound("dimensions") {
                    new NbtCompound("minecraft:overworld")
                    {
                        new NbtCompound("generator")
                        {
                            new NbtCompound("biome_source")
                            {
                                new NbtByte("large_biomes", 0),
                                new NbtLong("seed", world.Seed),
                                new NbtString("type", "minecraft:vanilla_layered")
                            },
                            new NbtLong("seed", world.Seed),
                            new NbtString("settings", "minecraft:overworld"),
                            new NbtString("type", "minecraft:noise")
                        },
                        new NbtString("type", "minecraft:overworld")
                    },
                    new NbtCompound("minecraft:the_end")
                    {
                        new NbtCompound("generator")
                        {
                            new NbtCompound("biome_source")
                            {
                                new NbtLong("seed", world.Seed),
                                new NbtString("type", "minecraft:the_end")
                            },
                            new NbtLong("seed", world.Seed),
                            new NbtString("settings", "minecraft:end"),
                            new NbtString("type", "minecraft:noise")
                        },
                        new NbtString("type", "minecraft:the_end")
                    },
                    new NbtCompound("minecraft:the_nether")
                    {
                        new NbtCompound("generator")
                        {
                            new NbtCompound("biome_source")
                            {
                                new NbtLong("seed", world.Seed),
                                new NbtString("preset", "minecraft:nether"),
                                new NbtString("type", "minecraft:multi_noise")
                            },
                            new NbtLong("seed", world.Seed),
                            new NbtString("settings", "minecraft:nether"),
                            new NbtString("type", "minecraft:noise")
                        },
                        new NbtString("type", "minecraft:the_nether")
                    }
                },
                new NbtLong("seed", world.Seed),
                new NbtByte("bonus_chest", 0),
                new NbtByte("generate_features", 1),
            };
        }

        private static Dictionary<(int, int), List<(Chunk, int, int)>> SeparateFiles(Dimension dimension)
        {
            Dictionary<(int, int), List<(Chunk, int, int)>> files = new Dictionary<(int, int), List<(Chunk, int, int)>>();
            foreach (var chunk in dimension.Chunks.Values)
            {
                int fileX = (int)Math.Floor(chunk.ChunkPosX / 32.0);
                int fileZ = (int)Math.Floor(chunk.ChunkPosZ / 32.0);
                int xInFile = MathHelper.Mod(chunk.ChunkPosX, 32);
                int zInFile = MathHelper.Mod(chunk.ChunkPosZ, 32);

                if (!files.ContainsKey((fileX, fileZ)))
                    files.Add((fileX, fileZ), new List<(Chunk, int, int)>());

                files[(fileX, fileZ)].Add((chunk, xInFile, zInFile));
            }
            return files;
        }

        private static void EncodeFile(int fileX, int fileZ, string regionPath, List<(Chunk, int, int)> chunks)
        {
            //Convert chunks to nbt and compress
            //zlib chunk, xInFile, zInfile
            List<(byte[], int, int)> encodedChunks = new List<(byte[], int, int)>();
            foreach (var chunk in chunks)
            {
                byte[] encoded = EncodeChunk(chunk.Item1);
                encodedChunks.Add((encoded, chunk.Item2, chunk.Item3));
            }

            //Figure out where each chunk goes in the file
            int offset = 2;
            //xInFile, zInFile, 4kOffset, 4kSize
            Dictionary<(int, int), (int, int)> chunkLocations = new Dictionary<(int, int), (int, int)>();
            foreach (var encChunk in encodedChunks)
            {
                int size4k = (int)Math.Ceiling((encChunk.Item1.Length + 5) / 4096.0); //+5 bytes for chunk data header
                chunkLocations.Add((encChunk.Item2, encChunk.Item3), (offset, size4k));
                offset += size4k;
            }

            //Write chunks into the file
            using (FileStream file = File.Create(Path.Combine(regionPath, $"r.{fileX}.{fileZ}.mca")))
            {
                byte[] empty = new byte[4096];
                //write chunk locations
                for (int z = 0; z < 32; z++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        if (!chunkLocations.ContainsKey((x, z)))
                        {
                            file.Write(empty, 0, 4);
                            continue;
                        }
                        var (offset4k, size4k) = chunkLocations[(x, z)];
                        uint offsetUint = (uint)offset4k;
                        file.WriteByte((byte)((offsetUint >> 16) & 0xFF));
                        file.WriteByte((byte)((offsetUint >> 8) & 0xFF));
                        file.WriteByte((byte)(offsetUint & 0xFF));
                        file.WriteByte((byte)size4k);
                    }
                }
                //write dummy chunk timestamps
                file.Write(empty, 0, 4096);
                //write the chunks themselves
                foreach (var encChunk in encodedChunks)
                {
                    //reverse to make big-endian
                    file.Write(BitConverter.GetBytes((uint)encChunk.Item1.Length + 1).Reverse().ToArray(), 0, 4);
                    file.WriteByte(2);
                    file.Write(encChunk.Item1, 0, encChunk.Item1.Length);
                    //pad to a multiple of 4k
                    file.Write(empty, 0, 4096 - (encChunk.Item1.Length + 5) % 4096);
                }
            }

        }

        private static byte[] EncodeChunk(Chunk chunk)
        {
            NbtCompound level = new NbtCompound("Level")
            {
                new NbtLong("InhabitedTime", 0),
                new NbtString("Status", "full"),
                new NbtInt("xPos", chunk.ChunkPosX),
                new NbtInt("zPos", chunk.ChunkPosZ),
                new NbtIntArray("Biomes", chunk.RawBiomes),
            };
            level.Add(CreateSectionsNbt(chunk));
            level.Add(CreateTileEntitiesNbt(chunk));
            NbtFile file = new NbtFile();
            file.RootTag.Add(level);
            file.RootTag.Add(new NbtInt("DataVersion", 2730));
            var uncompressed = file.SaveToBuffer(NbtCompression.ZLib);
            return uncompressed;
        }

        private static NbtList CreateTileEntitiesNbt(Chunk chunk)
        {
            NbtList tileEntities = new NbtList("TileEntities", NbtTagType.Compound);
            foreach (var tileEntity in chunk.RawTileEntities)
            {
                var (x, y, z) = tileEntity.Key;
                var nbt = new NbtCompound(tileEntity.Value);
                nbt.Add(new NbtString("id", chunk.GetBlock(x, y, z)));
                nbt.Add(new NbtByte("keepPacked", 0));
                int gX = chunk.ChunkPosX * 16 + x;
                int gZ = chunk.ChunkPosZ * 16 + z;
                nbt.Add(new NbtInt("x", gX));
                nbt.Add(new NbtInt("y", y));
                nbt.Add(new NbtInt("z", gZ));
                tileEntities.Add(nbt);
            }
            return tileEntities;
        }

        private static NbtList CreateSectionsNbt(Chunk chunk)
        {
            var sections = new NbtList("Sections", NbtTagType.Compound);
            foreach (var section in chunk.RawSections)
            {
                //Сreate the palette
                //regular
                int index = 0;
                Dictionary<string, int> regularPalette = new Dictionary<string, int>();
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            var block = section.Value[x, y, z];
                            if (regularPalette.ContainsKey(block))
                                continue;
                            if (chunk.RawBlockProperties.ContainsKey((x, y + section.Key * 16, z)))
                                continue;
                            regularPalette[block] = index;
                            index++;
                        }
                    }
                }

                //with properties
                Dictionary<int, IReadOnlyDictionary<string, string>> propertiesPalette = new Dictionary<int, IReadOnlyDictionary<string, string>>();
                Dictionary<string, List<int>> propertiesIdsPalette = new Dictionary<string, List<int>>();
                foreach (var block in chunk.RawBlockProperties)
                {
                    var (x, y, z) = block.Key;
                    if (y / 16 != section.Key)
                        continue;
                    foreach (var entry in propertiesPalette)
                    {
                        if (ContainerHelper.ArePropertiesEqual(entry.Value, block.Value))
                            continue;
                    }
                    propertiesPalette[index] = block.Value;
                    string blockName = chunk.GetBlock(x, y, z);
                    if (!propertiesIdsPalette.ContainsKey(blockName))
                        propertiesIdsPalette[blockName] = new List<int>();
                    propertiesIdsPalette[blockName].Add(index);
                    index++;
                }

                //Encode blocks with palette
                List<int> orderedPalettedBlocks = new List<int>();
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        for (int x = 15; x >= 0; x--)
                        {
                            if (chunk.RawBlockProperties.ContainsKey((x, y + section.Key * 16, z)))
                            {
                                List<int> potentialIndices = propertiesIdsPalette[section.Value[x, y, z]];
                                foreach (int potIndex in potentialIndices)
                                {
                                    if (ContainerHelper.ArePropertiesEqual(propertiesPalette[potIndex], chunk.RawBlockProperties[(x, y + section.Key * 16, z)]))
                                    {
                                        orderedPalettedBlocks.Add(potIndex);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                orderedPalettedBlocks.Add(regularPalette[section.Value[x, y, z]]);
                            }
                        }
                    }
                }

                //Fit the encoded value into longs
                int bitsPerBlock = (int)Math.Max(4, Math.Ceiling(Math.Log(2, Math.Max(regularPalette.Count, 4))));
                int blocksPerLong = 64 / bitsPerBlock;

                int curBlockIndex = 0;
                List<long> longs = new List<long>();
                while (true)
                {
                    long value = 0;
                    for (int i = 0; i < blocksPerLong; i++)
                    {
                        value <<= bitsPerBlock;
                        value |= (uint)orderedPalettedBlocks[curBlockIndex];
                        curBlockIndex++;
                    }
                    longs.Add(value);
                    if (curBlockIndex >= 16 * 16 * 16)
                        break;
                }

                //Encode palette as nbt
                var nbtPalette = new NbtList("Palette", NbtTagType.Compound);
                foreach (var block in regularPalette)
                {
                    nbtPalette.Add(new NbtCompound()
                    {
                        new NbtString("Name", block.Key)
                    });
                }
                foreach (var blockIds in propertiesIdsPalette)
                {
                    foreach (int id in blockIds.Value)
                    {
                        var entry = new NbtCompound()
                        {
                            new NbtString("Name", blockIds.Key)
                        };
                        var props = new NbtCompound("Properties");
                        foreach (var pair in propertiesPalette[id])
                        {
                            props.Add(new NbtString(pair.Key, pair.Value));
                        }
                        entry.Add(props);
                        nbtPalette.Add(entry);
                    }
                }

                //Create nbt
                var compound = new NbtCompound()
                {
                    new NbtByte("Y", (byte)section.Key),
                    nbtPalette,
                    new NbtLongArray("BlockStates", longs.ToArray())
                };
                sections.Add(compound);
            }
            return sections;
        }
    }
}
