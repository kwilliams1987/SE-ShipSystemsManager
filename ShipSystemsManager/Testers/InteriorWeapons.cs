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
            var interiorTurrets = GetBlocks<IMyLargeInteriorTurret>(t => t.IsWorking && t.IsInZone(zone));
            var blocks = new List<IMyTerminalBlock>();
            blocks.AddRange(GetZoneBlocks<IMyDoor>(zone, BlockFunction.DOOR_SECURITY, true));
            blocks.AddRange(GetZoneBlocks<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR));
            blocks.AddRange(GetZoneBlocks<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING));
            blocks.AddRange(GetZoneBlocks<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN));

            if (interiorTurrets.Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            {
                Output($"Turret detected enemy in zone {zone}!");

                blocks.SetStates(BlockState.INTRUDER1);
            }
            else
            {
                blocks.GroupBy(b => b.GetZones())
                    .Where(g => !GetBlocks<IMyLargeInteriorTurret>(t => t.IsWorking && t.IsInAnyZone(g.Key.ToArray()))
                        .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                    .SelectMany(g => g)
                    .ClearStates(BlockState.INTRUDER1);
            }
        }
    }
}
