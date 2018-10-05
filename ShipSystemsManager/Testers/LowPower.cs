using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        private void TestLowPower()
        {
            if (GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(b => !b.OnlyRecharge).Count() == 0)
                return;

            var power = GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(b => b.IsFunctional && !b.OnlyRecharge && b.Enabled).Average(b => b.CurrentStoredPower / b.MaxStoredPower);

            if (power < 0.1f)
            {
                GridTerminalSystem.GetBlocksOfType<IMyAssembler>()
                    .SetStates(BlockState.LOWPOWER);
                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>()
                    .SetStates(BlockState.LOWPOWER);
                GridTerminalSystem.GetBlocksOfType<IMyRefinery>()
                    .SetStates(BlockState.LOWPOWER);
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType<IMyAssembler>()
                    .ClearStates(BlockState.LOWPOWER);
                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>()
                    .ClearStates(BlockState.LOWPOWER);
                GridTerminalSystem.GetBlocksOfType<IMyRefinery>()
                    .ClearStates(BlockState.LOWPOWER);
            }
        }
    }
}
