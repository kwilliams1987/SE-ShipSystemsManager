using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using System;
using VRage.Game;

namespace IngameScript
{
    static class IMyGridTerminalSystemExtensions
    {
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
