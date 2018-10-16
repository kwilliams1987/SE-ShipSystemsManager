using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class ShipWelder : IMyFunctionalBlockSerializer<IMyShipWelder>
        {
            protected override void Serialize(IMyShipWelder block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.HelpOthers), block.HelpOthers);
                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMyShipWelder block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.HelpOthers):
                            block.HelpOthers = Convert.ToBoolean(value.Value); break;
                        case nameof(block.UseConveyorSystem):
                            block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
