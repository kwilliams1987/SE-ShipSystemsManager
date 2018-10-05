using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        private void TestSelfDestruct()
        {
            if (Me.HasConfigFlag("custom-states", "destruct"))
            {
                GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY))
                    .SetStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_BATTLE))
                    .SetStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING))
                    .SetStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
                    .SetStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>()
                    .SetStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMyWarhead>(w => w.HasFunction(BlockFunction.WARHEAD_DESTRUCT))
                    .SetStates(BlockState.SELFDESTRUCT);
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY))
                    .ClearStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_BATTLE))
                    .ClearStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING))
                    .ClearStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
                    .ClearStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>()
                    .ClearStates(BlockState.SELFDESTRUCT);
                GridTerminalSystem.GetBlocksOfType<IMyWarhead>(w => w.HasFunction(BlockFunction.WARHEAD_DESTRUCT))
                    .ClearStates(BlockState.SELFDESTRUCT);
            }
        }
    }
}
