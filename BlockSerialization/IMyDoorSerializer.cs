using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyDoorSerializer : IMyFunctionalBlockSerializer<IMyDoor>
        {
            protected override void Serialize(IMyDoor block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);
            }

            protected override void Deserialize(IMyDoor block, Dictionary<String, Object> values)
            {
                if (values.ContainsKey(CustomProperties.Locked) && Object.Equals(values[CustomProperties.Locked], true))
                {
                    values.Remove(nameof(block.Enabled));
                    switch (block.Status)
                    {
                        case DoorStatus.Closed:
                            block.Enabled = false;
                            break;

                        case DoorStatus.Open:
                        case DoorStatus.Opening:
                            block.Enabled = true;
                            block.CloseDoor();
                            break;
                    }
                }

                if (values.ContainsKey(CustomProperties.Closed) && Object.Equals(values[CustomProperties.Closed], true))
                {
                    switch (block.Status)
                    {
                        case DoorStatus.Open:
                        case DoorStatus.Opening:
                            values.Remove(nameof(block.Enabled));
                            block.Enabled = true;

                            block.CloseDoor();
                            break;
                    }
                }

                base.Deserialize(block, values);
            }
        }
    }
}
