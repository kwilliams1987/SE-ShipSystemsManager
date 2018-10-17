using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        class BattleStationsStyler : BaseStyler
        {
            protected override String Prefix => "battle";

            public BattleStationsStyler(IMyProgrammableBlock block) : base(4, Program.State.BattleStations, block) { }

            public override void Style(IMyTerminalBlock block, MyIni storage)
            {
                var config = new MyConfig(block);
                var door = block as IMyDoor;
                if (door != default(IMyDoor) && config.IsA(Function.Security))
                {
                    door.Apply(new Dictionary<String, Object>()
                    {
                        { Serializer.Custom.Closed, true }
                    }, storage);

                    return;
                }

                var lcd = block as IMyTextPanel;
                if (lcd != default(IMyTextPanel))
                {
                    if (config.IsA(Function.BattleSign))
                    {
                        lcd.Apply(new Dictionary<String, Object>()
                        {
                            { Serializer.Custom.PublicText, Get<String>("text") },
                            { nameof(IMyTextPanel.Font), Get<String>("text.font") },
                            { nameof(IMyTextPanel.FontColor), Get<Color>("text.color") },
                            { nameof(IMyTextPanel.BackgroundColor), Get<Color>("sign.color") }
                        }, storage);

                        lcd.AutoFitText();
                    }

                    if (config.IsA(Function.Warning))
                    {
                        lcd.Apply(new Dictionary<String, Object>()
                        {
                            { Serializer.Custom.Images, Get<String>("sign.images") },
                            { nameof(IMyTextPanel.Enabled), true }
                        }, storage);
                    }

                    return;
                }

                var soundBlock = block as IMySoundBlock;
                if (soundBlock != default(IMySoundBlock))
                {
                    if (config.IsA(Function.Siren))
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
                    if (config.IsA(Function.Siren))
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
                }
            }
        }
    }
}
