using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        void TestSelfDestruct()
        {
            var blocks = new List<IMyTerminalBlock>()
                                .Concat(GetBlocks<IMyDoor>(d => GetConfig(d).IsA(Function.Security)))
                                .Concat(GetBlocks<IMyTextPanel>())
                                .Concat(GetBlocks<IMySoundBlock>(s => GetConfig(s).IsA(Function.Siren)))
                                .Concat(GetBlocks<IMyLightingBlock>())
                                .Concat(GetBlocks<IMyWarhead>(w => GetConfig(w).IsA(Function.SelfDestruct)));

            if (SelfStorage.GetValues("custom-states").Contains("destruct"))
            {
                SetStates(blocks, State.Destruct);
            }
            else
            {
                ClearStates(blocks, State.Destruct);
            }
        }
    }
}
