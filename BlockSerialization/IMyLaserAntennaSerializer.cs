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

                block.AttachedProgrammableBlock = Convert.ToInt64(values[nameof(block.AttachedProgrammableBlock)]);
                block.IsPermanent = Convert.ToBoolean(values[nameof(block.IsPermanent)]);
                block.Range = Convert.ToSingle(values[nameof(block.Range)]);
            }
        }
    }
}
