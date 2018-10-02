using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Serialization
    {
        public interface IMyTerminalBlockSerializer
        {
            Dictionary<String, Object> GetState(IMyTerminalBlock block);
            void SetState(IMyTerminalBlock block, Dictionary<String, Object> values);
            void SaveState(IMyTerminalBlock block);
            void RestoreState(IMyTerminalBlock block);
        }

        public class IMyTerminalBlockSerializer<T>: IMyTerminalBlockSerializer
            where T : IMyTerminalBlock
        {
            public Dictionary<String, Object> GetState(IMyTerminalBlock block)
            {
                var values = new Dictionary<String, Object>();
                if (block is T)
                {
                    Serialize((T)block, values);
                }

                return values;
            }

            public void SetState(IMyTerminalBlock block, Dictionary<String, Object> values)
            {
                SaveState(block);

                if (block is T)
                {
                    Deserialize((T)block, values);
                }
            }

            public void SaveState(IMyTerminalBlock block)
            {
                if (block.CustomData.Split('n').Any(l => l == "saved:true"))
                    return;

                var states = GetState(block);
                var config = "saved:true\n";
                foreach (var state in states)
                {
                    config += state.Key + ":" + state.Value.ToString().Replace("\n", "#N#") + "\n";
                }

                block.CustomData += config;
            }

            public void RestoreState(IMyTerminalBlock block)
            {
                var values = new Dictionary<String, Object>();

                foreach (var line in block.CustomData.Split('\n').Where(l => l.Contains(":")))
                {
                    var pair = line.Split(':');
                    values.Add(pair.ElementAt(0), String.Join(":", pair.Skip(1)).Replace("#N#", "\n"));
                }

                SetState(block, values);
            }

            protected virtual void Serialize(T block, Dictionary<String, Object> values)
            {
                values.Add(nameof(block.CustomName), block.CustomName);
                values.Add(nameof(block.ShowInInventory), block.ShowInInventory);
                values.Add(nameof(block.ShowInTerminal), block.ShowInTerminal);
                values.Add(nameof(block.ShowInToolbarConfig), block.ShowInToolbarConfig);
                values.Add(nameof(block.ShowOnHUD), block.ShowOnHUD);
            }

            protected virtual void Deserialize(T block, Dictionary<String, Object> values)
            {
                foreach (var value in values)
                {
                    switch (value.Key)
                    {
                        case nameof(block.CustomName):
                            block.CustomName = value.Value.ToString(); break;
                        case nameof(block.ShowInInventory):
                            block.ShowInInventory = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ShowInTerminal):
                            block.ShowInTerminal = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ShowInToolbarConfig):
                            block.ShowInToolbarConfig = Convert.ToBoolean(value.Value); break;
                        case nameof(block.ShowOnHUD):
                            block.ShowOnHUD = Convert.ToBoolean(value.Value); break;
                    }
                }
            }
        }
    }
}