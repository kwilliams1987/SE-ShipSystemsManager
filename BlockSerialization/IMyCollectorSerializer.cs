using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyCollectorSerializer : IMyFunctionalBlockSerializer<IMyCollector>
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
                        case nameof(block.UseConveyorSystem):
                            block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
