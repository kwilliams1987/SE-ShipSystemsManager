using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        void TestLowPower()
        {
            var batteries = new List<IMyBatteryBlock>();
            GridTerminalSystem.GetBlocksOfType(batteries);

            if (batteries.Count(b => !b.OnlyRecharge) == 0)
                return;

            var power = batteries.Where(b => b.IsFunctional && !b.OnlyRecharge && b.Enabled).Average(b => b.CurrentStoredPower / b.MaxStoredPower);

            var blocks = new List<IMyTerminalBlock>()
                            .Concat(GetBlocks<IMyAssembler>())
                            .Concat(GetBlocks<IMyLightingBlock>())
                            .Concat(GetBlocks<IMyRefinery>());

            if (power < 0.1f)
            {
                SetStates(blocks, State.LowPower);
            }
            else
            {
                ClearStates(blocks, State.LowPower);
            }
        }
    }
}
