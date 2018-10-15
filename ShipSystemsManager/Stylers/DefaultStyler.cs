using Sandbox.ModAPI.Ingame;
using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {
        class DefaultStyler : BaseStyler
        {
            protected override String StylePrefix => "";

            public DefaultStyler(IMyProgrammableBlock block)
                : base(1000, "", block) { }

            public override void Style(IMyTerminalBlock block, MyIni storage) => block.RestoreConfig(storage);
        }
    }
}
