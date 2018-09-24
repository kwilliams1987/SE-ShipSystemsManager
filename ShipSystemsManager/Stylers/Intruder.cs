using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;
using VRage.Game;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        public static void StyleIntruder(IMyTerminalBlock block)
        {
            var door = block as IMyDoor;
            if (door != default(IMyDoor))
            {
                door.ApplyConfig(new Dictionary<String, Object>()
                    {
                        { "Closed", true }
                    });
            }

            var lcd = block as IMyTextPanel;
            if (lcd != default(IMyTextPanel))
            {
                if (lcd.HasFunction(BlockFunction.SIGN_DOOR))
                {
                    lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "PublicText", Configuration.Intruder.ZONE_LABEL },
                            { "FontColor", Configuration.Intruder.SIGN_FOREGROUND_COLOR },
                            { "BackgroundColor", Configuration.Intruder.SIGN_BACKGROUND_COLOR },
                            { "FontSize", 2.9f / Configuration.Intruder.FONTSIZE }
                        });
                }

                if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
                {
                    lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "Images", Configuration.Intruder.SIGN_IMAGE },
                            { "Enabled", true }
                        });
                }
            }

            var soundBlock = block as IMySoundBlock;
            if (soundBlock != default(IMySoundBlock))
            {
                if (soundBlock.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
                {
                    soundBlock.ApplyConfig(new Dictionary<String, Object>
                    {
                        { "SelectedSound", Configuration.Intruder.ALERT_SOUND },
                        { "LoopPeriod", 3600 },
                        { "Enabled", true },
                        { "Play", true }
                    });
                }
                else
                {
                    soundBlock.ApplyConfig(new Dictionary<String, Object>()
                    {
                        { "Play", false }
                    });
                }
            }
        }
    }
}