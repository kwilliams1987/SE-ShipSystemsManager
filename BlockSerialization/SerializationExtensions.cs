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
        private static Serializer.IMyTerminalBlockSerializer GetSerializer<T>(T block)
             where T : IMyTerminalBlock
        {
            if (block is IMyAssembler)
                return new Serializer.Assembler();

            if (block is IMyBatteryBlock)
                return new Serializer.BatteryBlock();

            if (block is IMyCameraBlock)
                return new Serializer.CameraBlock();

            if (block is IMyCockpit)
                return new Serializer.Cockpit();

            if (block is IMyCollector)
                return new Serializer.Collector();

            if (block is IMyConveyorSorter)
                return new Serializer.ConveyorSorter();

            if (block is IMyDoor)
                return new Serializer.Door();

            if (block is IMyGasGenerator)
                return new Serializer.GasGenerator();

            if (block is IMyGasTank)
                return new Serializer.GasTank();

            if (block is IMyGyro)
                return new Serializer.Gyro();

            if (block is IMyGravityGenerator)
                return new Serializer.GravityGenerator();

            if (block is IMyGravityGeneratorSphere)
                return new Serializer.GravityGeneratorSphere();

            if (block is IMyLargeTurretBase)
                return new Serializer.LargeThrusterBase();

            if (block is IMyLaserAntenna)
                return new Serializer.LaserAntenna();

            if (block is IMyLightingBlock)
                return new Serializer.LightingBlock();

            if (block is IMyMotorStator)
                return new Serializer.MotorStator();

            if (block is IMyOreDetector)
                return new Serializer.OreDetector();

            if (block is IMyPistonBase)
                return new Serializer.PistonBase();

            if (block is IMyRadioAntenna)
                return new Serializer.RadioAntenna();

            if (block is IMyReactor)
                return new Serializer.Reactor();

            if (block is IMySensorBlock)
                return new Serializer.SensorBlock();

            if (block is IMyShipConnector)
                return new Serializer.ShipConnector();

            if (block is IMyShipDrill)
                return new Serializer.ShipDrill();

            if (block is IMyShipWelder)
                return new Serializer.ShipWelder();

            if (block is IMySmallGatlingGun)
                return new Serializer.SmallGatlingGun();

            if (block is IMySmallMissileLauncher)
                return new Serializer.SmallMissileLauncher();

            if (block is IMySoundBlock)
                return new Serializer.SoundBlock();

            if (block is IMyTextPanel)
                return new Serializer.TextPanel();

            if (block is IMyThrust)
                return new Serializer.Thrust();

            if (block is IMyWarhead)
                return new Serializer.Warhead();

            if (block is IMyFunctionalBlock)
                return new Serializer.FunctionalBlock();

            return new Serializer.TerminalBlock<T>();
        }

        public static void Restore<T>(this T block, MyIni storage)
             where T : IMyTerminalBlock => GetSerializer(block).RestoreState(block, storage);

        public static void Apply<T>(this T block, Dictionary<String, Object> configValues, MyIni storage)
             where T : IMyTerminalBlock => GetSerializer(block).SetState(block, configValues, storage);

        public static void Apply<T>(this IEnumerable<T> blocks, Dictionary<String, Object> configValues, MyIni storage)
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
