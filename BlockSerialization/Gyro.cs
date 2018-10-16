using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class Gyro : IMyFunctionalBlockSerializer<IMyGyro>
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
                        case nameof(block.GyroPower):
                            block.GyroPower = Convert.ToSingle(value.Value); break;
                        case nameof(block.GyroOverride):
                            block.GyroOverride = Convert.ToBoolean(value.Value); break;
                        case nameof(block.Yaw):
                            block.Yaw = Convert.ToSingle(value.Value); break;
                        case nameof(block.Pitch):
                            block.Pitch = Convert.ToSingle(value.Value); break;
                        case nameof(block.Roll):
                            block.Roll = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }
    }
}
