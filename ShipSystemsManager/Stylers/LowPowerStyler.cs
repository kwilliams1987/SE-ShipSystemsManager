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
            protected override String StylePrefix => "lowpower";

            public LowPowerStyler(IMyProgrammableBlock block)
                : base(5, BlockState.LOWPOWER, block) { }

            public override void Style(IMyTerminalBlock block, MyIni storage)
            {
                if (block is IMyAssembler)
                {
                    if (!block.HasFunction(BlockFunction.EMERGENCYPOWER))
                    {
                        block.ApplyConfig(new Dictionary<String, Object>
                        {
                            { nameof(IMyLightingBlock.Enabled), false }
                        }, storage);
                    }
                }

                if (block is IMyLightingBlock)
                {
                    if (block.HasFunction(BlockFunction.EMERGENCYPOWER))
                    {
                        block.ApplyConfig(new Dictionary<String, Object>
                        {
                            { nameof(IMyLightingBlock.Enabled), true },
                            { nameof(IMyLightingBlock.Intensity), GetStyle<Single>("light.intensity") },
                            { nameof(IMyLightingBlock.Radius), GetStyle<Single>("light.radius") }
                        }, storage);
                    }
                    else
                    {
                        block.ApplyConfig(new Dictionary<String, Object>
                        {
                            { nameof(IMyLightingBlock.Enabled), false }
                        }, storage);
                    }
                }

                if (block is IMyRefinery)
                {
                    if (!block.HasFunction(BlockFunction.EMERGENCYPOWER))
                    {
                        block.ApplyConfig(new Dictionary<String, Object>
                        {
                            { nameof(IMyLightingBlock.Enabled), false }
                        }, storage);
                    }
                }
            }
        }
    }
}
