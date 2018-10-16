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
        
        public static IEnumerable<String> GetZones(this IMyTerminalBlock block)
        {
            using (var config = block.GetConfig())
                return config.GetValues(Zones).Where(z => z != "");
        }

        public static Boolean IsA(this IMyTerminalBlock block, String function)
        {
            using (var config = block.GetConfig())
                return config.GetValues(Functions).Contains(function);
        }

        public static Boolean InZone(this IMyTerminalBlock block, String zone)
            => block.GetZones().Contains(zone);

        public static Boolean InAnyZone(this IMyTerminalBlock block, params String[] zones)
            => block.GetZones().Any(z => zones.Contains(z));

        public static Boolean InAllZones(this IMyTerminalBlock block, params String[] zones)
            => block.GetZones().All(z => zones.Contains(z));

        public static MyConfig GetConfig(this IMyTerminalBlock block) 
            => new MyConfig(block);
    }
}
