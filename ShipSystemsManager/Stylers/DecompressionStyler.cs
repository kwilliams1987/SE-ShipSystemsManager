using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        class DecompressionStyler : BaseStyler
        {
            protected override String Prefix => "decompression";

            public DecompressionStyler(IMyProgrammableBlock block)
                : base(2, Program.State.Decompression, block) { }

            public override void Style(IMyTerminalBlock block, MyIni storage)
            {
                var door = block as IMyDoor;
                if (door != default(IMyDoor) && door.IsA(Function.Airlock))
                {
                    door.Apply(new Dictionary<String, Object>()
                    {
                        { Serializer.Custom.Locked, true }
                    }, storage);
                }

                var lcd = block as IMyTextPanel;
                if (lcd != default(IMyTextPanel))
                {
                    if (lcd.IsA(Function.DoorSign))
                    {
                        lcd.Apply(new Dictionary<String, Object>()
                        {
                            { Serializer.Custom.PublicText, Get<String>("text") },
                            { nameof(IMyTextPanel.FontColor), Get<Color>("text.color") },
                            { nameof(IMyTextPanel.BackgroundColor), Get<Color>("sign.color") },
                            { nameof(IMyTextPanel.FontSize), Get<Single>("text.size") }
                        }, storage);
                    }

                    if (lcd.IsA(Function.Warning))
                    {
                        lcd.Apply(new Dictionary<String, Object>()
                        {
                            { Serializer.Custom.Images, Get<String>("sign.images") },
                            { nameof(IMyTextPanel.Enabled), true }
                        }, storage);
                    }
                }

                var soundBlock = block as IMySoundBlock;
                if (soundBlock != default(IMySoundBlock))
                {
                    if (soundBlock.IsA(Function.Siren))
                    {
                        soundBlock.Apply(new Dictionary<String, Object>
                        {
                            { nameof(IMySoundBlock.SelectedSound), Get<String>("sound") },
                            { nameof(IMySoundBlock.LoopPeriod), 3600 },
                            { nameof(IMySoundBlock.Enabled), true },
                            { nameof(IMySoundBlock.Play), true }
                        }, storage);
                    }
                    else
                    {
                        soundBlock.Apply(new Dictionary<String, Object>()
                        {
                            { nameof(IMySoundBlock.Play), false }
                        }, storage);
                    }

                    return;
                }

                var light = block as IMyLightingBlock;
                if (light != default(IMyLightingBlock))
                {
                    if (light.IsA(Function.Siren))
                    {
                        light.Apply(new Dictionary<String, Object>()
                        {
                            { nameof(IMyInteriorLight.Color), Get<Color>("light.color") },
                            { nameof(IMyInteriorLight.BlinkIntervalSeconds), Get<Single>("light.interval") },
                            { nameof(IMyInteriorLight.BlinkLength), Get<Single>("light.duration") },
                            { nameof(IMyInteriorLight.BlinkOffset), Get<Single>("light.offset") },
                            { nameof(IMyInteriorLight.Enabled), true },
                        }, storage);
                    }
                    else
                    {
                        light.Apply(new Dictionary<String, Object>()
                        {
                            { nameof(IMyInteriorLight.Enabled), false }
                        }, storage);
                    }

                    return;
                }
            }
        }
    }
}
