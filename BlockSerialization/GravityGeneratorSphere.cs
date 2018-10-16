using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class GravityGeneratorSphere : IMyFunctionalBlockSerializer<IMyGravityGeneratorSphere>
        {
            protected override void Serialize(IMyGravityGeneratorSphere block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.GravityAcceleration), block.GravityAcceleration);
                values.Add(nameof(block.Radius), block.Radius);

            }

            protected override void Deserialize(IMyGravityGeneratorSphere block, Dictionary<String, Object> values)
            {
                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.GravityAcceleration):
                            block.GravityAcceleration = Convert.ToSingle(value.Value); break;

                        case nameof(block.Radius):
                            block.Radius = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }
    }
}
