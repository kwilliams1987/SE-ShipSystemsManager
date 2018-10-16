using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class MotorStator : IMyFunctionalBlockSerializer<IMyMotorStator>
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
                        case nameof(block.BrakingTorque):
                            block.BrakingTorque = Convert.ToSingle(value.Value); break;
                        case nameof(block.Displacement):
                            block.Displacement = Convert.ToSingle(value.Value); break;
                        case nameof(block.LowerLimitDeg):
                            block.LowerLimitDeg = Convert.ToSingle(value.Value); break;
                        case nameof(block.RotorLock):
                            block.RotorLock = Convert.ToBoolean(value.Value); break;
                        case nameof(block.TargetVelocityRPM):
                            block.TargetVelocityRPM = Convert.ToSingle(value.Value); break;
                        case nameof(block.Torque):
                            block.Torque = Convert.ToSingle(value.Value); break;
                        case nameof(block.UpperLimitDeg):
                            block.UpperLimitDeg = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }
    }
}
