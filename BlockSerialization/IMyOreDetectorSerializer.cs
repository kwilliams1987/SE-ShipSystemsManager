using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyOreDetectorSerializer : IMyFunctionalBlockSerializer<IMyOreDetector>
        {
            protected override void Serialize(IMyOreDetector block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.BroadcastUsingAntennas), block.BroadcastUsingAntennas);
            }

            protected override void Deserialize(IMyOreDetector block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.BroadcastUsingAntennas):
                            block.BroadcastUsingAntennas = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
