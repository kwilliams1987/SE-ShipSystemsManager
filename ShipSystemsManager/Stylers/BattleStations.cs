using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        public static void StyleBattleStations(IMyTerminalBlock block)
        {
            var door = block as IMyDoor;
            if (door != default(IMyDoor) && door.HasFunction(BlockFunction.DOOR_SECURITY))
            {
                door.ApplyConfig(new Dictionary<String, Object>()
                    {
                        { "Closed", true }
                    });
            }

            var lcd = block as IMyTextPanel;
            if (lcd != default(IMyTextPanel))
            {
                if (lcd.HasFunction(BlockFunction.SIGN_BATTLE))
                {
                    lcd.ApplyConfig(new Dictionary<String, Object>()
                    {
                        { "PublicText", Configuration.BattleStations.ZONE_LABEL },
                        { "FontColor", Configuration.BattleStations.SIGN_FOREGROUND_COLOR },
                        { "BackgroundColor", Configuration.BattleStations.SIGN_BACKGROUND_COLOR },
                        { "FontSize", Configuration.BattleStations.FONTSIZE }
                    });
                }

                if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
                {
                    lcd.ApplyConfig(new Dictionary<String, Object>()
                    {
                        { "Images", Configuration.BattleStations.SIGN_IMAGE },
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
                            { "SelectedSound", Configuration.BattleStations.ALERT_SOUND },
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

            var light = block as IMyLightingBlock;
            if (light != default(IMyLightingBlock))
            {
                if (light.HasFunction(BlockFunction.LIGHT_WARNING))
                {
                    light.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "Color", Configuration.BattleStations.LIGHT_COLOR },
                            { "BlinkIntervalSeconds", Configuration.BattleStations.LIGHT_BLINK },
                            { "BlinkLength", Configuration.BattleStations.LIGHT_DURATION },
                            { "BlinkOffset", Configuration.BattleStations.LIGHT_OFFSET },
                            { "Enabled", true },
                        });
                }
                else
                {
                    light.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "Enabled", false }
                        });
                }
            }
        }
    }
}
