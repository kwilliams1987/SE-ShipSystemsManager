using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;
using VRage.Game;

namespace IngameScript
{
    partial class Program
    {
        private void TestSensors(String zone)
        {
            var sensors = GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInZone(zone) && s.DetectEnemy);

            if (sensors.Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
            {
                Output($"Sensor detected enemy in zone {zone}!");

                GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true)
                    .SetStates(BlockState.INTRUDER2);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
                    .SetStates(BlockState.INTRUDER2);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
                    .SetStates(BlockState.INTRUDER2);

                GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
                    .SetStates(BlockState.INTRUDER2);
            }
            else
            {
                GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true)
                    .GroupBy(d => d.GetZones())
                    .Where(g => !GridTerminalSystem
                        .GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                        .Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
                    .SelectMany(g => g)
                    .ClearStates(BlockState.INTRUDER2);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR)
                    .GroupBy(d => d.GetZones())
                    .Where(g => !GridTerminalSystem
                        .GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                        .Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
                    .SelectMany(g => g)
                    .ClearStates(BlockState.INTRUDER2);

                GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING)
                    .GroupBy(d => d.GetZones())
                    .Where(g => !GridTerminalSystem
                        .GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                        .Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
                    .SelectMany(g => g)
                    .ClearStates(BlockState.INTRUDER2);

                GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN)
                    .GroupBy(d => d.GetZones())
                    .Where(g => !GridTerminalSystem
                        .GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInAnyZone(g.Key.ToArray()))
                        .Any(s => s.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
                    .SelectMany(g => g)
                    .ClearStates(BlockState.INTRUDER2);
            }
        }
    }
}
