using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
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
            switch (arguments.FirstOrDefault())
            {
                case "activate":
                    var newstate = String.Join(" ", arguments.Skip(1));

                    Me.SetConfigFlag("custom-states", newstate);
                    return;
                case "deactivate":
                    var oldstate = String.Join(" ", arguments.Skip(1));

                    Me.ClearConfigFlag("custom-states", oldstate);
                    return;
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
                TestBattleStations();

                ApplyBlockStates();
            }
        }

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
                var styler = StatePriority.FirstOrDefault(s => states.Contains(s.Key));
                
                if (styler != default(StateStyler))
                {
                    styler.Style(block);
                }
                else
                {
                    StyleRestore(block);
                }
            }
        }
    }
}