using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class FunctionalBlock: IMyFunctionalBlockSerializer<IMyFunctionalBlock> { }

        public class IMyFunctionalBlockSerializer<T> : TerminalBlock<T>
                where T : IMyFunctionalBlock
        {
            protected override void Serialize(T block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);
                values.Add(nameof(block.Enabled), block.Enabled);
            }

            protected override void Deserialize(T block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);
                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.Enabled):
                            block.Enabled = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
