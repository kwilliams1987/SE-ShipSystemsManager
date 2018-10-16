using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class Cockpit : TerminalBlock<IMyCockpit>
        {
            protected override void Serialize(IMyCockpit block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.ControlThrusters), block.ControlThrusters);
                values.Add(nameof(block.ControlWheels), block.ControlWheels);
                values.Add(nameof(block.DampenersOverride), block.DampenersOverride);
                values.Add(nameof(block.HandBrake), block.HandBrake);
                values.Add(nameof(block.IsMainCockpit), block.IsMainCockpit);
                values.Add(nameof(block.ShowHorizonIndicator), block.ShowHorizonIndicator);
            }

            protected override void Deserialize(IMyCockpit block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.ControlThrusters):
                            block.ControlThrusters = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ControlWheels):
                            block.ControlWheels = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DampenersOverride):
                            block.DampenersOverride = Convert.ToBoolean(value.Value); break;
                        case nameof(block.HandBrake):
                            block.HandBrake = Convert.ToBoolean(value.Value); break;
                        case nameof(block.IsMainCockpit):
                            block.IsMainCockpit = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ShowHorizonIndicator):
                            block.ShowHorizonIndicator = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}
