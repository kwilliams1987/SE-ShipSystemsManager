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
            switch (updateSource)
            {
                case UpdateType.Update1:
                    if (EnableSingleTickCycle)
                    {
                        Tick();
                    }
                    else
                    {
                        throw new Exception("Running the program at one cycle per tick is not recommended.");
                    }
                    return;                    
                case UpdateType.Update10:
                case UpdateType.Update100:
                    Tick();
                    return;

                default:
                    var arguments = argument.Split(' ');
                    switch (arguments.FirstOrDefault())
                    {
                        case "":

                            return;
                        case "activate":
                            var newstate = String.Join(" ", arguments.Skip(1));

                            Me.SetConfigFlag("custom-states", newstate);
                            return;
                        case "deactivate":
                            var oldstate = String.Join(" ", arguments.Skip(1));

                            Me.ClearConfigFlag("custom-states", oldstate);
                            return;
                    }
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
                    HandleAirVents(zone);
                }

                HandleSensors(zone);
                HandleInteriorWeapons(zone);
                HandleBattleStations();
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
    }
}