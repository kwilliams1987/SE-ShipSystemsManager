using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public static class SerializationExtensions
    {
        private static Serialization.IMyTerminalBlockSerializer GetSerializer<T>()
             where T : IMyTerminalBlock
        {
            var serializer = default(Serialization.IMyTerminalBlockSerializer);

            if (typeof(T) == typeof(IMyAssembler))
                serializer = new Serialization.IMyAssemblerSerializer();

            if (typeof(T) == typeof(IMyBatteryBlock))
                serializer = new Serialization.IMyBatteryBlockSerializer();

            if (typeof(T) == typeof(IMyCameraBlock))
                serializer = new Serialization.IMyCameraBlockSerializer();

            if (typeof(T) == typeof(IMyCockpit))
                serializer = new Serialization.IMyCockpitSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer))
                serializer = new Serialization.IMyTerminalBlockSerializer<T>();

            return serializer;
        }

        public static void RestoreConfig<T>(this T block)
             where T : IMyTerminalBlock => GetSerializer<T>().RestoreState(block);

        public static void ApplyConfig<T>(this T block, Dictionary<String, Object> configValues)
             where T : IMyTerminalBlock => GetSerializer<T>().SetState(block, configValues);

        public static void ApplyConfig<T>(this IEnumerable<T> blocks, Dictionary<String, Object> configValues)
            where T : IMyTerminalBlock
        {
            var serializer = GetSerializer<T>();
            foreach (var block in blocks)
            {
                serializer.SetState(block, configValues);
            }
        }
    }
}
