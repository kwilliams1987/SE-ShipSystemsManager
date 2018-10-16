using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        void TestBattleStations()
        {
            var blocks = new List<IMyTerminalBlock>()
                            .Concat(GetBlocks<IMyDoor>(d => d.IsA(Function.Security)))
                            .Concat(GetBlocks<IMyTextPanel>(l => l.IsA(Function.BattleSign)))
                            .Concat(GetBlocks<IMyTextPanel>(l => l.IsA(Function.Warning)))
                            .Concat(GetBlocks<IMySoundBlock>(s => s.IsA(Function.Siren)))
                            .Concat(GetBlocks<IMyLightingBlock>());

            if (SelfStorage.GetValues("custom-states").Contains("battle"))
            {
                SetStates(blocks, State.BattleStations);
            }
            else
            {
                ClearStates(blocks, State.BattleStations);
            }
        }
    }
}
