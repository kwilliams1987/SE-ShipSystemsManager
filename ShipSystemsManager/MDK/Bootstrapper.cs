using System;
using Malware.MDKUtilities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.GameSystems;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.Entities.Blocks;
// using IngameScript.Mockups;
// using IngameScript.Mockups.Blocks;

namespace IngameScript.MDK
{
    public class TestBootstrapper
    {
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
            /* Disabled mockups
            var grid = new MockGridTerminalSystem();
            var programmableBlock = new MockProgrammableBlock()
            {
                DisplayNameText = "Programmable Block [SSM]"
            };

            {
                var vent = new MockAirVent()
                {
                    DisplayNameText = "Air Vent (Deck 1)",
                    ShowInTerminal = false,
                    IsDepressurizing = false
                };
                vent.SetConfig("zones", "deck-1");

                var light = new MockInteriorLight()
                {
                    Name = "Normal Light (Deck 1)",
                    ShowInTerminal = false
                };
                light.SetConfig("zones", "deck-1");
                light.SetConfig("functions", "warnlight");

                grid.Add(vent);
                grid.Add(light);
            }
            
            var program = MDKFactory.CreateProgram<Program>(new MDKFactory.ProgramConfig()
            {
                Echo = message => Console.WriteLine("[{0}] {1}", DateTime.Now.ToString("U"), message),
                GridTerminalSystem = grid,
                ProgrammableBlock = programmableBlock
            });

            MDKFactory.Run(program, updateType: UpdateType.Update10);

            // TODO: Test States.

            var vents = grid.GetBlocksOfType<MockAirVent>(v => v.IsInZone("deck-1"));
            foreach (var vent in vents)
            {
                vent.IsDepressurizing = true;
            }

            MDKFactory.Run(program, updateType: UpdateType.Update10);

            // TODO: Test States.
            */
        }
    }

    internal class MyGridProgramRuntimeInfo : IMyGridProgramRuntimeInfo
    {
        public TimeSpan TimeSinceLastRun
        {
            get
            {
                switch (UpdateFrequency)
                {
                    case UpdateFrequency.Update1:
                        return TimeSpan.FromSeconds(1f / 60);
                    case UpdateFrequency.Update10:
                        return TimeSpan.FromSeconds(1f / 6);
                    case UpdateFrequency.Update100:
                        return TimeSpan.FromSeconds(6f);
                    default:
                        return TimeSpan.Zero;
                }
            }
        }

        public Double LastRunTimeMs { get; protected set; }

        public Int32 MaxInstructionCount { get; protected set; }

        public Int32 CurrentInstructionCount { get; protected set; }

        public Int32 MaxMethodCallCount { get; protected set; }

        public Int32 CurrentMethodCallCount { get; protected set; }

        public UpdateFrequency UpdateFrequency { get; set; }

        public Int32 MaxCallChainDepth { get; set; }

        public Int32 CurrentCallChainDepth { get; set; }

        public MyGridProgramRuntimeInfo(UpdateFrequency updateFrequency)
        {
            UpdateFrequency = updateFrequency;
            LastRunTimeMs = 0;
            MaxInstructionCount = 0;
            CurrentInstructionCount = 0;
            MaxMethodCallCount = 0;
            CurrentMethodCallCount = 0;
        }
    }
}