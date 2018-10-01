using Sandbox.ModAPI.Ingame;
using System;

namespace IngameScript
{
    partial class Program
    {
        class DefaultStyler : BaseStyler
        {
            protected override String StylePrefix => "";

            public DefaultStyler(IMyProgrammableBlock block)
                : base(1000, "", block) { }

            public override void Style(IMyTerminalBlock block)
            {
                block.RestoreState();
            }
        }
    }
}
