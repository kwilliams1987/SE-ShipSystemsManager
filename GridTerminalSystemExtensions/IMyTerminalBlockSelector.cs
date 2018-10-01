using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    static class IMyTerminalBlockSelectorExtensions
    {
        public static IEnumerable<String> GetFunctions(this IMyTerminalBlock block)
        {
            return block.GetConfigs("functions");
        }

        public static Boolean HasFunction(this IMyTerminalBlock block, String function)
        {
            return block.GetFunctions().Contains(function);
        }

        public static IEnumerable<String> GetZones(this IMyTerminalBlock block)
        {
            return block.GetConfigs("zones");
        }

        public static Boolean IsInZone(this IMyTerminalBlock block, String zone)
        {
            return block.GetZones().Contains(zone);
        }

        public static Boolean IsInAnyZone(this IMyTerminalBlock block, params String[] zones)
        {
            return block.GetZones().Any(z => zones.Contains(z));
        }

        public static Boolean IsInAllZones(this IMyTerminalBlock block, params String[] zones)
        {
            return block.GetZones().All(z => zones.Contains(z));
        }
    }
}
