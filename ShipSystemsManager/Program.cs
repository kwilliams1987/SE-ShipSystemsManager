// <mdk sortorder="2" />
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private MyIni GridStorage { get; set; }
        private MyConfig SelfStorage { get; set; }

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            GridStorage = new MyIni();
            StatePriority = new List<BaseStyler>
            {
                new SelfDestructStyler(Me, GridTerminalSystem),
                new DecompressionStyler(Me),
                new IntruderStyler(Me, BlockState.INTRUDER1), // Turrets
                new IntruderStyler(Me, BlockState.INTRUDER2), // Sensors
                new BattleStationsStyler(Me)
            }.OrderBy(s => s.Priority);
        }

        public void Main(String argument, UpdateType updateSource)
        {
            {
                var error = default(MyIniParseResult);
                if (!GridStorage.TryParse(Storage, out error))
                {
                    throw new Exception(error.Error);
                }
            }

            try
            {
                SelfStorage = new MyConfig(Me);
                if (String.IsNullOrWhiteSpace(argument))
                {
                    // Perform a tick.
                    if ((updateSource & UpdateType.Update1) != UpdateType.None)
                    {
                        // Running in high speed mode is not recommended!
                        if (!SelfStorage.GetValue("FastMode").ToBoolean())
                        {
                            Output("Running the program at one cycle per tick is not recommended.");
                            Output("Add \"FastMode:true\" to the Programmable Block CustomData to enable this mode.");

                            // Throw an exception to prevent further cycles.
                            throw new Exception();
                        }
                    }

                    Tick();
                }
                else
                {
                    // Set or clear argument flags.
                    Flags(argument);
                }
            }
            finally
            {
                Storage = GridStorage.ToString();
                SelfStorage.Dispose();
                Output($"{Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount} instructions. {Runtime.LastRunTimeMs}ms");
            }
        }

        private void Flags(String argument)
        {
            var arguments = argument.Split(' ');
            if (arguments.Count() > 0)
            {
                var state = String.Join(" ", arguments.Skip(1));
                switch (arguments.First())
                {
                    case "activate":
                        switch (state)
                        {
                            case "destruct":
                                var warheads = GetBlocks<IMyWarhead>(w => w.HasFunction(BlockFunction.WARHEAD_DESTRUCT) && w.IsFunctional);
                                if (!warheads.Any())
                                {
                                    Output("WARNING: Self Destruct is unavailable.");
                                }
                                break;
                        }

                        if (!String.IsNullOrWhiteSpace(state))
                            SelfStorage.AddValue("custom-states", state);
                        return;

                    case "deactivate":
                        if (!String.IsNullOrWhiteSpace(state))
                            SelfStorage.ClearValue("custom-states", state);
                        return;


                    case "toggle":
                        if (SelfStorage.GetValues("custom-states").Contains(state))
                        {
                            Flags("deactivate " + state);
                        }
                        else
                        {
                            Flags("activate " + state);
                        }
                        return;

                    case "reset":
                        if (arguments.ElementAtOrDefault(1) == "confirm")
                        {
                            var styler = new DefaultStyler(Me);
                            foreach (var block in GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>())
                            {
                                ClearStates(block);
                                styler.Style(block, GridStorage);
                            }

                            Storage = "";

                            using (var config = Me.GetConfig())
                                config.ClearValue("custom-states");
                        }
                        else
                        {
                            Output("You must confirm this action - using it will break block states for any zone not accurately stored.");
                        }
                        return;

                    case "customize":
                        switch (arguments.ElementAtOrDefault(1))
                        {
                            case "":
                            case "preserve":
                                Output("Writing default styler settings to the Programmable Block CustomData attribute (preserving existing).");

                                using (var configs = Me.GetConfig())
                                {
                                    foreach (var styler in BaseStyler.DefaultStyles.OrderBy(s => s.Key))
                                    {
                                        if (!configs.ContainsKey(styler.Key))
                                        {
                                            SelfStorage.SetValue(styler.Key, styler.Value);
                                        }
                                    }
                                }
                                break;
                            case "overwrite":
                                Output("Writing default styler settings to the Programmable Block CustomData attribute (overwriting existing).");

                                foreach (var styler in BaseStyler.DefaultStyles.OrderBy(s => s.Key))
                                {
                                    SelfStorage.SetValue(styler.Key, styler.Value);
                                }
                                break;
                            case "reset":
                                SelfStorage.Clear();

                                foreach (var styler in BaseStyler.DefaultStyles.OrderBy(s => s.Key))
                                {
                                    SelfStorage.SetValue(styler.Key, styler.Value);
                                }
                                break;
                        }
                        break;

                    case "grouptozone":
                        {
                            var args = String.Join(" ", arguments.Skip(1)).Split(';');
                            if (args.Count() != 2)
                            {
                                Output("grouptozone failed: Incorrect argument count.");
                                break;
                            }

                            var group = args.ElementAt(0);
                            var zone = args.ElementAt(1);

                            var blockGroup = GridTerminalSystem.GetBlockGroupWithName(group);

                            if (blockGroup == default(IMyBlockGroup))
                            {
                                Output($"grouptozone failed: Group \"{group}\" was not found.");
                                return;
                            }

                            var blocks = new List<IMyTerminalBlock>();
                            blockGroup.GetBlocks(blocks);

                            foreach (var block in blocks)
                            {
                                using (var config = block.GetConfig())
                                {
                                    config.AddValue("zones", zone);
                                }
                            }

                            Output($"Added {blocks.Count()} blocks to zone {zone}.");
                        }
                        break;
                        
                    case "grouptofunction":
                        {
                            var args = String.Join(" ", arguments.Skip(1)).Split(';');
                            if (args.Count() != 2)
                            {
                                Output("grouptofunction failed: Incorrect argument count.");
                                break;
                            }

                            var group = args.ElementAt(0);
                            var function = args.ElementAt(1);

                            var blockGroup = GridTerminalSystem.GetBlockGroupWithName(group);

                            if (blockGroup == default(IMyBlockGroup))
                            {
                                Output($"grouptofunction failed: Group \"{group}\" was not found.");
                                return;
                            }

                            var blocks = new List<IMyTerminalBlock>();
                            blockGroup.GetBlocks(blocks);

                            foreach (var block in blocks)
                            {
                                using (var config = block.GetConfig())
                                {
                                    config.AddValue("functions", function);
                                }
                            }

                            Output($"Added {function} to {blocks.Count()} blocks.");
                        }
                        break;
                }
            }
        }

        private void Tick()
        {
            Output($"Running tick.");
            // Only check air vents if pressurization is enabled.
            var pressure = GetBlocks<IMyAirVent>().FirstOrDefault(v => v.PressurizationEnabled) != default(IMyAirVent);
            var batteries = GetBlocks<IMyBatteryBlock>().Any();
            var zones = GridTerminalSystem.GetZones();

            Output($"Found {zones.Count()} zones.");

            try
            {
                foreach (var zone in zones)
                {
                    Output($"Checking Zone \"{zone}\" for new triggers.");

                    if (pressure)
                    {
                        TestAirVents(zone);
                    }

                    TestSensors(zone);
                    TestInteriorWeapons(zone);
                }

                if (batteries)
                {
                    TestLowPower();
                }

                TestSelfDestruct();
                TestBattleStations();

                ApplyBlockStates();
            }
            catch (Exception e)
            {
                Output("Exception: " + e.Message + "\n" + e.StackTrace);                    
            }
        }

        /// <summary>
        /// Output the provided message to the Programmable Block debug UI and any other LCDs with the "debug lcd" function enabled.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="append"></param>
        private void Output(String message, Boolean append = true)
        {
            message = $"[{DateTime.Now:HH:mm:ss}] {message}";
            Echo(message);

            var lcds = GetBlocks<IMyTextPanel>(p => p.HasFunction("debug lcd"));
            Echo($"[{DateTime.Now:HH:mm:ss}] Found {lcds.Count()} Debug LCD panels.");
            var text = "";

            foreach (var lcd in lcds)
            {
                lcd.WritePublicTitle("ShipSystemsManager Diagnostics");
                lcd.FontSize = 0.5f;
                lcd.Font = "DEBUG";

                if (append)
                {
                    if (text == "")
                    {
                        // Do scrolling logic only once.
                        var lines = lcd.GetPublicText().Split('\n').ToList();
                        if (lines.Count() > 32)
                        {
                            lines.RemoveAt(0);
                        }
                        lines.Add(message);
                        text = String.Join("\n", lines);
                    }

                    lcd.WritePublicText(text);
                }
                else
                {
                    lcd.WritePublicText(message);
                }
            }
        }

        private void ApplyBlockStates()
        {
            foreach (var block in GetBlocks<IMyTerminalBlock>().Where(b => GridStorage.Get(BlockKey(b), "state-changed").ToBoolean()))
            {
                var states = GetStates(block);
                var styler = StatePriority.FirstOrDefault(s => states.Contains(s.State));

                if (styler == default(BaseStyler))
                {
                    styler = new DefaultStyler(Me);
                }

                styler.Style(block, GridStorage);
                GridStorage.Set(BlockKey(block), "state-changed", false);
            }
        }

        private IEnumerable<T> GetZoneBlocks<T>(String zone, String function = "", Boolean all = true)
            where T: class, IMyTerminalBlock
        {
            if (String.IsNullOrWhiteSpace(function))
            {
                return GridTerminalSystem.GetZoneBlocks<T>(zone, all);
            }
            else
            {
                return GridTerminalSystem.GetZoneBlocksByFunction<T>(zone, function, all);
            }
        }

        private IEnumerable<T> GetBlocks<T>(Func<T, Boolean> collect = null)
            where T : class, IMyTerminalBlock => GridTerminalSystem.GetBlocksOfType(collect);
    }
}