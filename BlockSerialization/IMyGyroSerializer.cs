using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyGyroSerializer : IMyFunctionalBlockSerializer<IMyGyro>
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

                block.GyroPower = Convert.ToSingle(values[nameof(block.GyroPower)]);
                block.GyroOverride = Convert.ToBoolean(values[nameof(block.GyroOverride)]);
                block.Yaw = Convert.ToSingle(values[nameof(block.Yaw)]);
                block.Pitch = Convert.ToSingle(values[nameof(block.Pitch)]);
                block.Roll = Convert.ToSingle(values[nameof(block.Roll)]);
            }
        }
    }
}
