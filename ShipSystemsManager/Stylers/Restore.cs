using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public static void StyleRestore(IMyTerminalBlock block)
        {
            block.RestoreState();
        }
    }
}
