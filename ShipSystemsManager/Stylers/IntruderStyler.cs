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
        class IntruderStyler : BaseStyler
        {
            protected override String Prefix => "intruder";
            public IntruderStyler(IMyProgrammableBlock block, String blockState) : base(3, blockState, block) { }

            public override void Style(IMyTerminalBlock block, MyIni storage)
            {
                var config = new MyConfig(block);

                var door = block as IMyDoor;
                if (door != default(IMyDoor))
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
                    if (config.IsA(Function.DoorSign))
                    {
                        lcd.Apply(new Dictionary<String, Object>()
                        {
                            { Serializer.Custom.PublicText, Get<String>("text") },
                            { nameof(IMyTextPanel.FontColor), Get<Color>("text.color") },
                            { nameof(IMyTextPanel.BackgroundColor), Get<Color>("sign.color") }
                        }, storage);

                        lcd.AutoFitText();
                    }

                    if (config.IsA(Function.Warning))
                    {
                        lcd.Apply(new Dictionary<String, Object>()
                        {
                            { Serializer.Custom.Images, Get<Color>("sign.images") },
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
                            { nameof(IMySoundBlock.SelectedSound), Get<Color>("sound") },
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
            }
        }
    }
}