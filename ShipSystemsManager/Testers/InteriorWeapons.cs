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
        void TestInteriorWeapons(String zone)
        {
            var interiorTurrets = GetBlocks<IMyLargeInteriorTurret>(t => t.IsWorking && t.InZone(zone));
            var blocks = new List<IMyTerminalBlock>()
                                .Concat(GetZoneBlocks<IMyDoor>(zone, Function.Security, true))
                                .Concat(GetZoneBlocks<IMyTextPanel>(zone, Function.DoorSign))
                                .Concat(GetZoneBlocks<IMyTextPanel>(zone, Function.Warning))
                                .Concat(GetZoneBlocks<IMySoundBlock>(zone, Function.Siren));

            if (interiorTurrets.Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            {
                Output($"Turret detected enemy in zone {zone}!");

                SetStates(blocks, State.Intruder1);
            }
            else
            {
                ClearStates(blocks.GroupBy(b => b.GetZones())
                    .Where(g => !GetBlocks<IMyLargeInteriorTurret>(t => t.IsWorking && t.InAnyZone(g.Key.ToArray()))
                        .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                    .SelectMany(g => g), State.Intruder1);
            }
        }
    }
}
