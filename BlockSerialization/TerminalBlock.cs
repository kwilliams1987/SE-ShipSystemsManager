using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    partial class Serializer
    {
        private const String LineBreak = "#N#";

        public interface IMyTerminalBlockSerializer
        {
            Dictionary<String, Object> GetState(IMyTerminalBlock block);
            void SetState(IMyTerminalBlock block, Dictionary<String, Object> values, MyIni storage);
            void SaveState(IMyTerminalBlock block, MyIni storage);
            void RestoreState(IMyTerminalBlock block, MyIni storage);
        }

        public class TerminalBlock<T>: IMyTerminalBlockSerializer
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

            public void SetState(IMyTerminalBlock block, Dictionary<String, Object> values, MyIni storage)
            {
                SaveState(block, storage);

                if (block is T)
                {
                    Deserialize((T)block, values);
                }
            }

            public void SaveState(IMyTerminalBlock block, MyIni storage)
            {
                var section = "Entity" + block.EntityId;
                if (storage.ContainsSection(section))
                    return;

                var states = GetState(block);

                foreach (var state in states)
                {
                    storage.Set(section, state.Key, state.Value);
                }
            }

            public void RestoreState(IMyTerminalBlock block, MyIni storage)
            {
                var section = "Entity" + block.EntityId;
                var keys = new List<MyIniKey>();
                var values = new Dictionary<String, Object>();

                storage.GetKeys(section, keys);
                
                foreach (var key in keys)
                {
                    values.Add(key.Name, storage.Get(key).ToObject());
                }

                SetState(block, values, storage);
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