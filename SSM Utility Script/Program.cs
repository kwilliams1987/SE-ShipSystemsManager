using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public void Main(String argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.Terminal)
            {
                switch (argument)
                {
                    case "query-lcd-type":
                        var blocks = new List<IMyTextPanel>();
                        GridTerminalSystem.GetBlocksOfType(blocks, b => !b.CustomData.Contains("SubtypeId"));

                        foreach (var block in blocks)
                        {
                            var result = "\nSubtypeId=" + block.BlockDefinition.SubtypeId + "\nSubtypeName=" + block.BlockDefinition.SubtypeName;
                            block.CustomData += result;

                            Echo(block.GetType().Name + ":" + result + "\n");
                        }
                        break;
                }
            }
        }
    }
}