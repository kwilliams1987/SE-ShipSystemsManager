using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using System;
using VRage.Game;

namespace IngameScript
{
    static class IMyGridTerminalSystemExtensions
    {
        public static IEnumerable<T> GetBlocksOfType<T>(this IMyGridTerminalSystem grid, Func<T, Boolean> collect = null)
            where T : class, IMyTerminalBlock
        {
            var result = new List<T>();
            grid.GetBlocksOfType(result, collect);
            return result;
        }
        
        public static IEnumerable<T> GetZoneBlocks<T>(this IMyGridTerminalSystem grid, String zone, Boolean all = false)
            where T : class, IMyTerminalBlock
        {
            return grid.GetBlocksOfType<T>(p => (p.IsWorking || all) && p.IsInZone(zone));
        }

        public static IEnumerable<T> GetZoneBlocksByFunction<T>(this IMyGridTerminalSystem grid, String zone, String function, Boolean all = false)
            where T : class, IMyTerminalBlock
        {
            return grid.GetZoneBlocks<T>(zone, all).Where(p => p.IsA(function));
        }

        public static IEnumerable<String> GetZones(this IMyGridTerminalSystem grid)
        {
            return grid.GetBlocksOfType<IMyTerminalBlock>().SelectMany(b => b.GetZones()).Distinct();
        }

        public static Boolean AdjacentZonesTest<T>(this IMyGridTerminalSystem grid, Func<T, Boolean> test, params String[] zones)
            where T : class, IMyTerminalBlock
        {
            return grid.GetBlocksOfType<T>(v => v.IsInAnyZone(zones)).All(test);
        }

        public static List<MyDetectedEntityInfo> GetDetectedEntities(this IMySensorBlock sensorBlock, Func<MyDetectedEntityInfo, Boolean> collect = null)
        {
            var result = new List<MyDetectedEntityInfo>();
            sensorBlock.DetectedEntities(result);

            if (collect == null)
            {
                return result;
            }
            else
            {
                return result.Where(collect).ToList();
            }
        }
    }
}
