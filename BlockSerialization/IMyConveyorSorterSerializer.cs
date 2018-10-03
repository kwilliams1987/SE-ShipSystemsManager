using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Serialization
    {
        public class IMyConveyorSorterSerializer : IMyFunctionalBlockSerializer<IMyConveyorSorter>
        {
            protected override void Serialize(IMyConveyorSorter block, Dictionary<String, Object> values)
            {
                base.Serialize(block, values);

                values.Add(nameof(block.DrainAll), block.DrainAll);
                values.Add(nameof(block.Mode), block.Mode);

                // NYI: Need to properly serialize the MyInventoryItemFilter class.
                //var filters = new List<MyInventoryItemFilter>();
                //block.GetFilterList(filters);
                //var filterStrings = filters.Select(f => "[" + f.ItemId.ToString() + ":" + f.AllSubTypes.ToString() + "]");

                //values.Add(nameof(CustomProperties.FilterList), String.Join(",", filterStrings));
            }

            protected override void Deserialize(IMyConveyorSorter block, Dictionary<String, Object> values)
            {
                base.Deserialize(block, values);

                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.DrainAll):
                            block.DrainAll = Convert.ToBoolean(value.Value); break;

                        case nameof(block.Mode):
                            // NYI : SetBlacklist & SetWhitelist don't exist...
                            break;

                        case nameof(CustomProperties.FilterList):
                            // NYI: Need to properly deserialize the MyInventoryItemFilter class.

                            //var list = value.Value.ToString().Split(',');
                            //var filters = new List<MyInventoryItemFilter>();
                            //foreach (var item in list)
                            //{
                            //    var itemId = item.Substring(1, item.IndexOf(',') - 2);
                            //    var subtypes = item.Skip(item.IndexOf(',')).TakeWhile(c => c != ']');
                            //}
                            break;
                    }
                }
            }
        }
    }
}
