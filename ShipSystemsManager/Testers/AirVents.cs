using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;

namespace IngameScript
{
    partial class Program
    {
        private void TestAirVents(String zone)
        {
            var vents = GridTerminalSystem.GetBlocksOfType<IMyAirVent>(v => v.IsWorking && v.IsInZone(zone) && !v.Depressurize);
            if (vents.Any(v => !v.CanPressurize))
            {
                Output($"Depressurization detected in {vents.Count()} Air Vents in zone {zone}.");

                GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true)
                    .SetStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
                    .SetStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
                    .SetStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
                    .SetStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocks<IMyInteriorLight>(zone)
                    .SetStates(BlockState.DECOMPRESSION);

                return;
            }
            else
            {
                GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true)
                    .ClearStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
                    .ClearStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
                    .ClearStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
                    .ClearStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocks<IMyInteriorLight>(zone)
                    .ClearStates(BlockState.DECOMPRESSION);
                return;
            }
        }
    }
}
