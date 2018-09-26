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
                new SelfDestructStyler(GridTerminalSystem),
                new DecompressionStyler(),
                new IntruderStyler(BlockState.INTRUDER1), // Intruders detected by turrets.
                new IntruderStyler(BlockState.INTRUDER2), // Intruders detected by sensors.
                new BattleStationsStyler()
            }.OrderBy(s => s.Priority);
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(String argument, UpdateType updateSource)
        {
            if (String.IsNullOrWhiteSpace(argument))
            {
                // Perform a tick.
                if ((updateSource & UpdateType.Update1) != UpdateType.None)
                {
                    // Running in high speed mode is not recommended!
                    if (!EnableSingleTickCycle)
                    {
                        // Throw an exception to prevent further cycles.
                        throw new Exception("Running the program at one cycle per tick is not recommended.");
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
            if (arguments.Count() > 1)
            {
                var state = String.Join(" ", argument.Skip(1));
                switch (arguments.FirstOrDefault())
                {
                    case "activate":
                        switch (state)
                        {
                            case "destruct":
                                var warheads = GridTerminalSystem.GetBlocksOfType<IMyWarhead>(w => w.HasFunction(BlockFunction.WARHEAD_DESTRUCT) && w.IsFunctional);
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
                }
            }
        }

        private void Tick()
        {
            // Only check air vents if pressurization is enabled.
            var pressure = GridTerminalSystem.GetBlocksOfType<IMyAirVent>().FirstOrDefault(v => v.PressurizationEnabled) != default(IMyAirVent);
            
            foreach (var zone in GridTerminalSystem.GetZones())
            {
                Output("Checking Zone \"" + zone + "\" for new triggers.");

                if (pressure)
                {
                    TestAirVents(zone);
                }

                TestSensors(zone);
                TestInteriorWeapons(zone);
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
            message = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message;
            Echo(message);

            var lcds = GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(p => p.HasFunction("debug lcd"));

            foreach (var lcd in lcds)
            {
                lcd.WritePublicTitle("ShipSystemsManager Diagnostics");
                lcd.FontSize = 1.5f;
                lcd.Font = "DEBUG";

                if (append)
                {
                    var text = lcd.GetPublicText().Split('\n').ToList();
                    if (text.Count() > 34)
                    {
                        text.RemoveAt(0);
                    }
                    text.Add(message);
                    lcd.WritePublicText(String.Join("\n", text));
                }
                else
                {
                    lcd.WritePublicText(message);
                }
            }
        }

        private void ApplyBlockStates()
        {
            foreach (var block in GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(b => b.GetConfigs("zones").Any()))
            {
                var states = block.GetConfigs("state");
                var styler = StatePriority.FirstOrDefault(s => states.Contains(s.State));
                
                if (styler == default(BaseStyler))
                {
                    styler = new DefaultStyler();
                }

                styler.Style(block);
            }
        }
    }
}