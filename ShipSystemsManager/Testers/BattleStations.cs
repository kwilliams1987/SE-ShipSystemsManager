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
                            .Concat(GetBlocks<IMyDoor>(d => GetConfig(d).IsA(Function.Security)))
                            .Concat(GetBlocks<IMyTextPanel>(l => GetConfig(l).IsA(Function.BattleSign)))
                            .Concat(GetBlocks<IMyTextPanel>(l => GetConfig(l).IsA(Function.Warning)))
                            .Concat(GetBlocks<IMySoundBlock>(s => GetConfig(s).IsA(Function.Siren)))
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
