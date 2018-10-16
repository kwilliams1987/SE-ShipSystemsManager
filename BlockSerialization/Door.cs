using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serializer
    {
        public class Door : IMyFunctionalBlockSerializer<IMyDoor>
        {
            protected override void Serialize(IMyDoor block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);
            }

            protected override void Deserialize(IMyDoor block, Dictionary<String, Object> values)
            {
                if (values.ContainsKey(Custom.Locked) && Object.Equals(values[Custom.Locked], true))
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

                if (values.ContainsKey(Custom.Closed) && Object.Equals(values[Custom.Closed], true))
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
