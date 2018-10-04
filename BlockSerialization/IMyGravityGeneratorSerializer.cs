using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyGravityGeneratorSerializer : IMyFunctionalBlockSerializer<IMyGravityGenerator>
        {
            protected override void Serialize(IMyGravityGenerator block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.FieldSize.X), block.FieldSize.X);
                values.Add(nameof(block.FieldSize.Y), block.FieldSize.Y);
                values.Add(nameof(block.FieldSize.Z), block.FieldSize.Z);
                values.Add(nameof(block.GravityAcceleration), block.GravityAcceleration);

            }

            protected override void Deserialize(IMyGravityGenerator block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);
                var fieldX = block.FieldSize.X;
                var fieldY = block.FieldSize.Y;
                var fieldZ = block.FieldSize.Z;

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.FieldSize.X):
                            fieldX = Convert.ToSingle(value.Value); break;

                        case nameof(block.FieldSize.Y):
                            fieldY = Convert.ToSingle(value.Value); break;

                        case nameof(block.FieldSize.Z):
                            fieldZ = Convert.ToSingle(value.Value); break;

                        case nameof(block.GravityAcceleration):
                            block.GravityAcceleration = Convert.ToSingle(value.Value); break;
                    }
                }

                if (fieldX != block.FieldSize.X || fieldY != block.FieldSize.Y || fieldZ != block.FieldSize.Z)
                {
                    block.FieldSize = new VRageMath.Vector3(fieldX, fieldY, fieldZ);
                }
            }
        }
    }
}
