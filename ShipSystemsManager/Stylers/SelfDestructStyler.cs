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
            static readonly String ZONE_LABEL = "SELF DESTRUCT in {0}";
            static readonly String SIGN_IMAGE = "Danger";
            static readonly Single WARHEAD_TIMER = 180;

            private readonly IMyGridTerminalSystem Grid;

            public SelfDestructStyler(IMyGridTerminalSystem grid)
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
                    countdown = WARHEAD_TIMER;

                if (warhead != default(IMyWarhead) && warhead.HasFunction(BlockFunction.WARHEAD_DESTRUCT))
                {
                    if (!warhead.IsCountingDown)
                    {
                        warhead.SaveState();
                        warhead.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "IsArmed", true },
                            { "DetonationTime", countdown },
                            { "Countdown", true }
                        });
                    }
                }

                var lcd = block as IMyTextPanel;
                if (lcd != default(IMyTextPanel))
                {
                    var label = String.Format(ZONE_LABEL, TimeSpan.FromSeconds(countdown));
                    var fontSize = label.Split('\n').Length;

                    if (lcd.HasFunction(BlockFunction.SIGN_WARNING))
                    {
                        lcd.ApplyConfig(new Dictionary<String, Object>()
                        {
                            { "PublicText", label },
                            { "FontSize", fontSize }
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
            }
        }
    }
}
