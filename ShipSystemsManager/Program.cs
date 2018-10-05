// <mdk sortorder="2" />
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        readonly IOrderedEnumerable<BaseStyler> StatePriority;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            StatePriority = new List<BaseStyler>
            {
                new SelfDestructStyler(Me, GridTerminalSystem),
                new DecompressionStyler(Me),
                new IntruderStyler(Me, BlockState.INTRUDER1), // Intruders detected by turrets.
                new IntruderStyler(Me, BlockState.INTRUDER2), // Intruders detected by sensors.
                new BattleStationsStyler(Me)
            }.OrderBy(s => s.Priority);
        }
        
        public void Main(String argument, UpdateType updateSource)
        {
            if (String.IsNullOrWhiteSpace(argument))
            {
                // Perform a tick.
                if ((updateSource & UpdateType.Update1) != UpdateType.None)
                {
                    // Running in high speed mode is not recommended!
                    if (!Me.GetConfig<Boolean>("FastMode"))
                    {
                        Output("Running the program at one cycle per tick is not recommended.");
                        Output("Add \"FastMode:true\" to the Programmable Block CustomData to enable this mode.");

                        // Throw an exception to prevent further cycles.
                        throw new Exception();
                    }
                }

                if ((updateSource & UpdateType.Update100) != UpdateType.None)
                {
                    // Script is running in slow mode.
                    Output("Script is running in slow mode (once per 6 seconds).");
                }

                Tick();
            }
            else
            {
                // Set or clear argument flags.
                Flags(argument);
            }
        }

        private void Flags(String argument)
        {
            var arguments = argument.Split(' ');
            if (arguments.Count() > 0)
            {
                var state = String.Join(" ", argument.Skip(1));
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
                            Me.SetConfigFlag("custom-states", state);
                        return;

                    case "deactivate":
                        if (!String.IsNullOrWhiteSpace(state))
                            Me.ClearConfigFlag("custom-states", state);
                        return;


                    case "toggle":
                        if (Me.HasConfigFlag("custom-states", state))
                        {
                            Flags("deactivate " + state);
                        }
                        else
                        {
                            Flags("activate " + state);
                        }
                        return;

                    case "customize":
                        switch (arguments.ElementAtOrDefault(1))
                        {
                            case "":
                            case "preserve":
                                Output("Writing default styler settings to the Programmable Block CustomData attribute (preserving existing).");

                                var configs = Me.GetConfig();
                                foreach (var styler in BaseStyler.DefaultStyles.OrderBy(s => s.Key))
                                {
                                    if (!configs.ContainsKey(styler.Key))
                                    {
                                        Me.SetConfig(styler.Key, styler.Value);
                                    }
                                }
                                break;
                            case "overwrite":
                                Output("Writing default styler settings to the Programmable Block CustomData attribute (overwriting existing).");

                                foreach (var styler in BaseStyler.DefaultStyles.OrderBy(s => s.Key))
                                {
                                    Me.SetConfig(styler.Key, styler.Value);
                                }
                                break;
                            case "reset":
                                var config = Me.CustomData.Split('\n');
                                var newConfig = new List<String>();
                                var styles = BaseStyler.DefaultStyles.Select(s => s.Key);
                                
                                foreach (var line in config)
                                {
                                    if (!line.Contains(':') || !styles.Contains(line.Split(':')[0]))
                                        newConfig.Add(line);
                                }

                                Me.CustomData = String.Join("\n", newConfig);
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
                                block.SetConfigFlag("zones", zone);
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
                                block.SetConfigFlag("functions", function);
                            }

                            Output($"Added {function} to {blocks.Count()} blocks.");
                        }
                        break;
                }
            }
        }

        private void Tick()
        {
            // Only check air vents if pressurization is enabled.
            var pressure = GetBlocks<IMyAirVent>().FirstOrDefault(v => v.PressurizationEnabled) != default(IMyAirVent);
            var batteries = GetBlocks<IMyBatteryBlock>().Any();
            
            foreach (var zone in GridTerminalSystem.GetZones())
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
            var text = "";

            foreach (var lcd in lcds)
            {
                lcd.WritePublicTitle("ShipSystemsManager Diagnostics");
                lcd.FontSize = 1.5f;
                lcd.Font = "DEBUG";

                if (append)
                {
                    if (text == "")
                    {
                        // Do scrolling logic only once.
                        var lines = lcd.GetPublicText().Split('\n').ToList();
                        if (lines.Count() > 34)
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
            foreach (var block in GetBlocks<IMyTerminalBlock>(b => b.GetConfig<Boolean>("state-changed")))
            {
                var states = block.GetConfigs("state");
                var styler = StatePriority.FirstOrDefault(s => states.Contains(s.State));
                
                if (styler == default(BaseStyler))
                {
                    styler = new DefaultStyler(Me);
                }

                styler.Style(block);
                block.SetConfig("state-changed", false);
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