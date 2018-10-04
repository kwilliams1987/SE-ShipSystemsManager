using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyReactorSerializer : IMyFunctionalBlockSerializer<IMyReactor>
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
                        case nameof(block.UseConveyorSystem):
                            block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
