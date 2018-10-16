using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class PistonBase : IMyFunctionalBlockSerializer<IMyPistonBase>
        {
            protected override void Serialize(IMyPistonBase block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.MaxLimit), block.MaxLimit);
                values.Add(nameof(block.MinLimit), block.MinLimit);
                values.Add(nameof(block.Velocity), block.Velocity);
            }

            protected override void Deserialize(IMyPistonBase block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case (nameof(block.MaxLimit)):
                            block.MaxLimit = Convert.ToSingle(value.Value); break;
                        case (nameof(block.MinLimit)):
                            block.MinLimit = Convert.ToSingle(value.Value); break;
                        case (nameof(block.Velocity)):
                            block.Velocity = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }
    }
}
