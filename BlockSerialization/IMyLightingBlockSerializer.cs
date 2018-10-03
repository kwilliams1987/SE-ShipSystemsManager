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

                block.Radius = Convert.ToSingle(values[nameof(block.Radius)]);
                block.Intensity = Convert.ToSingle(values[nameof(block.Intensity)]);
                block.Falloff = Convert.ToSingle(values[nameof(block.Falloff)]);
                block.BlinkIntervalSeconds = Convert.ToSingle(values[nameof(block.BlinkIntervalSeconds)]);
                block.BlinkLength = Convert.ToSingle(values[nameof(block.BlinkLength)]);
                block.BlinkOffset = Convert.ToSingle(values[nameof(block.BlinkOffset)]);
                block.Color = new VRageMath.Color(Convert.ToInt64(values[nameof(block.Color)]));
            }
        }
    }
}
