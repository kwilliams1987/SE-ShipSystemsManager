using System.Linq;
using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public static void StyleDecompression(IMyTerminalBlock block)
        {

            var door = block as IMyDoor;
            if (door != default(IMyDoor) && door.HasFunction(BlockFunction.DOOR_AIRLOCK))
            {
                door.ApplyConfig(new Dictionary<String, Object>()
                    {
                        { "Locked", true }
                    });
            }

            var lcd = block as IMyTextPanel;
            if (lcd != default(IMyTextPanel))
            {
                if (lcd.HasFunction(BlockFunction.SIGN_DOOR))
                {
                    lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "PublicText", Configuration.Decompression.ZONE_LABEL },
                            { "FontColor", Configuration.Decompression.SIGN_FOREGROUND_COLOR },
                            { "BackgroundColor", Configuration.Decompression.SIGN_BACKGROUND_COLOR },
                            { "FontSize", 2.9f / Configuration.Decompression.FONTSIZE }
                        });
                }

                if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
                {
                    lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "Images", Configuration.Decompression.SIGN_IMAGE },
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
                        { "SelectedSound", Configuration.Decompression.ALERT_SOUND },
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
                            { "Color", Configuration.Decompression.LIGHT_COLOR },
                            { "BlinkIntervalSeconds", Configuration.Decompression.LIGHT_BLINK },
                            { "BlinkLength", Configuration.Decompression.LIGHT_DURATION },
                            { "BlinkOffset", Configuration.Decompression.LIGHT_OFFSET },
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
