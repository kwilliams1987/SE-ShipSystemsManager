using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        class IntruderStyler : BaseStyler
        {
            static readonly String ZONE_LABEL = "INTRUDER ALERT";
            static readonly Color SIGN_FOREGROUND_COLOR = new Color(255, 0, 0);
            static readonly Color SIGN_BACKGROUND_COLOR = new Color(0, 0, 0);
            static readonly String SIGN_IMAGE = "Danger";
            static readonly String ALERT_SOUND = "Alert 1";
            static readonly Single FONTSIZE = 2.9f / ZONE_LABEL.Split('\n').Length;

            static readonly Color LIGHT_COLOR = new Color(255, 0, 0);

            public IntruderStyler(String blockState)
                : base(2, blockState) { }

            public override void Style(IMyTerminalBlock block)
            {
                var door = block as IMyDoor;
                if (door != default(IMyDoor))
                {
                    door.ApplyConfig(new Dictionary<String, Object>()
                    {
                        { "Closed", true }
                    });

                    return;
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

                    return;
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
            }
        }
    }
}