using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class Assembler : IMyFunctionalBlockSerializer<IMyAssembler>
        {
            protected override void Serialize(IMyAssembler block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.CooperativeMode), block.CooperativeMode);
                values.Add(nameof(block.Mode), block.Mode.ToString());
                values.Add(nameof(block.Repeating), block.Repeating);
                values.Add(nameof(block.UseConveyorSystem), block.UseConveyorSystem);
            }

            protected override void Deserialize(IMyAssembler block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.CooperativeMode):
                            block.CooperativeMode = Convert.ToBoolean(value.Value); break;
                        case nameof(block.Mode):
                            block.Mode = (MyAssemblerMode)Enum.Parse(typeof(MyAssemblerMode), value.Value.ToString()); break;
                        case nameof(block.Repeating):
                            block.Repeating = Convert.ToBoolean(value.Value); break;
                        case nameof(block.UseConveyorSystem):
                            block.UseConveyorSystem = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
