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
            var interiorTurrets = GetBlocks<IMyLargeInteriorTurret>(t => t.IsWorking);
            var zoneTurrets = interiorTurrets.Where(t => GetConfig(t).InZone(zone));

            var blocks = new List<IMyTerminalBlock>()
                                .Concat(GetZoneBlocksByFunction<IMyDoor>(zone, Function.Security, true))
                                .Concat(GetZoneBlocksByFunction<IMyTextPanel>(zone, Function.DoorSign))
                                .Concat(GetZoneBlocksByFunction<IMyTextPanel>(zone, Function.Warning))
                                .Concat(GetZoneBlocksByFunction<IMySoundBlock>(zone, Function.Siren));

            if (zoneTurrets.Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            {
                Output($"Turret detected enemy in zone {zone}!");

                SetStates(blocks, State.Intruder1);
            }
            else
            {
                ClearStates(blocks.GroupBy(b => GetConfig(b).GetZones())
                    .Where(g => !interiorTurrets.Where(t => GetConfig(t).InAnyZone(g.Key.ToArray()))
                        .Any(t => t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                    .SelectMany(g => g), State.Intruder1);
            }
        }
    }
}
