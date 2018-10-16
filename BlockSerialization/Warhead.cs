using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class Warhead : TerminalBlock<IMyWarhead>
        {
            protected override void Serialize(IMyWarhead block, Dictionary<String, Object> values)
            {
                // Do not serialize armed warheads.
                if (block.IsCountingDown)
                    return;
                
                base.Serialize(block, values);
                values.Add(nameof(block.DetonationTime), block.DetonationTime);
                values.Add(nameof(block.IsArmed), block.IsArmed);
            }

            protected override void Deserialize(IMyWarhead block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.DetonationTime):
                            block.DetonationTime = Convert.ToSingle(value.Value); break;

                        case nameof(block.IsArmed):
                            block.IsArmed = Convert.ToBoolean(value.Value); break;

                        case Custom.Countdown:
                            if (Object.Equals(true, value.Value))
                            {
                                block.StartCountdown();
                            }
                            else
                            {
                                block.StopCountdown();
                            }
                            break;
                    }
                }
            }
        }
    }
}
