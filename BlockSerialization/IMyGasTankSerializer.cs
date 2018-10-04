using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyGasTankSerializer : IMyFunctionalBlockSerializer<IMyGasTank>
        {
            protected override void Serialize(IMyGasTank block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.AutoRefillBottles), block.AutoRefillBottles);
                values.Add(nameof(block.Stockpile), block.Stockpile);
            }

            protected override void Deserialize(IMyGasTank block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.AutoRefillBottles):
                            block.AutoRefillBottles = Convert.ToBoolean(value.Value); break;
                        case nameof(block.Stockpile):
                            block.Stockpile = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
