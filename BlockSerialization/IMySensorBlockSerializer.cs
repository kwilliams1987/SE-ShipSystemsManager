using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMySensorBlockSerializer : IMyFunctionalBlockSerializer<IMySensorBlock>
        {
            protected override void Serialize(IMySensorBlock block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.DetectFloatingObjects), block.DetectFloatingObjects);
                values.Add(nameof(block.DetectEnemy), block.DetectEnemy);
                values.Add(nameof(block.DetectNeutral), block.DetectNeutral);
                values.Add(nameof(block.DetectFriendly), block.DetectFriendly);
                values.Add(nameof(block.DetectOwner), block.DetectOwner);
                values.Add(nameof(block.DetectAsteroids), block.DetectAsteroids);
                values.Add(nameof(block.DetectSubgrids), block.DetectSubgrids);
                values.Add(nameof(block.DetectStations), block.DetectStations);
                values.Add(nameof(block.DetectLargeShips), block.DetectLargeShips);
                values.Add(nameof(block.DetectSmallShips), block.DetectSmallShips);
                values.Add(nameof(block.DetectPlayers), block.DetectPlayers);
                values.Add(nameof(block.PlayProximitySound), block.PlayProximitySound);
                values.Add(nameof(block.BackExtend), block.BackExtend);
                values.Add(nameof(block.FrontExtend), block.FrontExtend);
                values.Add(nameof(block.BottomExtend), block.BottomExtend);
                values.Add(nameof(block.TopExtend), block.TopExtend);
                values.Add(nameof(block.RightExtend), block.RightExtend);
                values.Add(nameof(block.LeftExtend), block.LeftExtend);
            }

            protected override void Deserialize(IMySensorBlock block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.DetectFloatingObjects):
                            block.DetectFloatingObjects = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectEnemy):
                            block.DetectEnemy = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectNeutral):
                            block.DetectNeutral = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectFriendly):
                            block.DetectFriendly = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectOwner):
                            block.DetectOwner = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectAsteroids):
                            block.DetectAsteroids = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectSubgrids):
                            block.DetectSubgrids = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectStations):
                            block.DetectStations = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectLargeShips):
                            block.DetectLargeShips = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectSmallShips):
                            block.DetectSmallShips = Convert.ToBoolean(value.Value); break;
                        case nameof(block.DetectPlayers):
                            block.DetectPlayers = Convert.ToBoolean(value.Value); break;
                        case nameof(block.PlayProximitySound):
                            block.PlayProximitySound = Convert.ToBoolean(value.Value); break;
                        case nameof(block.BackExtend):
                            block.BackExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.FrontExtend):
                            block.FrontExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.BottomExtend):
                            block.BottomExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.TopExtend):
                            block.TopExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.RightExtend):
                            block.RightExtend = Convert.ToSingle(value.Value); break;
                        case nameof(block.LeftExtend):
                            block.LeftExtend = Convert.ToSingle(value.Value); break;
                    }
                }
            }
        }
    }
}
