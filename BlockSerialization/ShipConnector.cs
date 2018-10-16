using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class ShipConnector : IMyFunctionalBlockSerializer<IMyShipConnector>
        {
            protected override void Serialize(IMyShipConnector block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.CollectAll), block.CollectAll);
                values.Add(nameof(block.PullStrength), block.PullStrength);
                values.Add(nameof(block.ThrowOut), block.ThrowOut);
            }

            protected override void Deserialize(IMyShipConnector block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.CollectAll):
                            block.CollectAll = Convert.ToBoolean(value.Value); break;
                        case nameof(block.PullStrength):
                            block.PullStrength = Convert.ToSingle(value.Value); break;
                        case nameof(block.ThrowOut):
                            block.ThrowOut = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
