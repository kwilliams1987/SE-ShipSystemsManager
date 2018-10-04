using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMySmallMissileLauncherSerializer : IMyFunctionalBlockSerializer<IMySmallMissileLauncher>
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
    }
}
