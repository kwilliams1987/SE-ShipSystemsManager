using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyLargeTurretBaseSerializer : IMyFunctionalBlockSerializer<IMyLargeTurretBase>
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

                block.Azimuth = Convert.ToSingle(values[nameof(block.Azimuth)]);
                block.Elevation = Convert.ToSingle(values[nameof(block.Elevation)]);
                block.EnableIdleRotation = Convert.ToBoolean(values[nameof(block.EnableIdleRotation)]);

                block.SyncAzimuth();
                block.SyncElevation();
                block.SyncEnableIdleRotation();
            }
        }
    }
}
