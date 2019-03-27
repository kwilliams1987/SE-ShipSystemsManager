using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using IngameScript.Mockups;
using IngameScript.Mockups.Asserts;
using IngameScript.Mockups.Blocks;
using Malware.MDKUtilities;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;

namespace IngameScript.MDK
{
    public class TestBootstrapper
    {
        static class Styler
        {
            public static T Get<T>(String key) => (T)Program.BaseStyler.DefaultStyles[key];
        }
        
        // All the files in this folder, as well as all files containing the file ".debug.", will be excluded
        // from the build process. You can use this to create utilites for testing your scripts directly in 
        // Visual Studio.

        static TestBootstrapper()
        {
            // Initialize the MDK utility framework
            MDKUtilityFramework.Load();
        }

        public static void Main()
        {
            // In order for your program to actually run, you will need to provide a mockup of all the facilities 
            // your script uses from the game, since they're not available outside of the game.

            var nextEntityId = 1L;
            var grid = new MockGridTerminalSystem();
            var tickrate = UpdateType.Update100;

            var programmableBlock = new MockProgrammableBlock()
            {
                CustomName = "Programmable Block [SSM]"
            };

            grid.Add(new MockTextPanel()
            {
                EntityId = nextEntityId++,
                CustomName = "Debug Panel",
                CustomData = "[Ship Systems Manager]\nfunctions=debug lcd"
            });

            var zones = new List<Zone>();
            var airlock = new MockDoor()
            {
                EntityId = nextEntityId++,
                CustomName = "Zone 1/2 Airlock",
                CustomData = "[Ship Systems Manager]\nzones=\n|zone-1\n|zone-2\nfunctions=airlock"
            };

            airlock.OpenDoor();

            {
                var room = new Zone("zone-1");
                room.AddBlock(airlock);

                room.AddBlock(new MockAirVent()
                {
                    EntityId = nextEntityId++,
                    Status = VentStatus.Pressurized,
                    CustomName = "Air Vent (Zone 1)",
                    ShowInTerminal = false,
                    IsDepressurizing = false,
                    CanPressurize = true
                });

                room.AddBlock(new MockInteriorLight()
                {
                    EntityId = nextEntityId++,
                    CustomName = "Alert Light (Zone 1)",
                    ShowInTerminal = false,
                    CustomData = "[Ship Systems Manager]\nzones=zone-1\nfunctions=warnlight"
                });

                room.AddBlock(new MockInteriorLight()
                {
                    EntityId = nextEntityId++,
                    CustomName = "Normal Light (Zone 1)",
                    ShowInTerminal = false,
                });

                room.AddBlock(new MockTextPanel()
                {
                    EntityId = nextEntityId++,
                    CustomName = "Door Sign (Zone 1)",
                    CustomData = "[Ship Systems Manager]\nzones=zone-1\nfunctions=doorsign"
                });

                room.OfType<IMyTextPanel>().First(b => b.CustomName == "Door Sign (Zone 1)")
                    .WritePublicText("ZONE 1");

                var motorSubgrid = new MockCubeGrid();
                var motorhead = new MockMotorRotor()
                {
                    EntityId = nextEntityId++,
                    CubeGrid = motorSubgrid
                };

                grid.Add(new MockInteriorLight()
                {
                    CubeGrid = motorSubgrid
                });

                room.AddBlock(new MockMotorStator()
                {
                    EntityId = nextEntityId++,
                    CustomName = "Klaxon (Zone 1)",
                    CustomData = "[Ship Systems Manager]\nzones=zone-1\nfunctions=siren",
                    MockPendingAttachment = motorhead 
                });

                zones.Add(room);

                room.AddToGrid(grid);
            }

            {
                var room = new Zone("zone-2");
                room.AddBlock(airlock);

                room.AddBlock(new MockAirVent()
                {
                    EntityId = nextEntityId++,
                    Status = VentStatus.Pressurized,
                    CustomName = "Air Vent (Zone 2)",
                    ShowInTerminal = false,
                    IsDepressurizing = false,
                    CanPressurize = true
                });

                room.AddBlock(new MockInteriorLight()
                {
                    EntityId = nextEntityId++,
                    CustomName = "Alert Light (Zone 2)",
                    ShowInTerminal = false,
                    CustomData = "[Ship Systems Manager]\nzones=zone-2\nfunctions=warnlight"
                });

                room.AddBlock(new MockInteriorLight()
                {
                    EntityId = nextEntityId++,
                    CustomName = "Normal Light (Zone 2)",
                    ShowInTerminal = false,
                });

                room.AddBlock(new MockTextPanel()
                {
                    EntityId = nextEntityId++,
                    CustomName = "Door Sign (Zone 2)",
                    CustomData = "[Ship Systems Manager]\nzones=zone-2\nfunctions=doorsign"
                });

                room.OfType<IMyTextPanel>().First(b => b.CustomName == "Door Sign (Zone 2)")
                    .WritePublicText("ZONE 2");

                zones.Add(room);

                room.AddToGrid(grid);
            }

            var program = MDKFactory.CreateProgram<Program>(new MDKFactory.ProgramConfig()
            {
                Echo = message => Console.WriteLine("[{0}] {1}", DateTime.Now.ToString("U"), message),
                GridTerminalSystem = grid,
                ProgrammableBlock = programmableBlock
            });

            Console.WriteLine("Executing Run #1 (Normal)");
            MDKFactory.Run(program, updateType: tickrate);

            var lights = new List<IMyInteriorLight>();
            grid.GetBlocksOfType(lights);

            foreach (var light in lights)
            {
                Assert.Equals(new Color(255,255,255), light.Color, $"Light {light.EntityId} does not have the expected color.");
                Assert.Equals(0, light.BlinkIntervalSeconds, $"Light {light.EntityId} does not have the expected blink interval.");
                Assert.Equals(0, light.BlinkLength, $"Light {light.EntityId} does not have the expected blink length.");
                Assert.Equals(0, light.BlinkOffset, $"Light {light.EntityId} does not have the expected blink offset.");
            }

            var lcds = new List<IMyTextPanel>();
            grid.GetBlocksOfType(lcds);
            foreach (var lcd in lcds)
            {
                switch (lcd.CustomName)
                {
                    case "Door Sign (Zone 1)":
                        Assert.Equals("ZONE 1", lcd.GetPublicText(), $"LCD {lcd.EntityId} doesn't have the expected text.");
                        break;
                    case "Door Sign (Zone 2)":
                        Assert.Equals("ZONE 2", lcd.GetPublicText(), $"LCD {lcd.EntityId} doesn't have the expected text.");
                        break;
                }
            }

            var doors = new List<IMyDoor>();
            grid.GetBlocksOfType(doors, d => d.CustomData.Contains("zone-1") && d.CustomData.Contains("zone-2"));
            foreach (var door in doors)
            {
                Assert.True(DoorStatus.Open == door.Status, $"Airlock joined to Zone 1 and a 2 should be open.");
            }

            var vents = new List<MockAirVent>();
            grid.GetBlocksOfType(vents, v => v.CustomData.Contains("zone-1"));
            foreach (var vent in vents)
            {
                vent.IsDepressurizing = true;
                vent.CanPressurize = false;
            }

            Console.WriteLine("Executing Run #2 (Decompression - Zone 1)");
            MDKFactory.Run(program, updateType: tickrate);

            var lights1 = new List<IMyLightingBlock>();
            grid.GetBlocksOfType(lights1, l=>l.CustomData.Contains("zone-1"));
            foreach (var light in lights1)
            {
                if (light.CustomData.Contains(Program.Function.Siren))
                {
                    Assert.Equals(Styler.Get<Color>("decompression.light.color"), light.Color, $"Light {light.EntityId} does not have the expected color.");
                    Assert.Equals(Styler.Get<Single>("decompression.light.interval"), light.BlinkIntervalSeconds, $"Light {light.EntityId} does not have the expected blink interval.");
                    Assert.Equals(Styler.Get<Single>("decompression.light.duration"), light.BlinkLength, $"Light {light.EntityId} does not have the expected blink length.");
                    Assert.Equals(Styler.Get<Single>("decompression.light.offset"), light.BlinkOffset, $"Light {light.EntityId} does not have the expected blink offset.");
                    Assert.True(light.Enabled, $"Light {light.EntityId} should be enabled.");
                }
                else
                {
                    Assert.False(light.Enabled, $"Light {light.EntityId} should not be enabled.");
                }
            }

            var lcds1 = new List<IMyTextPanel>();
            grid.GetBlocksOfType(lcds1, l => l.CustomData.Contains("zone-1"));
            foreach (var lcd in lcds1)
            {
                switch (lcd.CustomName)
                {
                    case "Door Sign (Zone 1)":
                        Assert.Equals(Styler.Get<String>("decompression.text"), lcd.GetPublicText(), $"LCD {lcd.EntityId} doesn't have the expected text.");
                        Assert.True(1.327 < lcd.FontSize, $"LCD {lcd.EntityId} did not properly rescale.");
                        Assert.True(1.328 > lcd.FontSize, $"LCD {lcd.EntityId} did not properly rescale.");
                        break;
                }
            }

            var doors1 = new List<IMyDoor>();
            grid.GetBlocksOfType(doors1, d => d.CustomData.Contains("zone-1") && d.CustomData.Contains("zone-2"));
            foreach (var door in doors1)
            {
                Assert.True(DoorStatus.Closed == door.Status || DoorStatus.Closing == door.Status, $"Airlock joined to Zone 1 and a 2 should be closed or closing.");
                Assert.True(door.Enabled, $"Airlock joined to Zone 1 and a 2 should be enabled.");
            }

            Console.WriteLine("Executing Run #4 (Decompression - Zone 1)");
            MDKFactory.Run(program, updateType: tickrate);
            
            foreach (var door in doors1)
            {
                Assert.True(DoorStatus.Closed == door.Status, $"Airlock joined to Zone 1 and a 2 should be closed.");
                Assert.False(door.Enabled, $"Airlock joined to Zone 1 and a 2 should be disabled.");
            }

            foreach (var vent in vents)
            {
                vent.IsDepressurizing = false;
                vent.CanPressurize = true;
            }
            
            MDKFactory.Run(program, updateType: tickrate);

            foreach (var light in lights1)
            {
                Assert.True(light.Enabled, $"Light {light.EntityId} should be enabled.");
                Assert.Equals(new Color(255, 255, 255), light.Color, $"Light {light.EntityId} does not have the expected color.");
                Assert.Equals(0, light.BlinkIntervalSeconds, $"Light {light.EntityId} does not have the expected blink interval.");
                Assert.Equals(0, light.BlinkLength, $"Light {light.EntityId} does not have the expected blink length.");
                Assert.Equals(0, light.BlinkOffset, $"Light {light.EntityId} does not have the expected blink offset.");
            }

            foreach (var lcd in lcds1)
            {
                switch (lcd.CustomName)
                {
                    case "Door Sign (Zone 1)":
                        Assert.Equals("ZONE 1", lcd.GetPublicText(), $"LCD {lcd.EntityId} doesn't have the expected text.");
                        Assert.Equals(new Color(255, 255, 255), lcd.FontColor, $"LCD {lcd.EntityId} doesn't have the expected text color.");
                        Assert.Equals(new Color(0, 0, 0), lcd.BackgroundColor, $"LCD {lcd.EntityId} doesn't have the expected background color.");
                        Assert.Equals(1, lcd.FontSize, $"LCD {lcd.EntityId} Font Size was not correctly restored.");
                        break;
                }
            }

            Console.WriteLine("Executing Run #5 (Enable Battle Stations)");
            MDKFactory.Run(program, argument: "activate battle");

            Assert.Equals("battle", new MyConfig(programmableBlock).GetValue("custom-states").ToString().Trim(), "Activating battle state did not have desired effect.");

            Console.WriteLine("Executing Run #6 (Enable Self-Destruct)");
            MDKFactory.Run(program, argument: "activate destruct");

            Assert.Equals("battle\ndestruct", new MyConfig(programmableBlock).GetValue("custom-states").ToString().Trim(), "Activating destruct state did not have desired effect.");

            Console.WriteLine("Executing Run #7 (Toggle Battle Stations)");
            MDKFactory.Run(program, argument: "toggle battle");

            Console.WriteLine("Executing Run #8 (Self Destruct)");
            MDKFactory.Run(program, updateType: tickrate);
            Assert.Equals("destruct", new MyConfig(programmableBlock).GetValue("custom-states").ToString().Trim(), "Toggling battle state did not have desired effect.");

            Console.WriteLine("Executing Run #9 (Toggle Self-Destruct)");
            MDKFactory.Run(program, argument: "deactivate destruct");

            Assert.Equals("", new MyConfig(programmableBlock).GetValue("custom-states").ToString(), "Disabling destruct state did not have desired effect.");

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("DEBUG LCD OUTPUT:");
            Console.WriteLine("-----------------------------------");
            var blocks = new List<IMyTextPanel>();
            grid.GetBlocksOfType(blocks, t => (new MyConfig(t)).IsA("debug lcd"));
            Console.WriteLine(blocks.FirstOrDefault()?.GetPublicText()?.Trim() ?? ">> NO LCD FOUND <<");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }

