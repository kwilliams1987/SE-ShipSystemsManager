using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        private void TestBattleStations()
        {
            if (Me.HasConfigFlag("custom-states", "battle"))
            {
                GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY))
                    .SetStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_BATTLE))
                    .SetStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING))
                    .SetStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
                    .SetStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.HasFunction(BlockFunction.LIGHT_WARNING))
                    .SetStates(BlockState.BATTLESTATIONS);
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY))
                    .ClearStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_BATTLE))
                    .ClearStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING))
                    .ClearStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
                    .ClearStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.HasFunction(BlockFunction.LIGHT_WARNING))
                    .ClearStates(BlockState.BATTLESTATIONS);
            }
        }
    }
}
