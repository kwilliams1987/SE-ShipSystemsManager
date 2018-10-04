using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyThrustSerializer : IMyFunctionalBlockSerializer<IMyThrust>
        {
            protected override void Serialize(IMyThrust block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.ThrustOverridePercentage), block.ThrustOverridePercentage);
            }

            protected override void Deserialize(IMyThrust block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.ThrustOverridePercentage):
                            block.ThrustOverridePercentage = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }
    }
}
