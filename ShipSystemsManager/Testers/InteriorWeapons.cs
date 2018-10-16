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
            var interiorTurrets = GetBlocks<IMyLargeInteriorTurret>(t => t.IsWorking && t.InZone(zone));
            var blocks = new List<IMyTerminalBlock>()
                                .Concat(GetZoneBlocks<IMyDoor>(zone, BlockType.Security, true))
                                .Concat(GetZoneBlocks<IMyTextPanel>(zone, BlockType.DoorSign))
                                .Concat(GetZoneBlocks<IMyTextPanel>(zone, BlockType.Warning))
                                .Concat(GetZoneBlocks<IMySoundBlock>(zone, BlockType.Siren));

            if (interiorTurrets.Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            {
                Output($"Turret detected enemy in zone {zone}!");

                SetStates(blocks, BlockState.Intruder1);
            }
            else
            {
                ClearStates(blocks.GroupBy(b => b.GetZones())
                    .Where(g => !GetBlocks<IMyLargeInteriorTurret>(t => t.IsWorking && t.InAnyZone(g.Key.ToArray()))
                        .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                    .SelectMany(g => g), BlockState.Intruder1);
            }
        }
    }
}
