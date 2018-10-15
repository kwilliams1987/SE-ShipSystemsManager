using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public static class SerializationExtensions
    {
        private static Serialization.IMyTerminalBlockSerializer GetSerializer<T>(T block)
             where T : IMyTerminalBlock
        {
            if (block is IMyAssembler)
                return new Serialization.IMyAssemblerSerializer();

            if (block is IMyBatteryBlock)
                return new Serialization.IMyBatteryBlockSerializer();

            if (block is IMyCameraBlock)
                return new Serialization.IMyCameraBlockSerializer();

            if (block is IMyCockpit)
                return new Serialization.IMyCockpitSerializer();

            if (block is IMyCollector)
                return new Serialization.IMyCollectorSerializer();

            if (block is IMyConveyorSorter)
                return new Serialization.IMyConveyorSorterSerializer();

            if (block is IMyDoor)
                return new Serialization.IMyDoorSerializer();

            if (block is IMyGasGenerator)
                return new Serialization.IMyGasGeneratorSerializer();

            if (block is IMyGasTank)
                return new Serialization.IMyGasTankSerializer();

            if (block is IMyGyro)
                return new Serialization.IMyGyroSerializer();

            if (block is IMyGravityGenerator)
                return new Serialization.IMyGravityGeneratorSerializer();

            if (block is IMyGravityGeneratorSphere)
                return new Serialization.IMyGravityGeneratorSphereSerializer();

            if (block is IMyLargeTurretBase)
                return new Serialization.IMyLargeTurretBaseSerializer();

            if (block is IMyLaserAntenna)
                return new Serialization.IMyLaserAntennaSerializer();

            if (block is IMyLightingBlock)
                return new Serialization.IMyLightingBlockSerializer();

            if (block is IMyMotorStator)
                return new Serialization.IMyMotorStatorSerializer();

            if (block is IMyOreDetector)
                return new Serialization.IMyOreDetectorSerializer();

            if (block is IMyPistonBase)
                return new Serialization.IMyPistonBaseSerializer();

            if (block is IMyRadioAntenna)
                return new Serialization.IMyRadioAntennaSerializer();

            if (block is IMyReactor)
                return new Serialization.IMyReactorSerializer();

            if (block is IMySensorBlock)
                return new Serialization.IMySensorBlockSerializer();

            if (block is IMyShipConnector)
                return new Serialization.IMyShipConnectorSerializer();

            if (block is IMyShipDrill)
                return new Serialization.IMyShipDrillSerializer();

            if (block is IMyShipWelder)
                return new Serialization.IMyShipWelderSerializer();

            if (block is IMySmallGatlingGun)
                return new Serialization.IMySmallGatlingGunSerializer();

            if (block is IMySmallMissileLauncher)
                return new Serialization.IMySmallMissileLauncherSerializer();

            if (block is IMySoundBlock)
                return new Serialization.IMySoundBlockSerializer();

            if (block is IMyTextPanel)
                return new Serialization.IMyTextPanelSerializer();

            if (block is IMyThrust)
                return new Serialization.IMyThrustSerializer();

            if (block is IMyWarhead)
                return new Serialization.IMyWarheadSerializer();

            if (block is IMyFunctionalBlock)
                return new Serialization.IMyFunctionalBlockSerializer();

            return new Serialization.IMyTerminalBlockSerializer<T>();
        }

        public static void RestoreConfig<T>(this T block, MyIni storage)
             where T : IMyTerminalBlock => GetSerializer(block).RestoreState(block, storage);

        public static void ApplyConfig<T>(this T block, Dictionary<String, Object> configValues, MyIni storage)
             where T : IMyTerminalBlock => GetSerializer(block).SetState(block, configValues, storage);

        public static void ApplyConfig<T>(this IEnumerable<T> blocks, Dictionary<String, Object> configValues, MyIni storage)
            where T : IMyTerminalBlock
        {
            var serializer = GetSerializer(blocks.FirstOrDefault());
            foreach (var block in blocks)
            {
                serializer.SetState(block, configValues, storage);
            }
        }
    }
}
