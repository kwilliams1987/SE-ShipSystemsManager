using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        void TestLowPower()
        {
            if (GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(b => !b.OnlyRecharge).Count() == 0)
                return;

            var power = GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(b => b.IsFunctional && !b.OnlyRecharge && b.Enabled).Average(b => b.CurrentStoredPower / b.MaxStoredPower);

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
