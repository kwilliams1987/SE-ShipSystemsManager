using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class CameraBlock : IMyFunctionalBlockSerializer<IMyCameraBlock>
        {
            protected override void Serialize(IMyCameraBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.EnableRaycast), block.EnableRaycast);
            }

            protected override void Deserialize(IMyCameraBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.EnableRaycast):
                            block.EnableRaycast = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
