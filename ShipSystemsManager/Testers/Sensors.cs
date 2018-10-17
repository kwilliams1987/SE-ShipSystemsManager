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
            var sensors = GetBlocks<IMySensorBlock>(s => s.IsWorking && s.DetectEnemy);
            var zoneSensors = sensors.Where(s => GetConfig(s).InZone(zone));
            var blocks = new List<IMyTerminalBlock>()
                                .Concat(GetZoneBlocksByFunction<IMyDoor>(zone, Function.Security, true))
                                .Concat(GetZoneBlocksByFunction<IMyTextPanel>(zone, Function.DoorSign))
                                .Concat(GetZoneBlocksByFunction<IMyTextPanel>(zone, Function.Warning))
                                .Concat(GetZoneBlocksByFunction<IMySoundBlock>(zone, Function.Siren));

            if (sensors.Any(t => t.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
            {
                Output($"Sensor detected enemy in zone {zone}!");

                SetStates(blocks, State.Intruder2);
            }
            else
            {
                ClearStates(blocks.GroupBy(b => GetConfig(b).GetZones())
                    .Where(g => sensors.Where(s => GetConfig(s).InAnyZone(g.Key.ToArray()))
                        .All(t => !t.GetDetectedEntities(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies).Any()))
                    .SelectMany(g => g), State.Intruder2);
            }
        }
    }
}
