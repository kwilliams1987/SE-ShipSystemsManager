using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        private void TestBattleStations()
        {
            var blocks = new List<IMyTerminalBlock>()
                            .Concat(GetBlocks<IMyDoor>(d => d.IsA(BlockType.Security)))
                            .Concat(GetBlocks<IMyTextPanel>(l => l.IsA(BlockType.BattleSign)))
                            .Concat(GetBlocks<IMyTextPanel>(l => l.IsA(BlockType.Warning)))
                            .Concat(GetBlocks<IMySoundBlock>(s => s.IsA(BlockType.Siren)))
                            .Concat(GetBlocks<IMyLightingBlock>());

            if (SelfStorage.GetValues("custom-states").Contains("battle"))
            {
                SetStates(blocks, BlockState.BattleStations);
            }
            else
            {
                ClearStates(blocks, BlockState.BattleStations);
            }
        }
    }
}
