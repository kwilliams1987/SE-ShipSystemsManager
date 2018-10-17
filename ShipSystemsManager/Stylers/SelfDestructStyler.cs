using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        class SelfDestructStyler: BattleStationsStyler
        {
            private String prefix = "destruct";
            protected override String Prefix => prefix;
            IMyGridTerminalSystem Grid { get; }
            List<IMyWarhead> Warheads { get; } = new List<IMyWarhead>();
            Single Countdown { get; }

            public SelfDestructStyler(IMyProgrammableBlock block, IMyGridTerminalSystem grid): base(block)
            {
                Priority = 1;
                State = Program.State.Destruct;
                Grid = grid;

                Grid.GetBlocksOfType(Warheads, w => new MyConfig(w).IsA(Function.SelfDestruct));
                Countdown = Warheads.Where(w => w.IsCountingDown).Select(w => w.DetonationTime).DefaultIfEmpty(0).Min();

                if (Countdown == default(Single))
                    Countdown = Get<Single>("timer");
            }

            public override void Style(IMyTerminalBlock block, MyIni storage)
            {
                prefix = "battle";
                base.Style(block, storage);
                prefix = "destruct";
                var config = new MyConfig(block);

                var warhead = block as IMyWarhead;
                if (warhead != default(IMyWarhead) && config.IsA(Function.SelfDestruct))
                {
                    if (!warhead.IsCountingDown)
                    {
                        warhead.Apply(new Dictionary<String, Object>()
                        {
                            { nameof(IMyWarhead.IsArmed), true },
                            { nameof(IMyWarhead.DetonationTime), Countdown },
                            { Serializer.Custom.Countdown, true }
                        }, storage);
                    }
                }

                var lcd = block as IMyTextPanel;
                if (lcd != default(IMyTextPanel))
                {
                    var label = String.Format(Get<String>("text"), TimeSpan.FromSeconds(Countdown));
                    var color = Get<Color>("text.color");

                    if (!Warheads.Any())
                    {
                        label = Get<String>("text.fail");
                        color = Get<Color>("text.fail.color");
                    }
                    
                    if (config.IsA(Function.DoorSign))
                    {
                        lcd.Apply(new Dictionary<String, Object>()
                            {
                                { nameof(IMyTextPanel.BackgroundColor), new Color(0, 0, 0) },
                                { nameof(IMyTextPanel.FontColor), color },
                                { Serializer.Custom.PublicText, label }
                            }, storage);

                        lcd.AutoFitText();
                    }

                    if (config.IsA(Function.Warning) || config.IsA(Function.BattleSign))
                    {
                        if (lcd.IsSmall())
                        {
                            lcd.Apply(new Dictionary<String, Object>()
                            {
                                { nameof(IMyTextPanel.BackgroundColor), new Color(0, 0, 0) },
                                { nameof(IMyTextPanel.FontColor), color },
                                { Serializer.Custom.PublicText, label }
                            }, storage);

                            lcd.AutoFitText();
                        }
                        else
                        {
                            lcd.Apply(new Dictionary<String, Object>()
                            {
                                { Serializer.Custom.Images, Get<String>("sign.images") },
                                { nameof(IMyTextPanel.Enabled), true }
                            }, storage);
                        }
                    }
                }
            }
        }
    }
}
