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
            protected override String StylePrefix => "intruder";
            public IntruderStyler(IMyProgrammableBlock block, String blockState)
                : base(3, blockState, block) { }

            public override void Style(IMyTerminalBlock block)
            {
                var door = block as IMyDoor;
                if (door != default(IMyDoor))
                {
                    door.ApplyConfig(new Dictionary<String, Object>()
                    {
                        { Serialization.CustomProperties.Closed, true }
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
                            { Serialization.CustomProperties.PublicText, GetStyle<String>("text") },
                            { nameof(IMyTextPanel.FontColor), GetStyle<Color>("text.color") },
                            { nameof(IMyTextPanel.BackgroundColor), GetStyle<Color>("sign.color") },
                            { nameof(IMyTextPanel.FontSize), GetStyle<Single>("text.size") }
                        });
                    }

                    if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
                    {
                        lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { Serialization.CustomProperties.Images, GetStyle<Color>("sign.images") },
                            { nameof(IMyTextPanel.Enabled), true }
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
                        { nameof(IMySoundBlock.SelectedSound), GetStyle<Color>("sound") },
                        { nameof(IMySoundBlock.LoopPeriod), 3600 },
                        { nameof(IMySoundBlock.Enabled), true },
                        { nameof(IMySoundBlock.Play), true }
                    });
                    }
                    else
                    {
                        soundBlock.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { nameof(IMySoundBlock.Play), false }
                        });
                    }

                    return;
                }
            }
        }
    }
}