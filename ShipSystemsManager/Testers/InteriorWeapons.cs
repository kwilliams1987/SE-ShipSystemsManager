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
            }
        }
    }
}
