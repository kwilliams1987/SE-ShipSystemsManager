using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
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

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyGasGenerator)
                serializer = new Serialization.IMyGasGeneratorSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyGasTank)
                serializer = new Serialization.IMyGasTankSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyGyro)
                serializer = new Serialization.IMyGyroSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyGravityGenerator)
                serializer = new Serialization.IMyGravityGeneratorSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyGravityGeneratorSphere)
                serializer = new Serialization.IMyGravityGeneratorSphereSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyLargeTurretBase)
                serializer = new Serialization.IMyLargeTurretBaseSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyLaserAntenna)
                serializer = new Serialization.IMyLaserAntennaSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyLightingBlock)
                serializer = new Serialization.IMyLightingBlockSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyMotorStator)
                serializer = new Serialization.IMyMotorStatorSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyOreDetector)
                serializer = new Serialization.IMyOreDetectorSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyPistonBase)
                serializer = new Serialization.IMyPistonBaseSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyRadioAntenna)
                serializer = new Serialization.IMyRadioAntennaSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyReactor)
                serializer = new Serialization.IMyReactorSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMySensorBlock)
                serializer = new Serialization.IMySensorBlockSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyShipConnector)
                serializer = new Serialization.IMyShipConnectorSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyShipDrill)
                serializer = new Serialization.IMyShipDrillSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyShipWelder)
                serializer = new Serialization.IMyShipWelderSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMySmallGatlingGun)
                serializer = new Serialization.IMySmallGatlingGunSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMySmallMissileLauncher)
                serializer = new Serialization.IMySmallMissileLauncherSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMySoundBlock)
                serializer = new Serialization.IMySoundBlockSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyTextPanel)
                serializer = new Serialization.IMyTextPanelSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyThrust)
                serializer = new Serialization.IMyThrustSerializer();

            if (serializer == default(Serialization.IMyTerminalBlockSerializer) && block is IMyWarhead)
                serializer = new Serialization.IMyWarheadSerializer();

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
