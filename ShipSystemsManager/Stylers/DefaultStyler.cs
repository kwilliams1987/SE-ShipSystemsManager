using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        class DefaultStyler : BaseStyler
        {
            public DefaultStyler()
                : base(1000, "") { }

            public override void Style(IMyTerminalBlock block)
            {
                block.RestoreState();
            }
        }
    }
}
