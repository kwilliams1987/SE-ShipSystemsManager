using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMySmallGatlingGunSerializer : IMyFunctionalBlockSerializer<IMySmallGatlingGun>
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
    }
}
