using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        private void TestAirVents(String zone)
        {
            var vents = GetBlocks<IMyAirVent>(v => v.IsWorking && v.InZone(zone) && !v.Depressurize);
            var blocks = new List<IMyTerminalBlock>()
                            .Concat(GetZoneBlocks<IMyDoor>(zone, BlockType.Airlock, true))
                            .Concat(GetZoneBlocks<IMyTextPanel>(zone, BlockType.DoorSign))
                            .Concat(GetZoneBlocks<IMyTextPanel>(zone, BlockType.Warning))
                            .Concat(GetZoneBlocks<IMySoundBlock>(zone, BlockType.Siren))
                            .Concat(GridTerminalSystem.GetZoneBlocks<IMyInteriorLight>(zone));

            if (vents.Any(v => !v.CanPressurize))
            {
                Output($"Depressurization detected in {vents.Count()} Air Vents in zone {zone}.");

                SetStates(blocks, BlockState.Decompression);

                foreach (var door in blocks.OfType<IMyDoor>().Where(d => d.Enabled)) // These doors are not locked yet.
                    GridStorage.Set(BlockKey(door), "state-changed", true);
            }
            else
            {
                ClearStates(blocks.Where(b => b.GetZones().All(z => !GetBlocks<IMyAirVent>(v => v.InZone(z) && !v.CanPressurize).Any())), BlockState.Decompression);
            }
        }
    }
}
