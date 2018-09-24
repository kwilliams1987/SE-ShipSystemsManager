using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;
using VRage.Game;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        private void TestInteriorWeapons(String zone)
        {
            var interiorTurrets = GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(t => t.IsWorking && t.IsInZone(zone));

            if (interiorTurrets.Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            {
                Output("Turret detected enemy in zone " + zone + "!");

                GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true)
                    .SetStates(BlockState.INTRUDER1);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
                    .SetStates(BlockState.INTRUDER1);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
                    .SetStates(BlockState.INTRUDER1);

                GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
                    .SetStates(BlockState.INTRUDER1);

                return;

                // TODO: Move to Program.ApplyBlockStates
                var doors = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true);
                if (doors.Any())
                {
                    Output("Closing " + doors.Count() + " doors to zone " + zone + ".");

                    doors.ApplyBlockConfigs(new Dictionary<String, Object>()
                    {
                        { "Closed", true }
                    });
                }
                else
                {
                    Output("ALERT: Zone " + zone + " cannot be sealed, no functional doors were found!");
                }

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
                    .ApplyBlockConfigs(new Dictionary<String, Object>()
                {
                    { "FLAGS:state", BlockState.INTRUDER1 },
                    { "PublicText", Configuration.Intruder.ZONE_LABEL },
                    { "FontColor", Configuration.Intruder.SIGN_FOREGROUND_COLOR },
                    { "BackgroundColor", Configuration.Intruder.SIGN_BACKGROUND_COLOR },
                    { "FontSize", 2.9f / Configuration.Intruder.FONTSIZE }
                });
                
                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
                    .ApplyBlockConfigs(new Dictionary<String, Object>()
                {
                    { "FLAGS:state", BlockState.INTRUDER1 },
                    { "Images", Configuration.Intruder.SIGN_IMAGE },
                    { "Enabled", true }
                });

                GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
                    .ApplyBlockConfigs(new Dictionary<String, Object>
                {
                    { "FLAGS:state", BlockState.INTRUDER1 },
                    { "SelectedSound", Configuration.Intruder.ALERT_SOUND },
                    { "LoopPeriod", 3600 },
                    { "Enabled", true },
                    { "Play", true }
                });

            }
            else
            {
                GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true)
                    .GroupBy(d => d.GetZones())
                    .Where(g => !GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(t => t.IsWorking && t.IsInAnyZone(g.Key.ToArray()))
                    .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                    .SelectMany(g => g)
                    .ClearStates(BlockState.INTRUDER1);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
                    .GroupBy(s => s.GetZones())
                    .Where(g => !GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(t => t.IsWorking && t.IsInAnyZone(g.Key.ToArray()))
                    .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                    .SelectMany(g => g)
                    .ClearStates(BlockState.INTRUDER1);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
                    .GroupBy(d => d.GetZones())
                    .Where(g => !GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                    .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                    .SelectMany(g => g)
                    .ClearStates(BlockState.INTRUDER1);

                GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
                    .GroupBy(d => d.GetZones())
                    .Where(g => !GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                    .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                    .SelectMany(g => g)
                    .ClearStates(BlockState.INTRUDER1);

                return;

                // TODO: Move to Program.ApplyBlockStates

                // Group doors by zoning, test each group once and restore only doors in passing zone groups.
                var doorGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK).GroupBy(d => d.GetZones());
                foreach (var group in doorGroups)
                {
                    foreach (var block in group)
                    {
                        block.ClearConfigFlag("state", BlockState.INTRUDER1);
                    }

                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
                    {
                        group.RestoreStates();
                    }
                }

                var doorSignGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR).GroupBy(d => d.GetZones());
                foreach (var group in doorSignGroups)
                {
                    foreach (var block in group)
                    {
                        block.ClearConfigFlag("state", BlockState.INTRUDER1);
                    }

                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
                    {
                        group.RestoreStates();
                    }
                }

                var signGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING).GroupBy(d => d.GetZones());
                foreach (var group in signGroups)
                {
                    foreach (var block in group)
                    {
                        block.ClearConfigFlag("state", BlockState.INTRUDER1);
                    }

                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
                    {
                        group.RestoreStates();
                    }
                }

                var soundBlockGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN).GroupBy(s => s.GetZones());
                foreach (var group in soundBlockGroups)
                {
                    foreach (var block in group)
                    {
                        block.ClearConfigFlag("state", BlockState.INTRUDER1);
                    }

                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize, group.Key.ToArray()))
                    {
                        group.RestoreStates();
                    }
                }
            }
        }
    }
}
