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
    }
}
