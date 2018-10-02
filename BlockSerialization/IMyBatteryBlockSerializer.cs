using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyBatteryBlockSerializer : IMyFunctionalBlockSerializer<IMyBatteryBlock>
        {
            protected override void Serialize(IMyBatteryBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.OnlyDischarge), block.OnlyDischarge);
                values.Add(nameof(block.OnlyRecharge), block.OnlyRecharge);
                values.Add(nameof(block.SemiautoEnabled), block.SemiautoEnabled);
            }

            protected override void Deserialize(IMyBatteryBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.OnlyDischarge):
                            block.OnlyDischarge = Convert.ToBoolean(value.Value); break;
                        case nameof(block.OnlyRecharge):
                            block.OnlyRecharge = Convert.ToBoolean(value.Value); break;
                        case nameof(block.SemiautoEnabled):
                            block.SemiautoEnabled = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
