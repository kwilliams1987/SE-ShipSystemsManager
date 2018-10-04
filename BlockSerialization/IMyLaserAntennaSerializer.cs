using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyLaserAntennaSerializer : IMyFunctionalBlockSerializer<IMyLaserAntenna>
        {
            protected override void Serialize(IMyLaserAntenna block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.AttachedProgrammableBlock), block.AttachedProgrammableBlock);
                values.Add(nameof(block.IsPermanent), block.IsPermanent);
                values.Add(nameof(block.Range), block.Range);                
            }

            protected override void Deserialize(IMyLaserAntenna block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.AttachedProgrammableBlock):
                            // TODO: Check if Entity ID is a Programmable Block on the current grid.
                            block.AttachedProgrammableBlock = Convert.ToInt64(value.Value); break;
                        case nameof(block.IsPermanent):
                            block.IsPermanent = Convert.ToBoolean(value.Value); break;
                        case nameof(block.Range):
                            block.Range = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }
    }
}
