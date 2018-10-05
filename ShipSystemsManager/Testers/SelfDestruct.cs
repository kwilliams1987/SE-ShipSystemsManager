using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        private void TestSelfDestruct()
        {
            var blocks = new List<IMyTerminalBlock>();
            blocks.AddRange(GetBlocks<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY)));
            blocks.AddRange(GetBlocks<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_BATTLE)));
            blocks.AddRange(GetBlocks<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING)));
            blocks.AddRange(GetBlocks<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN)));
            blocks.AddRange(GetBlocks<IMyLightingBlock>());
            blocks.AddRange(GetBlocks<IMyWarhead>(w => w.HasFunction(BlockFunction.WARHEAD_DESTRUCT)));

            if (Me.HasConfigFlag("custom-states", "destruct"))
            {
                blocks.SetStates(BlockState.SELFDESTRUCT);
            }
            else
            {
                blocks.ClearStates(BlockState.SELFDESTRUCT);
            }
        }
    }
}
