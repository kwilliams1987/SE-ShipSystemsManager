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
        void TestSensors(String zone)
        {
            var sensors = GetBlocks<IMySensorBlock>(s => s.IsWorking && s.InZone(zone) && s.DetectEnemy);
            var blocks = new List<IMyTerminalBlock>()
                                .Concat(GetZoneBlocks<IMyDoor>(zone, Function.Security, true))
                                .Concat(GetZoneBlocks<IMyTextPanel>(zone, Function.DoorSign))
                                .Concat(GetZoneBlocks<IMyTextPanel>(zone, Function.Warning))
                                .Concat(GetZoneBlocks<IMySoundBlock>(zone, Function.Siren));

            if (sensors.Any(t => t.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
            {
                Output($"Sensor detected enemy in zone {zone}!");

                SetStates(blocks, State.Intruder2);
            }
            else
            {
                ClearStates(blocks.GroupBy(b => b.GetZones())
                    .Where(g => !GetBlocks<IMySensorBlock>(t => t.IsWorking && t.InAnyZone(g.Key.ToArray()) && t.DetectEnemy)
                        .All(t => !t.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
                    .SelectMany(g => g), State.Intruder2);
            }
        }
    }
}
