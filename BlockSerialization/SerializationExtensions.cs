using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public static class Serializer
    {
        public static class Custom
        {
            public const String Images = nameof(Images);
            public const String Closed = nameof(Closed);
            public const String Locked = nameof(Locked);
            public const String PublicText = nameof(PublicText);
            public const String PublicTitle = nameof(PublicTitle);
            public const String Countdown = nameof(Countdown);
            public const String FilterList = nameof(FilterList);
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

        static ISerializer GetSerializer<T>(T block)
             where T : IMyTerminalBlock
        {
            if (block is IMyAssembler) return new Assembler();
            if (block is IMyBatteryBlock) return new BatteryBlock();
            if (block is IMyCameraBlock) return new CameraBlock();
            if (block is IMyCockpit) return new Cockpit();
            if (block is IMyCollector) return new Collector();
            if (block is IMyConveyorSorter) return new ConveyorSorter();
            if (block is IMyDoor) return new Door();
            if (block is IMyGasGenerator) return new GasGenerator();
            if (block is IMyGasTank) return new GasTank();
            if (block is IMyGyro) return new Gyro();
            if (block is IMyGravityGenerator) return new GravityGenerator();
            if (block is IMyGravityGeneratorSphere) return new GravityGeneratorSphere();
            if (block is IMyLargeTurretBase) return new LargeThrusterBase();
            if (block is IMyLaserAntenna) return new LaserAntenna();
            if (block is IMyLightingBlock) return new LightingBlock();
            if (block is IMyMotorStator) return new MotorStator();
            if (block is IMyOreDetector) return new OreDetector();
            if (block is IMyPistonBase) return new PistonBase();
            if (block is IMyRadioAntenna) return new RadioAntenna();
            if (block is IMyReactor) return new Reactor();
            if (block is IMySensorBlock) return new SensorBlock();
            if (block is IMyShipConnector) return new ShipConnector();
            if (block is IMyShipDrill) return new ShipDrill();
            if (block is IMyShipWelder) return new ShipWelder();
            if (block is IMySmallGatlingGun) return new SmallGatlingGun();
            if (block is IMySmallMissileLauncher) return new SmallMissileLauncher();
            if (block is IMySoundBlock) return new SoundBlock();
            if (block is IMyTextPanel) return new TextPanel();
            if (block is IMyThrust) return new Thrust();
            if (block is IMyWarhead) return new Warhead();
            if (block is IMyFunctionalBlock) return new FunctionalBlock();

            return new TerminalBlock<T>();
        }

        #region Serializers
        public class Assembler : FunctionalBlock<IMyAssembler>
        {
            protected override void Serialize(IMyAssembler block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.CooperativeMode), block.CooperativeMode);
                values.Add(nameof(block.Mode), block.Mode.ToString());
                values.Add(nameof(block.Repeating), block.Repeating);
                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMyAssembler block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.CooperativeMode): block.CooperativeMode = Convert.ToBoolean(value.Value); break;
                        case nameof(block.Mode): block.Mode = (MyAssemblerMode)Enum.Parse(typeof(MyAssemblerMode), value.Value.ToString()); break;
                        case nameof(block.Repeating): block.Repeating = Convert.ToBoolean(value.Value); break;
                        case nameof(block.UseConveyorSystem): block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class BatteryBlock : FunctionalBlock<IMyBatteryBlock>
        {
            protected override void Serialize(IMyBatteryBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.OnlyDischarge), block.OnlyDischarge);
                values.Add(nameof(block.OnlyRecharge), block.OnlyRecharge);
                values.Add(nameof(block.SemiautoEnabled), block.SemiautoEnabled);
            }

            protected override void Deserialize(IMyBatteryBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.OnlyDischarge): block.OnlyDischarge = Convert.ToBoolean(value.Value); break;
                        case nameof(block.OnlyRecharge): block.OnlyRecharge = Convert.ToBoolean(value.Value); break;
                        case nameof(block.SemiautoEnabled): block.SemiautoEnabled = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class Beacon : FunctionalBlock<IMyBeacon>
        {
            protected override void Serialize(IMyBeacon block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.Radius), block.Radius);
            }

            protected override void Deserialize(IMyBeacon block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.Radius): block.Radius = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }

        public class CameraBlock : FunctionalBlock<IMyCameraBlock>
        {
            protected override void Serialize(IMyCameraBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.EnableRaycast), block.EnableRaycast);
            }

            protected override void Deserialize(IMyCameraBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.EnableRaycast): block.EnableRaycast = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class Cockpit : TerminalBlock<IMyCockpit>
        {
            protected override void Serialize(IMyCockpit block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.ControlThrusters), block.ControlThrusters);
                values.Add(nameof(block.ControlWheels), block.ControlWheels);
                values.Add(nameof(block.DampenersOverride), block.DampenersOverride);
                values.Add(nameof(block.HandBrake), block.HandBrake);
                values.Add(nameof(block.IsMainCockpit), block.IsMainCockpit);
                values.Add(nameof(block.ShowHorizonIndicator), block.ShowHorizonIndicator);
            }

            protected override void Deserialize(IMyCockpit block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.ControlThrusters): block.ControlThrusters = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ControlWheels): block.ControlWheels = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DampenersOverride): block.DampenersOverride = Convert.ToBoolean(value.Value); break;
                        case nameof(block.HandBrake): block.HandBrake = Convert.ToBoolean(value.Value); break;
                        case nameof(block.IsMainCockpit): block.IsMainCockpit = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ShowHorizonIndicator): block.ShowHorizonIndicator = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class Collector : FunctionalBlock<IMyCollector>
        {
            protected override void Serialize(IMyCollector block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMyCollector block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.UseConveyorSystem): block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class ConveyorSorter : FunctionalBlock<IMyConveyorSorter>
        {
            protected override void Serialize(IMyConveyorSorter block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.DrainAll), block.DrainAll);
                values.Add(nameof(block.Mode), block.Mode);

                // NYI: Need to properly serialize the MyInventoryItemFilter class.
                //var filters = new List<MyInventoryItemFilter>();
                //block.GetFilterList(filters);
                //var filterStrings = filters.Select(f => "[" + f.ItemId.ToString() + ":" + f.AllSubTypes.ToString() + "]");

                //values.Add(nameof(CustomProperties.FilterList), String.Join(",", filterStrings));
            }

            protected override void Deserialize(IMyConveyorSorter block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.DrainAll): block.DrainAll = Convert.ToBoolean(value.Value); break;

                        case nameof(block.Mode):
                            // NYI : SetBlacklist & SetWhitelist don't exist...
                            break;

                        case nameof(Custom.FilterList):
                            // NYI: Need to properly deserialize the MyInventoryItemFilter class.

                            //var list = value.Value.ToString().Split(',');
                            //var filters = new List<MyInventoryItemFilter>();
                            //foreach (var item in list)
                            //{
                            //    var itemId = item.Substring(1, item.IndexOf(',') - 2);
                            //    var subtypes = item.Skip(item.IndexOf(',')).TakeWhile(c => c != ']');
                            //}
                            break;
                    }
                }
            }
        }

        public class Door : FunctionalBlock<IMyDoor>
        {
            protected override void Serialize(IMyDoor block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);
            }

            protected override void Deserialize(IMyDoor block, Dictionary<String, Object> values)
            {
                if (values.ContainsKey(Custom.Locked) && Object.Equals(values[Custom.Locked], true))
                {
                    values.Remove(nameof(block.Enabled));
                    switch (block.Status)
                    {
                        case DoorStatus.Closed:
                            block.Enabled = false;
                            break;

                        case DoorStatus.Open:
                        case DoorStatus.Opening:
                            block.Enabled = true;
                            block.CloseDoor();
                            break;
                    }
                }

                if (values.ContainsKey(Custom.Closed) && Object.Equals(values[Custom.Closed], true))
                {
                    switch (block.Status)
                    {
                        case DoorStatus.Open:
                        case DoorStatus.Opening:
                            values.Remove(nameof(block.Enabled));
                            block.Enabled = true;

                            block.CloseDoor();
                            break;
                    }
                }

                base.Deserialize(block, values);
            }
        }

        public class FunctionalBlock : FunctionalBlock<IMyFunctionalBlock> { }

        public class FunctionalBlock<T> : TerminalBlock<T>
                where T : IMyFunctionalBlock
        {
            protected override void Serialize(T block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);
                values.Add(nameof(block.Enabled), block.Enabled);
            }

            protected override void Deserialize(T block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);
                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.Enabled): block.Enabled = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class GasGenerator : FunctionalBlock<IMyGasGenerator>
        {
            protected override void Serialize(IMyGasGenerator block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.AutoRefill), block.AutoRefill);
                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMyGasGenerator block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.AutoRefill): block.AutoRefill = Convert.ToBoolean(value.Value); break;
                        case nameof(block.UseConveyorSystem): block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class GasTank : FunctionalBlock<IMyGasTank>
        {
            protected override void Serialize(IMyGasTank block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.AutoRefillBottles), block.AutoRefillBottles);
                values.Add(nameof(block.Stockpile), block.Stockpile);
            }

            protected override void Deserialize(IMyGasTank block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.AutoRefillBottles): block.AutoRefillBottles = Convert.ToBoolean(value.Value); break;
                        case nameof(block.Stockpile): block.Stockpile = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class GravityGenerator : FunctionalBlock<IMyGravityGenerator>
        {
            protected override void Serialize(IMyGravityGenerator block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.FieldSize.X), block.FieldSize.X);
                values.Add(nameof(block.FieldSize.Y), block.FieldSize.Y);
                values.Add(nameof(block.FieldSize.Z), block.FieldSize.Z);
                values.Add(nameof(block.GravityAcceleration), block.GravityAcceleration);
            }

            protected override void Deserialize(IMyGravityGenerator block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);
                var fieldX = block.FieldSize.X;
                var fieldY = block.FieldSize.Y;
                var fieldZ = block.FieldSize.Z;

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.FieldSize.X): fieldX = Convert.ToSingle(value.Value); break;
                        case nameof(block.FieldSize.Y): fieldY = Convert.ToSingle(value.Value); break;
                        case nameof(block.FieldSize.Z): fieldZ = Convert.ToSingle(value.Value); break;
                        case nameof(block.GravityAcceleration): block.GravityAcceleration = Convert.ToSingle(value.Value); break;
                    }
                }

                if (fieldX != block.FieldSize.X || fieldY != block.FieldSize.Y || fieldZ != block.FieldSize.Z)
                {
                    block.FieldSize = new VRageMath.Vector3(fieldX, fieldY, fieldZ);
                }
            }
        }

        public class GravityGeneratorSphere : FunctionalBlock<IMyGravityGeneratorSphere>
        {
            protected override void Serialize(IMyGravityGeneratorSphere block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.GravityAcceleration), block.GravityAcceleration);
                values.Add(nameof(block.Radius), block.Radius);
            }

            protected override void Deserialize(IMyGravityGeneratorSphere block, Dictionary<String, Object> values)
            {
                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.GravityAcceleration): block.GravityAcceleration = Convert.ToSingle(value.Value); break;
                        case nameof(block.Radius): block.Radius = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }

        public class Gyro : FunctionalBlock<IMyGyro>
        {
            protected override void Serialize(IMyGyro block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.GyroPower), block.GyroPower);
                values.Add(nameof(block.GyroOverride), block.GyroOverride);
                values.Add(nameof(block.Yaw), block.Yaw);
                values.Add(nameof(block.Pitch), block.Pitch);
                values.Add(nameof(block.Roll), block.Roll);
            }

            protected override void Deserialize(IMyGyro block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.GyroPower): block.GyroPower = Convert.ToSingle(value.Value); break;
                        case nameof(block.GyroOverride): block.GyroOverride = Convert.ToBoolean(value.Value); break;
                        case nameof(block.Yaw): block.Yaw = Convert.ToSingle(value.Value); break;
                        case nameof(block.Pitch): block.Pitch = Convert.ToSingle(value.Value); break;
                        case nameof(block.Roll): block.Roll = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }

        public class LargeThrusterBase : FunctionalBlock<IMyLargeTurretBase>
        {
            protected override void Serialize(IMyLargeTurretBase block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.Azimuth), block.Azimuth);
                values.Add(nameof(block.Elevation), block.Elevation);
                values.Add(nameof(block.EnableIdleRotation), block.EnableIdleRotation);
            }

            protected override void Deserialize(IMyLargeTurretBase block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.Azimuth):
                            block.Azimuth = Convert.ToSingle(value.Value);
                            block.SyncAzimuth();
                            break;
                        case nameof(block.Elevation):
                            block.Elevation = Convert.ToSingle(value.Value);
                            block.SyncElevation();
                            break;
                        case nameof(block.EnableIdleRotation):
                            block.EnableIdleRotation = Convert.ToBoolean(value.Value);
                            block.SyncEnableIdleRotation();
                            break;
                    }
                }
            }
        }

        public class LaserAntenna : FunctionalBlock<IMyLaserAntenna>
        {
            protected override void Serialize(IMyLaserAntenna block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.AttachedProgrammableBlock), block.AttachedProgrammableBlock);
                values.Add(nameof(block.IsPermanent), block.IsPermanent);
                values.Add(nameof(block.Range), block.Range);
            }

            protected override void Deserialize(IMyLaserAntenna block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        // TODO: Check if Entity ID is a Programmable Block on the current grid.
                        case nameof(block.AttachedProgrammableBlock): block.AttachedProgrammableBlock = Convert.ToInt64(value.Value); break;
                        case nameof(block.IsPermanent): block.IsPermanent = Convert.ToBoolean(value.Value); break;
                        case nameof(block.Range): block.Range = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }

        public class LightingBlock : FunctionalBlock<IMyLightingBlock>
        {
            protected override void Serialize(IMyLightingBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.Radius), block.Radius);
                values.Add(nameof(block.Intensity), block.Intensity);
                values.Add(nameof(block.Falloff), block.Falloff);
                values.Add(nameof(block.BlinkIntervalSeconds), block.BlinkIntervalSeconds);
                values.Add(nameof(block.BlinkLength), block.BlinkLength);
                values.Add(nameof(block.BlinkOffset), block.BlinkOffset);
                values.Add(nameof(block.Color), block.Color.PackedValue);
            }

            protected override void Deserialize(IMyLightingBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.Radius): block.Radius = Convert.ToSingle(value.Value); break;
                        case nameof(block.Intensity): block.Intensity = Convert.ToSingle(value.Value); break;
                        case nameof(block.Falloff): block.Falloff = Convert.ToSingle(value.Value); break;
                        case nameof(block.BlinkIntervalSeconds): block.BlinkIntervalSeconds = Convert.ToSingle(value.Value); break;
                        case nameof(block.BlinkLength): block.BlinkLength = Convert.ToSingle(value.Value); break;
                        case nameof(block.BlinkOffset): block.BlinkOffset = Convert.ToSingle(value.Value); break;
                        case nameof(block.Color):
                            if (value.Value.GetType() == typeof(VRageMath.Color))
                            {
                                block.Color = (VRageMath.Color)value.Value;
                            }
                            else
                            {
                                block.Color = new VRageMath.Color(Convert.ToInt64(value.Value));
                            }
                            break;
                    }
                }
            }
        }

        public class MotorStator : FunctionalBlock<IMyMotorStator>
        {
            protected override void Serialize(IMyMotorStator block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.BrakingTorque), block.BrakingTorque);
                values.Add(nameof(block.Displacement), block.Displacement);
                values.Add(nameof(block.LowerLimitDeg), block.LowerLimitDeg);
                values.Add(nameof(block.RotorLock), block.RotorLock);
                values.Add(nameof(block.TargetVelocityRPM), block.TargetVelocityRPM);
                values.Add(nameof(block.Torque), block.Torque);
                values.Add(nameof(block.UpperLimitDeg), block.UpperLimitDeg);
            }

            protected override void Deserialize(IMyMotorStator block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.BrakingTorque): block.BrakingTorque = Convert.ToSingle(value.Value); break;
                        case nameof(block.Displacement): block.Displacement = Convert.ToSingle(value.Value); break;
                        case nameof(block.LowerLimitDeg): block.LowerLimitDeg = Convert.ToSingle(value.Value); break;
                        case nameof(block.RotorLock): block.RotorLock = Convert.ToBoolean(value.Value); break;
                        case nameof(block.TargetVelocityRPM): block.TargetVelocityRPM = Convert.ToSingle(value.Value); break;
                        case nameof(block.Torque): block.Torque = Convert.ToSingle(value.Value); break;
                        case nameof(block.UpperLimitDeg): block.UpperLimitDeg = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }

        public class OreDetector : FunctionalBlock<IMyOreDetector>
        {
            protected override void Serialize(IMyOreDetector block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.BroadcastUsingAntennas), block.BroadcastUsingAntennas);
            }

            protected override void Deserialize(IMyOreDetector block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.BroadcastUsingAntennas): block.BroadcastUsingAntennas = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class PistonBase : FunctionalBlock<IMyPistonBase>
        {
            protected override void Serialize(IMyPistonBase block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.MaxLimit), block.MaxLimit);
                values.Add(nameof(block.MinLimit), block.MinLimit);
                values.Add(nameof(block.Velocity), block.Velocity);
            }

            protected override void Deserialize(IMyPistonBase block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case (nameof(block.MaxLimit)): block.MaxLimit = Convert.ToSingle(value.Value); break;
                        case (nameof(block.MinLimit)): block.MinLimit = Convert.ToSingle(value.Value); break;
                        case (nameof(block.Velocity)): block.Velocity = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }

        public class RadioAntenna : FunctionalBlock<IMyRadioAntenna>
        {
            protected override void Serialize(IMyRadioAntenna block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.AttachedProgrammableBlock), block.AttachedProgrammableBlock);
                values.Add(nameof(block.EnableBroadcasting), block.EnableBroadcasting);
                values.Add(nameof(block.IgnoreAlliedBroadcast), block.IgnoreAlliedBroadcast);
                values.Add(nameof(block.IgnoreOtherBroadcast), block.IgnoreOtherBroadcast);
                values.Add(nameof(block.Radius), block.Radius);
                values.Add(nameof(block.ShowShipName), block.ShowShipName);
            }

            protected override void Deserialize(IMyRadioAntenna block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        // TODO: Check if Entity ID is a Programmable Block on the current grid.
                        case (nameof(block.AttachedProgrammableBlock)): block.AttachedProgrammableBlock = Convert.ToInt64(value.Value); break;
                        case (nameof(block.EnableBroadcasting)): block.EnableBroadcasting = Convert.ToBoolean(value.Value); break;
                        case (nameof(block.IgnoreAlliedBroadcast)): block.IgnoreAlliedBroadcast = Convert.ToBoolean(value.Value); break;
                        case (nameof(block.IgnoreOtherBroadcast)): block.IgnoreOtherBroadcast = Convert.ToBoolean(value.Value); break;
                        case (nameof(block.Radius)): block.Radius = Convert.ToInt64(value.Value); break;
                        case (nameof(block.ShowShipName)): block.ShowShipName = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class Reactor : FunctionalBlock<IMyReactor>
        {
            protected override void Serialize(IMyReactor block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMyReactor block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.UseConveyorSystem): block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class SensorBlock : FunctionalBlock<IMySensorBlock>
        {
            protected override void Serialize(IMySensorBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.DetectFloatingObjects), block.DetectFloatingObjects);
                values.Add(nameof(block.DetectEnemy), block.DetectEnemy);
                values.Add(nameof(block.DetectNeutral), block.DetectNeutral);
                values.Add(nameof(block.DetectFriendly), block.DetectFriendly);
                values.Add(nameof(block.DetectOwner), block.DetectOwner);
                values.Add(nameof(block.DetectAsteroids), block.DetectAsteroids);
                values.Add(nameof(block.DetectSubgrids), block.DetectSubgrids);
                values.Add(nameof(block.DetectStations), block.DetectStations);
                values.Add(nameof(block.DetectLargeShips), block.DetectLargeShips);
                values.Add(nameof(block.DetectSmallShips), block.DetectSmallShips);
                values.Add(nameof(block.DetectPlayers), block.DetectPlayers);
                values.Add(nameof(block.PlayProximitySound), block.PlayProximitySound);
                values.Add(nameof(block.BackExtend), block.BackExtend);
                values.Add(nameof(block.FrontExtend), block.FrontExtend);
                values.Add(nameof(block.BottomExtend), block.BottomExtend);
                values.Add(nameof(block.TopExtend), block.TopExtend);
                values.Add(nameof(block.RightExtend), block.RightExtend);
                values.Add(nameof(block.LeftExtend), block.LeftExtend);
            }

            protected override void Deserialize(IMySensorBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.DetectFloatingObjects): block.DetectFloatingObjects = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectEnemy): block.DetectEnemy = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectNeutral): block.DetectNeutral = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectFriendly): block.DetectFriendly = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectOwner): block.DetectOwner = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectAsteroids): block.DetectAsteroids = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectSubgrids): block.DetectSubgrids = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectStations): block.DetectStations = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectLargeShips): block.DetectLargeShips = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectSmallShips): block.DetectSmallShips = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectPlayers): block.DetectPlayers = Convert.ToBoolean(value.Value); break;
                        case nameof(block.PlayProximitySound): block.PlayProximitySound = Convert.ToBoolean(value.Value); break;
                        case nameof(block.BackExtend): block.BackExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.FrontExtend): block.FrontExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.BottomExtend): block.BottomExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.TopExtend): block.TopExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.RightExtend): block.RightExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.LeftExtend): block.LeftExtend = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }

        public class ShipConnector : FunctionalBlock<IMyShipConnector>
        {
            protected override void Serialize(IMyShipConnector block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.CollectAll), block.CollectAll);
                values.Add(nameof(block.PullStrength), block.PullStrength);
                values.Add(nameof(block.ThrowOut), block.ThrowOut);
            }

            protected override void Deserialize(IMyShipConnector block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.CollectAll): block.CollectAll = Convert.ToBoolean(value.Value); break;
                        case nameof(block.PullStrength): block.PullStrength = Convert.ToSingle(value.Value); break;
                        case nameof(block.ThrowOut): block.ThrowOut = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class ShipDrill : FunctionalBlock<IMyShipDrill>
        {
            protected override void Serialize(IMyShipDrill block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMyShipDrill block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.UseConveyorSystem): block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class ShipWelder : FunctionalBlock<IMyShipWelder>
        {
            protected override void Serialize(IMyShipWelder block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.HelpOthers), block.HelpOthers);
                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMyShipWelder block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.HelpOthers): block.HelpOthers = Convert.ToBoolean(value.Value); break;
                        case nameof(block.UseConveyorSystem): block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class SmallGatlingGun : FunctionalBlock<IMySmallGatlingGun>
        {
            protected override void Serialize(IMySmallGatlingGun block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMySmallGatlingGun block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.UseConveyorSystem):
                            // TODO: IMySmallGatlingGun.UseConveyorSystem is currently read-only.
                            // block.UseConveyorSystem = Convert.ToBoolean(value.Value);
                            break;
                    }
                }
            }
        }

        public class SmallMissileLauncher : FunctionalBlock<IMySmallMissileLauncher>
        {
            protected override void Serialize(IMySmallMissileLauncher block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMySmallMissileLauncher block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.UseConveyorSystem):
                            // TODO: IMySmallMissileLauncher.UseConveyorSystem is currently read-only.
                            // block.UseConveyorSystem = Convert.ToBoolean(value.Value);
                            break;
                    }
                }
            }
        }

        public class SoundBlock : FunctionalBlock<IMySoundBlock>
        {
            protected override void Serialize(IMySoundBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.LoopPeriod), block.LoopPeriod);
                // TODO: Find way to serialize play state of block.
                values.Add(nameof(block.Range), block.Range);
                values.Add(nameof(block.SelectedSound), block.SelectedSound);
                values.Add(nameof(block.Volume), block.Volume);
            }

            protected override void Deserialize(IMySoundBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(IMySoundBlock.Play): block.Play(); break;
                        case nameof(IMySoundBlock.Stop): block.Stop(); break;
                        case nameof(block.LoopPeriod): block.LoopPeriod = Convert.ToSingle(value.Value); break;
                        case nameof(block.Range): block.Range = Convert.ToSingle(value.Value); break;
                        case nameof(block.SelectedSound): block.SelectedSound = Convert.ToString(value.Value); break;
                        case nameof(block.Volume): block.Volume = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }

        public interface ISerializer
        {
            Dictionary<String, Object> GetState(IMyTerminalBlock block);
            void SetState(IMyTerminalBlock block, Dictionary<String, Object> values, MyIni storage);
            void SaveState(IMyTerminalBlock block, MyIni storage);
            void RestoreState(IMyTerminalBlock block, MyIni storage);
        }

        public class TerminalBlock<T> : ISerializer
            where T : IMyTerminalBlock
        {
            const String LineBreak = "#N#";

            public Dictionary<String, Object> GetState(IMyTerminalBlock block)
            {
                var values = new Dictionary<String, Object>();
                if (block is T)
                {
                    Serialize((T)block, values);
                }

                return values;
            }

            public void SetState(IMyTerminalBlock block, Dictionary<String, Object> values, MyIni storage)
            {
                SaveState(block, storage);

                if (block is T)
                {
                    Deserialize((T)block, values);
                }
            }

            public void SaveState(IMyTerminalBlock block, MyIni storage)
            {
                var section = "Entity" + block.EntityId;
                if (storage.ContainsSection(section))
                    return;

                var states = GetState(block);

                foreach (var state in states)
                {
                    storage.Set(section, state.Key, state.Value);
                }
            }

            public void RestoreState(IMyTerminalBlock block, MyIni storage)
            {
                var section = "Entity" + block.EntityId;
                var keys = new List<MyIniKey>();
                var values = new Dictionary<String, Object>();

                storage.GetKeys(section, keys);

                foreach (var key in keys)
                {
                    values.Add(key.Name, storage.Get(key).ToObject());
                }

                SetState(block, values, storage);
            }

            protected virtual void Serialize(T block, Dictionary<String, Object> values)
            {
                values.Add(nameof(block.CustomName), block.CustomName);
                values.Add(nameof(block.ShowInInventory), block.ShowInInventory);
                values.Add(nameof(block.ShowInTerminal), block.ShowInTerminal);
                values.Add(nameof(block.ShowInToolbarConfig), block.ShowInToolbarConfig);
                values.Add(nameof(block.ShowOnHUD), block.ShowOnHUD);
            }

            protected virtual void Deserialize(T block, Dictionary<String, Object> values)
            {
                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.CustomName): block.CustomName = value.Value.ToString(); break;
                        case nameof(block.ShowInInventory): block.ShowInInventory = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ShowInTerminal): block.ShowInTerminal = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ShowInToolbarConfig): block.ShowInToolbarConfig = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ShowOnHUD): block.ShowOnHUD = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }

        public class TextPanel : FunctionalBlock<IMyTextPanel>
        {
            protected override void Serialize(IMyTextPanel block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.BackgroundColor), block.BackgroundColor.PackedValue);
                values.Add(nameof(block.ChangeInterval), block.ChangeInterval);
                values.Add(nameof(block.Font), block.Font);
                values.Add(nameof(block.FontColor), block.FontColor.PackedValue);
                values.Add(nameof(block.FontSize), block.FontSize);
                values.Add(nameof(block.ShowText), block.ShowText);
                values.Add(Custom.PublicText, block.GetPublicText());
                values.Add(Custom.PublicTitle, block.GetPublicTitle());

                var images = new List<String>();
                block.GetSelectedImages(images);
                values.Add(Custom.Images, String.Join(";", images));
            }

            protected override void Deserialize(IMyTextPanel block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.BackgroundColor):
                            if (value.Value.GetType() == typeof(VRageMath.Color))
                            {
                                block.BackgroundColor = (VRageMath.Color)value.Value;
                            }
                            else
                            {
                                block.BackgroundColor = new VRageMath.Color(Convert.ToUInt32(value.Value));
                            }
                            break;
                        case nameof(block.ChangeInterval): block.ChangeInterval = Convert.ToSingle(value.Value); break;
                        case nameof(block.Font):
                            var font = Convert.ToString(value.Value);
                            var fonts = new List<String>();
                            block.GetFonts(fonts);
                            if (fonts.Contains(font))
                                block.Font = font;
                            break;
                        case nameof(block.FontColor):
                            if (value.Value.GetType() == typeof(VRageMath.Color))
                            {
                                block.FontColor = (VRageMath.Color)value.Value;
                            }
                            else
                            {
                                block.FontColor = new VRageMath.Color(Convert.ToUInt32(value.Value));
                            }
                            break;
                        case nameof(block.FontSize): block.FontSize = Convert.ToSingle(value.Value); break;
                        case nameof(block.ShowText):
                            var show = Convert.ToBoolean(value.Value);
                            if (show)
                            {
                                block.ShowPublicTextOnScreen();
                            }
                            else
                            {
                                block.ShowTextureOnScreen();
                            }
                            break;
                        case Custom.PublicText:
                            block.WritePublicText(value.Value.ToString(), false);
                            break;
                        case Custom.PublicTitle:
                            block.WritePublicTitle(value.Value.ToString(), false);
                            break;
                        case Custom.Images:
                            var images = value.Value.ToString().Split(';').ToList();
                            block.ClearImagesFromSelection();
                            block.AddImagesToSelection(images, true);
                            break;
                    }
                }
            }
        }

        public class Thrust : FunctionalBlock<IMyThrust>
        {
            protected override void Serialize(IMyThrust block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.ThrustOverridePercentage), block.ThrustOverridePercentage);
            }

            protected override void Deserialize(IMyThrust block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.ThrustOverridePercentage): block.ThrustOverridePercentage = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }

        public class Warhead : TerminalBlock<IMyWarhead>
        {
            protected override void Serialize(IMyWarhead block, Dictionary<String, Object> values)
            {
                // Do not serialize armed warheads.
                if (block.IsCountingDown)
                    return;

                base.Serialize(block, values);
                values.Add(nameof(block.DetonationTime), block.DetonationTime);
                values.Add(nameof(block.IsArmed), block.IsArmed);
            }

            protected override void Deserialize(IMyWarhead block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.DetonationTime): block.DetonationTime = Convert.ToSingle(value.Value); break;
                        case nameof(block.IsArmed): block.IsArmed = Convert.ToBoolean(value.Value); break;

                        case Custom.Countdown:
                            if (Object.Equals(true, value.Value))
                            {
                                block.StartCountdown();
                            }
                            else
                            {
                                block.StopCountdown();
                            }
                            break;
                    }
                }
            }
        }
        #endregion
    }
}
