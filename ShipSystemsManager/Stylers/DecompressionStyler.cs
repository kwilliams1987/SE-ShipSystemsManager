using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        class DecompressionStyler : BaseStyler
        {
            static readonly String ZONE_LABEL = "DECOMPRESSION DANGER";
            static readonly Color SIGN_FOREGROUND_COLOR = new Color(0, 0, 255);
            static readonly Color SIGN_BACKGROUND_COLOR = new Color(0, 0, 0);
            static readonly String SIGN_IMAGE = "Danger";
            static readonly String ALERT_SOUND = "Alert 2";
            static readonly Single FONTSIZE = 2.9f / ZONE_LABEL.Split('\n').Length;

            static readonly Color LIGHT_COLOR = new Color(0, 0, 255);
            static readonly Single LIGHT_BLINK = 3;
            static readonly Single LIGHT_DURATION = 66.6f;
            static readonly Single LIGHT_OFFSET = 0;

            public DecompressionStyler()
                : base(1, BlockState.DECOMPRESSION) { }

            public override void Style(IMyTerminalBlock block)
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
                            { "PublicText", ZONE_LABEL },
                            { "FontColor", SIGN_FOREGROUND_COLOR },
                            { "BackgroundColor", SIGN_BACKGROUND_COLOR },
                            { "FontSize", 2.9f / FONTSIZE }
                        });
                    }

                    if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
                    {
                        lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "Images", SIGN_IMAGE },
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
                            { "SelectedSound", ALERT_SOUND },
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

                    return;
                }

                var light = block as IMyLightingBlock;
                if (light != default(IMyLightingBlock))
                {
                    if (light.HasFunction(BlockFunction.LIGHT_WARNING))
                    {
                        light.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "Color", LIGHT_COLOR },
                            { "BlinkIntervalSeconds", LIGHT_BLINK },
                            { "BlinkLength", LIGHT_DURATION },
                            { "BlinkOffset", LIGHT_OFFSET },
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

                    return;
                }
            }
        }
    }
}
