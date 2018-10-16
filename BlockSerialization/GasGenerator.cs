using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class GasGenerator : IMyFunctionalBlockSerializer<IMyGasGenerator>
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
                        case nameof(block.AutoRefill):
                            block.AutoRefill = Convert.ToBoolean(value.Value); break;
                        case nameof(block.UseConveyorSystem):
                            block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
