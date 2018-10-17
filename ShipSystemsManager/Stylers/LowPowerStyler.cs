using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {
        class LowPowerStyler: BaseStyler
        {
            protected override String Prefix => "lowpower";

            public LowPowerStyler(IMyProgrammableBlock block) : base(5, Program.State.LowPower, block) { }

            public override void Style(IMyTerminalBlock block, MyIni storage)
            {
                var config = new MyConfig(block);

                if (block is IMyAssembler)
                {
                    if (!config.IsA(Function.AlwaysOn))
                    {
                        block.Apply(new Dictionary<String, Object>
                        {
                            { nameof(IMyLightingBlock.Enabled), false }
                        }, storage);
                    }
                }

                if (block is IMyLightingBlock)
                {
                    if (config.IsA(Function.AlwaysOn))
                    {
                        block.Apply(new Dictionary<String, Object>
                        {
                            { nameof(IMyLightingBlock.Enabled), true },
                            { nameof(IMyLightingBlock.Intensity), Get<Single>("light.intensity") },
                            { nameof(IMyLightingBlock.Radius), Get<Single>("light.radius") }
                        }, storage);
                    }
                    else
                    {
                        block.Apply(new Dictionary<String, Object>
                        {
                            { nameof(IMyLightingBlock.Enabled), false }
                        }, storage);
                    }
                }

                if (block is IMyRefinery)
                {
                    if (!config.IsA(Function.AlwaysOn))
                    {
                        block.Apply(new Dictionary<String, Object>
                        {
                            { nameof(IMyLightingBlock.Enabled), false }
                        }, storage);
                    }
                }
            }
        }
    }
}
