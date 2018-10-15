using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    static class IMyTerminalBlockSelectorExtensions
    {
        const String Functions = "functions";
        const String Zones = "zones";

        public static IEnumerable<String> GetFunctions(this IMyTerminalBlock block)
        {
            using (var config = block.GetConfig())
                return config.GetValues(Functions);
        }

        public static IEnumerable<String> GetZones(this IMyTerminalBlock block)
        {
            using (var config = block.GetConfig())
                return config.GetValues(Zones).Where(z => z != "");
        }

        public static Boolean HasFunction(this IMyTerminalBlock block, String function)
            => block.GetFunctions().Contains(function);

        public static Boolean IsInZone(this IMyTerminalBlock block, String zone)
            => block.GetZones().Contains(zone);

        public static Boolean IsInAnyZone(this IMyTerminalBlock block, params String[] zones)
            => block.GetZones().Any(z => zones.Contains(z));

        public static Boolean IsInAllZones(this IMyTerminalBlock block, params String[] zones)
            => block.GetZones().All(z => zones.Contains(z));

        public static MyConfig GetConfig(this IMyTerminalBlock block) 
            => new MyConfig(block);
    }
}
