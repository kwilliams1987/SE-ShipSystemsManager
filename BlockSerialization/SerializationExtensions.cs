using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public static class SerializationExtensions
    {
        private static Serialization.IMyTerminalBlockSerializer GetSerializer<T>(T block)
             where T : IMyTerminalBlock
        {
            var serializer = default(Serialization.IMyTerminalBlockSerializer);

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyAssembler)
                serializer = new Serialization.IMyAssemblerSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyBatteryBlock)
                serializer = new Serialization.IMyBatteryBlockSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyCameraBlock)
                serializer = new Serialization.IMyCameraBlockSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyCockpit)
                serializer = new Serialization.IMyCockpitSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyCollector)
                serializer = new Serialization.IMyCollectorSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyConveyorSorter)
                serializer = new Serialization.IMyConveyorSorterSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyDoor)
                serializer = new Serialization.IMyDoorSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyGyro)
                serializer = new Serialization.IMyGyroSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyLargeTurretBase)
                serializer = new Serialization.IMyLargeTurretBaseSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyLaserAntenna)
                serializer = new Serialization.IMyLaserAntennaSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyLightingBlock)
                serializer = new Serialization.IMyLightingBlockSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyFunctionalBlock)
                serializer = new Serialization.IMyFunctionalBlockSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer))
                serializer = new Serialization.IMyTerminalBlockSerializer<T>();

            return serializer;
        }

        public static void RestoreConfig<T>(this T block)
             where T : IMyTerminalBlock => GetSerializer(block).RestoreState(block);

        public static void ApplyConfig<T>(this T block, Dictionary<String, Object> configValues)
             where T : IMyTerminalBlock => GetSerializer(block).SetState(block, configValues);

        public static void ApplyConfig<T>(this IEnumerable<T> blocks, Dictionary<String, Object> configValues)
            where T : IMyTerminalBlock
        {
            var serializer = GetSerializer(blocks.FirstOrDefault());
            foreach (var block in blocks)
            {
                serializer.SetState(block, configValues);
            }
        }
    }
}
