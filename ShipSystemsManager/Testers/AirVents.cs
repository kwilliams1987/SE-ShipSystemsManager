using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        void TestAirVents(String zone)
        {
            var vents = GetBlocks<IMyAirVent>(v => v.IsWorking && GetConfig(v).InZone(zone) && !v.Depressurize);
            var blocks = new List<IMyTerminalBlock>()
                            .Concat(GetZoneBlocksByFunction<IMyDoor>(zone, Function.Airlock, true))
                            .Concat(GetZoneBlocksByFunction<IMyTextPanel>(zone, Function.DoorSign))
                            .Concat(GetZoneBlocksByFunction<IMyTextPanel>(zone, Function.Warning))
                            .Concat(GetZoneBlocksByFunction<IMySoundBlock>(zone, Function.Siren))
                            .Concat(GetZoneBlocks<IMyInteriorLight>(zone));

            if (vents.Any(v => !v.CanPressurize))
            {
                Output($"Depressurization detected in {vents.Count()} Air Vents in zone {zone}.");

                SetStates(blocks, State.Decompression);

                foreach (var door in blocks.OfType<IMyDoor>().Where(d => d.Enabled)) // These doors are not locked yet.
                    GridStorage.Set(BlockKey(door), "state-changed", true);
            }
            else
            {
                ClearStates(blocks.Where(b => GetConfig(b).GetZones().All(z => !GetBlocks<IMyAirVent>(v => GetConfig(v).InZone(z) && !v.CanPressurize).Any())), State.Decompression);
            }
        }
    }
}
