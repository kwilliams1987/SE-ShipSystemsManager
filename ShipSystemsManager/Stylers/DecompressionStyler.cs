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
            protected override String StylePrefix => "decompression";

            public DecompressionStyler(IMyProgrammableBlock block)
                : base(1, BlockState.DECOMPRESSION, block) { }

            public override void Style(IMyTerminalBlock block)
            {

                var door = block as IMyDoor;
                if (door != default(IMyDoor) && door.HasFunction(BlockFunction.DOOR_AIRLOCK))
                {
                    door.ApplyConfig(new Dictionary<String, Object>()
                    {
                        { nameof(Serialization.CustomProperties.Locked), true }
                    });
                }

                var lcd = block as IMyTextPanel;
                if (lcd != default(IMyTextPanel))
                {
                    if (lcd.HasFunction(BlockFunction.SIGN_DOOR))
                    {
                        lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { nameof(Serialization.CustomProperties.PublicText), GetStyle<String>("text") },
                            { nameof(IMyTextPanel.FontColor), GetStyle<Color>("text.color") },
                            { nameof(IMyTextPanel.BackgroundColor), GetStyle<Color>("sign.color") },
                            { nameof(IMyTextPanel.FontSize), GetStyle<Single>("text.size") }
                        });
                    }

                    if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
                    {
                        lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { nameof(Serialization.CustomProperties.Images), GetStyle<String>("sign.images") },
                            { nameof(IMyTextPanel.Enabled), true }
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
                            { nameof(IMySoundBlock.SelectedSound), GetStyle<String>("sound") },
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

                var light = block as IMyLightingBlock;
                if (light != default(IMyLightingBlock))
                {
                    if (light.HasFunction(BlockFunction.LIGHT_WARNING))
                    {
                        light.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { nameof(IMyInteriorLight.Color), GetStyle<Color>("light.color") },
                            { nameof(IMyInteriorLight.BlinkIntervalSeconds), GetStyle<Single>("light.interval") },
                            { nameof(IMyInteriorLight.BlinkLength), GetStyle<Single>("light.duration") },
                            { nameof(IMyInteriorLight.BlinkOffset), GetStyle<Single>("light.offset") },
                            { nameof(IMyInteriorLight.Enabled), true },
                        });
                    }
                    else
                    {
                        light.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { nameof(IMyInteriorLight.Enabled), false }
                        });
                    }

                    return;
                }
            }
        }
    }
}