        class Zone : IReadOnlyList<IMyTerminalBlock>
        {
            public String Name { get; }
            public IEnumerable<String> AdjacentZones => Blocks.SelectMany(b => new MyConfig(b).GetZones()).Distinct().Where(z => z != Name);
            public List<IMyTerminalBlock> Blocks { get; } = new List<IMyTerminalBlock>();

            public Int32 Count => ((IReadOnlyList<IMyTerminalBlock>)Blocks).Count;

            public IMyTerminalBlock this[Int32 index] => ((IReadOnlyList<IMyTerminalBlock>)Blocks)[index];

            public Zone(String name)
                : base()
            {
                Name = name;
            }

            public void AddBlock(IMyTerminalBlock block)
            {
                Blocks.Add(block);
                using (var config = new MyConfig(block))
                {
                    config.AddValue("zones", Name);
                }
            }

            public void RemoveBlock(IMyTerminalBlock block)
            {
                Blocks.Remove(block);
                using (var config = new MyConfig(block))
                {
                    config.ClearValue("zones", Name);
                }
            }

            public void AddBlocks(IEnumerable<IMyTerminalBlock> blocks)
            {
                Blocks.AddRange(blocks);
                foreach (var block in blocks)
                {
                    using (var config = new MyConfig(block))
                    {
                        config.AddValue("zones", Name);
                    }
                }
            }

            public void AddToGrid(MockGridTerminalSystem grid)
            {
                var existing = new List<IMyTerminalBlock>();
                grid.GetBlocks(existing);

                foreach (var block in Blocks.Where(b => !existing.Contains(b)))
                {
                    grid.Add(block);
                }
            }

            public IEnumerator<IMyTerminalBlock> GetEnumerator() => ((IReadOnlyList<IMyTerminalBlock>)Blocks).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => ((IReadOnlyList<IMyTerminalBlock>)Blocks).GetEnumerator();
        }
    }
}