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
        private void TestSensors(String zone)
        {
            var sensors = GetBlocks<IMySensorBlock>(s => s.IsWorking && s.IsInZone(zone) && s.DetectEnemy);
            var blocks = new List<IMyTerminalBlock>();
            blocks.AddRange(GetZoneBlocks<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true));
            blocks.AddRange(GetZoneBlocks<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR));
            blocks.AddRange(GetZoneBlocks<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING));
            blocks.AddRange(GetZoneBlocks<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN));

            if (sensors.Any(t => t.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
            {
                Output($"Sensor detected enemy in zone {zone}!");

                SetStates(blocks, BlockState.INTRUDER2);
            }
            else
            {
                ClearStates(blocks.GroupBy(b => b.GetZones())
                    .Where(g => !GetBlocks<IMySensorBlock>(t => t.IsWorking && t.IsInAnyZone(g.Key.ToArray()) && t.DetectEnemy)
                        .All(t => !t.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
                    .SelectMany(g => g), BlockState.INTRUDER2);
            }
        }
    }
}
