using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyLightingBlockSerializer : IMyFunctionalBlockSerializer<IMyLightingBlock>
        {
            protected override void Serialize(IMyLightingBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.Radius), block.Radius);
                values.Add(nameof(block.Intensity), block.Intensity);
                values.Add(nameof(block.Falloff), block.Falloff);
                values.Add(nameof(block.BlinkIntervalSeconds), block.BlinkIntervalSeconds);
                values.Add(nameof(block.BlinkLength), block.BlinkLength);
                values.Add(nameof(block.BlinkOffset), block.BlinkOffset);
                values.Add(nameof(block.Color), block.Color.PackedValue);
            }

            protected override void Deserialize(IMyLightingBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.Radius):
                            block.Radius = Convert.ToSingle(value.Value); break;
                        case nameof(block.Intensity):
                            block.Intensity = Convert.ToSingle(value.Value); break;
                        case nameof(block.Falloff):
                            block.Falloff = Convert.ToSingle(value.Value); break;
                        case nameof(block.BlinkIntervalSeconds):
                            block.BlinkIntervalSeconds = Convert.ToSingle(value.Value); break;
                        case nameof(block.BlinkLength):
                            block.BlinkLength = Convert.ToSingle(value.Value); break;
                        case nameof(block.BlinkOffset):
                            block.BlinkOffset = Convert.ToSingle(value.Value); break;
                        case nameof(block.Color):
                            block.Color = new VRageMath.Color(Convert.ToInt64(value.Value)); break;
                    }
                }
            }
        }
    }
}
