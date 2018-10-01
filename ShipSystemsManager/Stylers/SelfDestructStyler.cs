using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        class SelfDestructStyler: BattleStationsStyler
        {
            protected override String StylePrefix => "destruct";
            private IMyGridTerminalSystem Grid { get; }

            public SelfDestructStyler(IMyProgrammableBlock block, IMyGridTerminalSystem grid)
                : base(block)
            {
                Priority = 1;
                State = BlockState.SELFDESTRUCT;
                Grid = grid;
            }

            public override void Style(IMyTerminalBlock block)
            {
                base.Style(block);

                var warhead = block as IMyWarhead;
                var countdown = Grid.GetBlocksOfType<IMyWarhead>(w => w.IsCountingDown && w.HasFunction(BlockFunction.WARHEAD_DESTRUCT))
                    .Min(w => w.DetonationTime);
                
                if (countdown == default(Single))
                    countdown = GetStyle<Single>("timer");

                if (warhead != default(IMyWarhead) && warhead.HasFunction(BlockFunction.WARHEAD_DESTRUCT))
                {
                    if (!warhead.IsCountingDown)
                    {
                        warhead.SaveState();
                        warhead.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { nameof(IMyWarhead.IsArmed), true },
                            { nameof(IMyWarhead.DetonationTime), countdown },
                            { "Countdown", true }
                        });
                    }
                }

                var lcd = block as IMyTextPanel;
                if (lcd != default(IMyTextPanel))
                {
                    var label = String.Format(GetStyle<String>("text"), TimeSpan.FromSeconds(countdown));
                    var fontSize = label.Split('\n').Length;

                    if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
                    {
                        lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "PublicText", label },
                            { nameof(IMyTextPanel.FontSize), fontSize }
                        });
                    }

                    if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
                    {
                        lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "Images", GetStyle<String>("sign.images") },
                            { nameof(IMyTextPanel.Enabled), true }
                        });
                    }
                }
            }
        }
    }
}
