using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    public partial class Program
    {
        const String States = "states";

        private String BlockKey(IMyTerminalBlock block) => "Entity " + block.EntityId;

        private IEnumerable<String> GetStates(IMyTerminalBlock block)
            => GridStorage.Get(BlockKey(block), States).ToString().Split('\n').Where(s => s != "");

        private void SetStates(IMyTerminalBlock block, params String[] states)
        {
            var current = GetStates(block).ToList();
            var concat = current.Concat(states).Distinct();

            if (concat.Count() != current.Count())
            {
                GridStorage.Set(BlockKey(block), States, String.Join("\n", concat));
                GridStorage.Set(BlockKey(block), "state-changed", true);
            }
        }

        private void ClearStates(IMyTerminalBlock block, params String[] states)
        {
            var current = GetStates(block).ToList();
            var removed = current.RemoveAll(c => states.Contains(c));
            
            if (removed > 0)
            {
                GridStorage.Set(BlockKey(block), States, String.Join("\n", current));
                GridStorage.Set(BlockKey(block), "state-changed", true);
            }
        }

        private void SetStates(IEnumerable<IMyTerminalBlock> blocks, params String[] states)
        {
            foreach (var block in blocks)
                SetStates(block, states);
        }

        private void ClearStates(IEnumerable<IMyTerminalBlock> blocks, params String[] states)
        {
            foreach (var block in blocks)
                ClearStates(block, states);
        }
    }
}
