using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {
        class SelfDestructStyler: BattleStationsStyler
        {
            protected override String Prefix => "destruct";
            IMyGridTerminalSystem Grid { get; }

            public SelfDestructStyler(IMyProgrammableBlock block, IMyGridTerminalSystem grid): base(block)
            {
                Priority = 1;
                State = Program.State.Destruct;
                Grid = grid;
            }

            public override void Style(IMyTerminalBlock block, MyIni storage)
            {
                base.Style(block, storage);

                var warhead = block as IMyWarhead;
                var countdown = Grid.GetBlocksOfType<IMyWarhead>(w => w.IsCountingDown && w.IsA(Function.SelfDestruct))
                    .Min(w => w.DetonationTime);
                
                if (countdown == default(Single))
                    countdown = Get<Single>("timer");

                if (warhead != default(IMyWarhead) && warhead.IsA(Function.SelfDestruct))
                {
                    if (!warhead.IsCountingDown)
                    {
                        warhead.Apply(new Dictionary<String, Object>()
                        {
                            { nameof(IMyWarhead.IsArmed), true },
                            { nameof(IMyWarhead.DetonationTime), countdown },
                            { Serializer.Custom.Countdown, true }
                        }, storage);
                    }
                }

                var lcd = block as IMyTextPanel;
                if (lcd != default(IMyTextPanel))
                {
                    var label = String.Format(Get<String>("text"), TimeSpan.FromSeconds(countdown));
                    var fontSize = label.Split('\n').Length;

                    if (lcd.IsA(Function.Warning))
                    {
                        lcd.Apply(new Dictionary<String, Object>()
                        {
                            { Serializer.Custom.PublicText, label },
                            { nameof(IMyTextPanel.FontSize), fontSize }
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
            }
        }
    }
}
