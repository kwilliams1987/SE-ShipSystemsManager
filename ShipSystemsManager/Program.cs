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
        public static class Function
        {
            public const String Airlock = "airlock";
            public const String Security = "security";
            public const String DoorSign = "doorsign";
            public const String BattleSign = "battle";
            public const String Warning = "warnsign";
            public const String Siren = "siren";
            public const String SelfDestruct = "selfdestruct";
            public const String AlwaysOn = "lowpower";
        }

        public static class State
        {
            public const String LowPower = "lowpower";
            public const String BattleStations = "battle";
            public const String Decompression = "decompression";
            public const String Intruder1 = "intruder1"; // Turrets
            public const String Intruder2 = "intruder2"; // Sensors
            public const String Destruct = "selfdestruct";
        }

        #region mdk macros
        const String VERSION = "$MDK_DATE$, $MDK_TIME$";
        #endregion

        const String States = "states";

        MyIni GridStorage { get; set; }
        MyConfig SelfStorage { get; set; }
        Dictionary<Int64, MyConfig> EntityStorage { get; set; } = new Dictionary<Int64, MyConfig>();
        readonly IOrderedEnumerable<BaseStyler> StatePriority;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            GridStorage = new MyIni();
            StatePriority = new List<BaseStyler>
            {
                new SelfDestructStyler(Me, GridTerminalSystem),
                new DecompressionStyler(Me),
                new IntruderStyler(Me, State.Intruder1), // Turrets
                new IntruderStyler(Me, State.Intruder2), // Sensors
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
                Output($"{Runtime.TimeSinceLastRun} since last execution.");
                if (String.IsNullOrWhiteSpace(argument))
                {
                    if ((updateSource & (UpdateType.Update1 | UpdateType.Update10)) != UpdateType.None)
                    {
                        // Running in high speed mode is not recommended!
                        if (!SelfStorage.GetValue("FastMode").ToBoolean())
                        {
                            Output("Running the program at one cycle per tick is not recommended.");
                            Output("Add \"FastMode=true\" to the Programmable Block CustomData to enable this mode.");

                            // Throw an exception to prevent further cycles.
                            throw new Exception();
                        }
                    }

                    // Perform a tick.
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
                // Commit config updates.
                Storage = GridStorage.ToString();
                SelfStorage.Dispose();
                foreach (var config in EntityStorage)
                    config.Value.Dispose();

                Output($"{Runtime.LastRunTimeMs}ms. {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount} instructions.");
            }
        }

        void Flags(String argument)
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
                                var warheads = GetBlocks<IMyWarhead>(w => GetConfig(w).IsA(Function.SelfDestruct) && w.IsFunctional);
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
                            foreach (var block in GetBlocksOfType<IMyTerminalBlock>())
                            {
                                ClearStates(block);
                                styler.Style(block, GridStorage);
                            }

                            Storage = "";
                            GetConfig(Me).ClearValue("custom-states");
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

                                foreach (var styler in BaseStyler.DefaultStyles.OrderBy(s => s.Key))
                                {
                                    if (!SelfStorage.ContainsKey(styler.Key))
                                    {
                                        SelfStorage.SetValue(styler.Key, styler.Value);
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
                                GetConfig(block).AddValue("zones", zone);
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
                                GetConfig(block).AddValue("functions", function);
                            }

                            Output($"Added {function} to {blocks.Count()} blocks.");
                        }
                        break;
                }
            }
        }

        void Tick()
        {
            Output($"Running tick.");
            // Only check air vents if pressurization is enabled.
            var pressure = GetBlocks<IMyAirVent>().FirstOrDefault(v => v.PressurizationEnabled) != default(IMyAirVent);
            var batteries = GetBlocks<IMyBatteryBlock>().Any();
            var zones = GetZones();

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
        void Output(String message, Boolean append = true)
        {
            message = $"[{DateTime.Now:HH:mm:ss}] {message}";
            Echo(message);

            var lcds = GetBlocks<IMyTextPanel>(p => GetConfig(p).IsA("debug lcd"));
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

        void ApplyBlockStates()
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

        IEnumerable<T> GetBlocks<T>(Func<T, Boolean> collect = null)
            where T : class, IMyTerminalBlock => GetBlocksOfType(collect);


        String BlockKey(IMyTerminalBlock block) => "Entity " + block.EntityId;

        IEnumerable<String> GetStates(IMyTerminalBlock block)
            => GridStorage.Get(BlockKey(block), States).ToString().Split('\n').Where(s => s != "");

        void SetStates(IMyTerminalBlock block, params String[] states)
        {
            var current = GetStates(block).ToList();
            var concat = current.Concat(states).Distinct();

            if (concat.Count() != current.Count())
            {
                GridStorage.Set(BlockKey(block), States, String.Join("\n", concat));
                GridStorage.Set(BlockKey(block), "state-changed", true);
            }
        }

        void ClearStates(IMyTerminalBlock block, params String[] states)
        {
            var current = GetStates(block).ToList();
            var removed = current.RemoveAll(c => states.Contains(c));

            if (removed > 0)
            {
                GridStorage.Set(BlockKey(block), States, String.Join("\n", current));
                GridStorage.Set(BlockKey(block), "state-changed", true);
            }
        }

        void SetStates(IEnumerable<IMyTerminalBlock> blocks, params String[] states)
        {
            foreach (var block in blocks)
                SetStates(block, states);
        }

        void ClearStates(IEnumerable<IMyTerminalBlock> blocks, params String[] states)
        {
            foreach (var block in blocks)
                ClearStates(block, states);
        }

        MyConfig GetConfig(IMyTerminalBlock block)
        {
            if (!EntityStorage.ContainsKey(block.EntityId))
                EntityStorage.Add(block.EntityId, new MyConfig(block));

            return EntityStorage[block.EntityId];
        }


        public IEnumerable<T> GetBlocksOfType<T>(Func<T, Boolean> collect = null)
            where T : class, IMyTerminalBlock
        {
            var result = new List<T>();
            GridTerminalSystem.GetBlocksOfType(result, collect);
            return result;
        }

        public IEnumerable<T> GetZoneBlocks<T>(String zone, Boolean all = false)
            where T : class, IMyTerminalBlock => GetBlocksOfType<T>(b => (b.IsWorking || all) && GetConfig(b).InZone(zone));

        public IEnumerable<T> GetZoneBlocksByFunction<T>(String zone, String function, Boolean all = false)
            where T : class, IMyTerminalBlock => GetZoneBlocks<T>(zone, all).Where(b => GetConfig(b).IsA(function));

        public IEnumerable<String> GetZones() => GetBlocksOfType<IMyTerminalBlock>().SelectMany(b => GetConfig(b).GetZones()).Distinct();

        public Boolean AdjacentZonesTest<T>(Func<T, Boolean> test, params String[] zones)
            where T : class, IMyTerminalBlock => GetBlocksOfType<T>(v => GetConfig(v).InAnyZone(zones)).All(test);
    }
}