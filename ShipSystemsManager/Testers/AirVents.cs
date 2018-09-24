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
            var vents = GridTerminalSystem.GetBlocksOfType<IMyAirVent>(v => v.IsWorking && v.IsInZone(zone) && !v.Depressurize);
            if (vents.Any(v => !v.CanPressurize))
            {
                Output("Depressurization detected in " + vents.Count() + " Air Vents.");

                GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true)
                    .SetStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
                    .SetStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
                    .SetStates(BlockState.DECOMPRESSION);

                GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
                    .SetStates(BlockState.DECOMPRESSION);

                return;

                // TODO: Move to Program.ApplyBlockStates
                var doors = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true);
                if (doors.Any())
                {
                    Output("Locking down " + doors.Count() + " doors to zone " + zone + ".");

                    doors.ApplyConfig<IMyDoor>(new Dictionary<String, Object>()
                    {
                        { "Locked", true }
                    });
                }
                else
                {
                    Output("ALERT: Zone " + zone + " cannot be sealed, no functional doors were found!");
                }

                base.GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
                    .ApplyConfig<IMyTextPanel>(new Dictionary<String, Object>()
                {
                    { "FLAGS:state", BlockState.DECOMPRESSION },
                    { "PublicText", Configuration.Decompression.ZONE_LABEL },
                    { "FontColor", Configuration.Decompression.SIGN_FOREGROUND_COLOR },
                    { "BackgroundColor", Configuration.Decompression.SIGN_BACKGROUND_COLOR },
                    { "FontSize", 2.9f / Configuration.Decompression.FONTSIZE }
                });

                base.GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
                    .ApplyConfig<IMyTextPanel>(new Dictionary<String, Object>()
                {
                    { "FLAGS:state", BlockState.DECOMPRESSION },
                    { "Images", Configuration.Decompression.SIGN_IMAGE },
                    { "Enabled", true }
                });

                base.GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
                    .ApplyConfig<IMySoundBlock>(new Dictionary<String, Object>
                {
                    { "FLAGS:state", BlockState.DECOMPRESSION },
                    { "SelectedSound", Configuration.Decompression.ALERT_SOUND },
                    { "LoopPeriod", 3600 },
                    { "Enabled", true },
                    { "Play", true }
                });

                foreach (var group in GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.IsInZone(zone)).GroupBy(l => l.HasFunction(BlockFunction.LIGHT_WARNING)))
                { 
                    if (group.Key)
                    {
                        group.ApplyConfig<IMyLightingBlock>(new Dictionary<String, Object>()
                        {
                            { "Color", Configuration.Decompression.LIGHT_COLOR },
                            { "BlinkIntervalSeconds", Configuration.Decompression.LIGHT_BLINK },
                            { "BlinkLength", Configuration.Decompression.LIGHT_DURATION },
                            { "BlinkOffset", Configuration.Decompression.LIGHT_OFFSET },
                            { "Enabled", true },
                        });
                    }
                    else
                    {
                        group.ApplyConfig<IMyLightingBlock>(new Dictionary<String, Object>()
                        {
                            { "Enabled", false }
                        });
                    }
                }
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

                return;

                // TODO: Move to Program.ApplyBlockStates
                // Group doors by zoning, test each group once and restore only doors in passing zone groups.
                var doorGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK).GroupBy(d => d.GetZones());

                foreach (var group in doorGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
                    {
                        group.RestoreStates();
                    }
                }

                var doorSignGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR).GroupBy(d => d.GetZones());
                foreach (var group in doorSignGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
                    {
                        group.RestoreStates();
                    }
                }

                var signGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING).GroupBy(d => d.GetZones());
                foreach (var group in signGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
                    {
                        group.RestoreStates();
                    }
                }

                var soundBlockGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN).GroupBy(s => s.GetZones());
                foreach (var group in soundBlockGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
                    {
                        group.RestoreStates();
                    }
                }

                var lightGroups = GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.IsInZone(zone)).GroupBy(d => d.GetZones());
                foreach (var group in lightGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
                    {
                        group.RestoreStates();
                    }
                }
            }
        }
    }
}
