using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMySoundBlockSerializer : IMyFunctionalBlockSerializer<IMySoundBlock>
        {
            protected override void Serialize(IMySoundBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.LoopPeriod), block.LoopPeriod);
                // TODO: Find way to serialize play state of block.
                values.Add(nameof(block.Range), block.Range);
                values.Add(nameof(block.SelectedSound), block.SelectedSound);
                values.Add(nameof(block.Volume), block.Volume);
            }

            protected override void Deserialize(IMySoundBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(IMySoundBlock.Play):
                            block.Play(); break;
                        case nameof(IMySoundBlock.Stop):
                            block.Stop(); break;
                        case nameof(block.LoopPeriod):
                            block.LoopPeriod = Convert.ToSingle(value.Value); break;
                        case nameof(block.Range):
                            block.Range = Convert.ToSingle(value.Value); break;
                        case nameof(block.SelectedSound):
                            block.SelectedSound = Convert.ToString(value.Value); break;
                        case nameof(block.Volume):
                            block.Volume = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }
    }
}
