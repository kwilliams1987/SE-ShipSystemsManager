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
            var vents = GetBlocks<IMyAirVent>(v => v.IsWorking && v.IsInZone(zone) && !v.Depressurize);
            var blocks = new List<IMyTerminalBlock>();
            blocks.AddRange(GetZoneBlocks<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true));
            blocks.AddRange(GetZoneBlocks<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR));
            blocks.AddRange(GetZoneBlocks<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING));
            blocks.AddRange(GetZoneBlocks<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN));
            blocks.AddRange(GridTerminalSystem.GetZoneBlocks<IMyInteriorLight>(zone));

            if (vents.Any(v => !v.CanPressurize))
            {
                Output($"Depressurization detected in {vents.Count()} Air Vents in zone {zone}.");

                SetStates(blocks, BlockState.DECOMPRESSION);

                foreach (var door in blocks.OfType<IMyDoor>().Where(d => d.Enabled)) // These doors are not locked yet.
                    GridStorage.Set(BlockKey(door), "state-changed", true);
            }
            else
            {
                ClearStates(blocks.Where(b => b.GetZones().All(z => !GetBlocks<IMyAirVent>(v => v.IsInZone(z) && !v.CanPressurize).Any())), BlockState.DECOMPRESSION);
            }
        }
    }
}
