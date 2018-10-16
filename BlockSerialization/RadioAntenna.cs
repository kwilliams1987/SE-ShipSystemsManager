using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class RadioAntenna : IMyFunctionalBlockSerializer<IMyRadioAntenna>
        {
            protected override void Serialize(IMyRadioAntenna block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.AttachedProgrammableBlock), block.AttachedProgrammableBlock);
                values.Add(nameof(block.EnableBroadcasting), block.EnableBroadcasting);
                values.Add(nameof(block.IgnoreAlliedBroadcast), block.IgnoreAlliedBroadcast);
                values.Add(nameof(block.IgnoreOtherBroadcast), block.IgnoreOtherBroadcast);
                values.Add(nameof(block.Radius), block.Radius);
                values.Add(nameof(block.ShowShipName), block.ShowShipName);
            }

            protected override void Deserialize(IMyRadioAntenna block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case (nameof(block.AttachedProgrammableBlock)):
                            // TODO: Check if Entity ID is a Programmable Block on the current grid.
                            block.AttachedProgrammableBlock = Convert.ToInt64(value.Value); break;
                        case (nameof(block.EnableBroadcasting)):
                            block.EnableBroadcasting = Convert.ToBoolean(value.Value); break;
                        case (nameof(block.IgnoreAlliedBroadcast)):
                            block.IgnoreAlliedBroadcast = Convert.ToBoolean(value.Value); break;
                        case (nameof(block.IgnoreOtherBroadcast)):
                            block.IgnoreOtherBroadcast = Convert.ToBoolean(value.Value); break;
                        case (nameof(block.Radius)):
                            block.Radius = Convert.ToInt64(value.Value); break;
                        case (nameof(block.ShowShipName)):
                            block.ShowShipName = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
