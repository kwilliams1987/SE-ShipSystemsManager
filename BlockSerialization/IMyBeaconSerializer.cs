using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyBeaconSerializer : IMyFunctionalBlockSerializer<IMyBeacon>
        {
            protected override void Serialize(IMyBeacon block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.Radius), block.Radius);
            }

            protected override void Deserialize(IMyBeacon block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.Radius):
                            block.Radius = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }
    }
}
