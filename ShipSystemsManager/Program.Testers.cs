using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game;

namespace IngameScript
{
    public partial class Program
    {
        private Boolean TestDecompression(String zone, IEnumerable<Block<IMyTerminalBlock>> blocks)
            => blocks.OfType<Block<IMyAirVent>>().Select(b => b.Target).Any(v => v.IsFunctional && !v.CanPressurize);

        private Boolean TestIntruder(String zone, IEnumerable<Block<IMyTerminalBlock>> blocks)
        {
            var terminals = blocks.Select(b => b.Target);
            var turrets = terminals.OfType<IMyLargeInteriorTurret>();

            if (turrets.Any(t => t.IsFunctional && t.HasTarget && t.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                return true;

            var sensors = terminals.OfType<IMySensorBlock>();
            var entities = new List<MyDetectedEntityInfo>();
            foreach (var sensor in sensors.Where(s => s.IsFunctional && s.DetectEnemy))
            {
                sensor.DetectedEntities(entities);
                if (entities.Any(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                    return true;
            }

            return false;
        }

        private Boolean TestLowPower(IEnumerable<Block<IMyTerminalBlock>> blocks)
        {
            var batteries = blocks.OfType<Block<IMyBatteryBlock>>().Select(b => b.Target)
                    .Where(b => b.ChargeMode == ChargeMode.Auto || b.ChargeMode == ChargeMode.Discharge);

            if (!batteries.Any())
                return false;

            return batteries.Average(b => b.CurrentStoredPower / b.MaxStoredPower) < PowerThreshold;
        }
    }
}
