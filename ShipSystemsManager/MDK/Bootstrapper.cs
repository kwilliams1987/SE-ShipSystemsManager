using System;
using System.Linq;
using IngameScript.Mockups;
using IngameScript.Mockups.Asserts;
using IngameScript.Mockups.Blocks;
using Malware.MDKUtilities;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ModAPI;
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

            var grid = new MockGridTerminalSystem();
            var programmableBlock = new MockProgrammableBlock()
            {
                DisplayNameText = "Programmable Block [SSM]"
            };

            {
                var vent = new MockAirVent()
                {
                    Status= VentStatus.Pressurized,
                    DisplayNameText = "Air Vent (Deck 1)",
                    ShowInTerminal = false,
                    IsDepressurizing = false,
                    CanPressurize = true
                };
                vent.SetConfig("zones", "deck-1");

                var alertLight = new MockInteriorLight()
                {
                    Name = "Alert Light (Deck 1)",
                    ShowInTerminal = false
                };
                alertLight.SetConfig("zones", "deck-1");
                alertLight.SetConfig("functions", "warnlight");

                var normalLight = new MockInteriorLight()
                {
                    Name = "Normal Light (Deck 1)",
                    ShowInTerminal = false
                };
                normalLight.SetConfig("zones", "deck-1");

                var debugLcd = new MockTextPanel()
                {
                    Name = "Debug Panel"
                };
                debugLcd.SetConfig("functions", "debug lcd");

                var doorLcd = new MockTextPanel()
                {
                    Name = "Door Sign (Deck 1)"
                };
                doorLcd.WritePublicText("DECK 1");
                doorLcd.SetConfig("zones", "deck-1");
                doorLcd.SetConfig("functions", "doorsign");

                grid.Add(vent);
                grid.Add(alertLight);
                grid.Add(normalLight);
                grid.Add(debugLcd);
                grid.Add(doorLcd);
            }
            
            var program = MDKFactory.CreateProgram<Program>(new MDKFactory.ProgramConfig()
            {
                Echo = message => Console.WriteLine("[{0}] {1}", DateTime.Now.ToString("U"), message),
                GridTerminalSystem = grid,
                ProgrammableBlock = programmableBlock
            });

            Console.WriteLine("Executing Run #1 (Normal)");
            MDKFactory.Run(program, updateType: UpdateType.Update10);
            {
                var lights = grid.GetZoneBlocks<MockInteriorLight>("deck-1");

                foreach (var light in lights)
                {
                    Assert.That(light.Color.PackedValue == new Color(255,255,255).PackedValue, $"Light {light.EntityId} does not have the expected color.");
                    Assert.That(light.BlinkIntervalSeconds == 0, $"Light {light.EntityId} does not have the expected blink interval.");
                    Assert.That(light.BlinkLength == 0, $"Light {light.EntityId} does not have the expected blink length.");
                    Assert.That(light.BlinkOffset == 0, $"Light {light.EntityId} does not have the expected blink offset.");
                }
            }

            foreach (var vent in grid.GetZoneBlocks<MockAirVent>("deck-1"))
            {
                vent.IsDepressurizing = true;
                vent.CanPressurize = false;
            }

            Console.WriteLine("Executing Run #2 (Decompression - Deck 1)");
            MDKFactory.Run(program, updateType: UpdateType.Update10);

            {
                var lights = grid.GetZoneBlocks<MockInteriorLight>("deck-1");

                foreach (var light in lights)
                {
                    if (light.HasFunction(Program.BlockFunction.LIGHT_WARNING))
                    {
                        Assert.That(light.Color.PackedValue == Styler.Get<Color>("decompression.light.color").PackedValue, $"Light {light.EntityId} does not have the expected color.");
                        Assert.That(light.BlinkIntervalSeconds == Styler.Get<Single>("decompression.light.interval"), $"Light {light.EntityId} does not have the expected blink interval.");
                        Assert.That(light.BlinkLength == Styler.Get<Single>("decompression.light.duration"), $"Light {light.EntityId} does not have the expected blink length.");
                        Assert.That(light.BlinkOffset == Styler.Get<Single>("decompression.light.offset"), $"Light {light.EntityId} does not have the expected blink offset.");
                        Assert.That(light.Enabled, $"Light {light.EntityId} should be enabled.");
                    }
                    else
                    {
                        Assert.That(!light.Enabled, $"Light {light.EntityId} should not be enabled.");
                    }
                }
            }

            foreach (var vent in grid.GetZoneBlocks<MockAirVent>("deck-1"))
            {
                vent.IsDepressurizing = false;
                vent.CanPressurize = true;
            }

            Console.WriteLine("Executing Run #3 (Normal)");
            MDKFactory.Run(program, updateType: UpdateType.Update10);
            {
                var lights = grid.GetZoneBlocks<MockInteriorLight>("deck-1");

                foreach (var light in lights)
                {
                    Assert.That(light.Color.PackedValue == new Color(255, 255, 255).PackedValue, $"Light {light.EntityId} does not have the expected color.");
                    Assert.That(light.BlinkIntervalSeconds == 0, $"Light {light.EntityId} does not have the expected blink interval.");
                    Assert.That(light.BlinkLength == 0, $"Light {light.EntityId} does not have the expected blink length.");
                    Assert.That(light.BlinkOffset == 0, $"Light {light.EntityId} does not have the expected blink offset.");
                }
            }

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");

            Console.Write(grid.GetBlocksOfType<IMyTextPanel>(t => t.HasFunction("debug lcd")).FirstOrDefault()?.GetPublicText() ?? ">> NO LCD FOUND <<");

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }
    }
}