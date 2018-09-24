using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        private void TestBattleStations()
        {
            if (Me.HasConfigFlag("custom-states", "battle"))
            {
                GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY))
                    .SetStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_BATTLE))
                    .SetStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING))
                    .SetStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
                    .SetStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.HasFunction(BlockFunction.LIGHT_WARNING))
                    .SetStates(BlockState.BATTLESTATIONS);

                return;

                // TODO: Move to Program.ApplyBlockStates

                var doors = GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY));
                if (doors.Any())
                {
                    Output("Locking down " + doors.Count() + " security doors.");
                    doors.ApplyBlockConfigs(new Dictionary<String, Object>()
                    {
                        { "Closed", true }
                    });
                }

                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(d => d.HasFunction(BlockFunction.SIGN_BATTLE))
                    .Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION)) // Decompression superseeds battle stations.
                    .ApplyBlockConfigs(new Dictionary<String, Object>()
                    {
                        { "PublicText", Configuration.BattleStations.ZONE_LABEL },
                        { "FontColor", Configuration.BattleStations.SIGN_FOREGROUND_COLOR },
                        { "BackgroundColor", Configuration.BattleStations.SIGN_BACKGROUND_COLOR },
                        { "FontSize", Configuration.BattleStations.FONTSIZE }
                    });

                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING))
                    .Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION)) // Decompression superseeds battle stations.
                    .ApplyBlockConfigs(new Dictionary<String, Object>()
                    {
                        { "Images", Configuration.BattleStations.SIGN_IMAGE },
                        { "Enabled", true }
                    });

                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
                    .Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION)) // Decompression superseeds battle stations.
                    .ApplyBlockConfigs(new Dictionary<String, Object>
                    {
                        { "SelectedSound", Configuration.BattleStations.ALERT_SOUND },
                        { "LoopPeriod", 3600 },
                        { "Enabled", true },
                        { "Play", true }
                    });

                foreach (var group in GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>()
                    .GroupBy(l => l.HasFunction(BlockFunction.LIGHT_WARNING)))
                {
                    if (group.Key)
                    {
                        group
                            .Where(l => !l.HasConfigFlag("state", BlockState.DECOMPRESSION))
                            .ApplyBlockConfigs(new Dictionary<String, Object>()
                        {
                            { "Color", Configuration.BattleStations.LIGHT_COLOR },
                            { "BlinkIntervalSeconds", Configuration.BattleStations.LIGHT_BLINK },
                            { "BlinkLength", Configuration.BattleStations.LIGHT_DURATION },
                            { "BlinkOffset", Configuration.BattleStations.LIGHT_OFFSET },
                            { "Enabled", true },
                        });
                    }
                    else
                    {
                        group.ApplyBlockConfigs(new Dictionary<String, Object>()
                        {
                            { "Enabled", false }
                        });
                    }
                }
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY))
                    .ClearStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_BATTLE))
                    .ClearStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(l => l.HasFunction(BlockFunction.SIGN_WARNING))
                    .ClearStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
                    .ClearStates(BlockState.BATTLESTATIONS);
                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.HasFunction(BlockFunction.LIGHT_WARNING))
                    .ClearStates(BlockState.BATTLESTATIONS);

                return;

                GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY))
                    .Where(d => !d.HasConfigFlag("state", BlockState.DECOMPRESSION, BlockState.INTRUDER1))
                    .RestoreStates();

                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(d => d.HasFunction(BlockFunction.SIGN_BATTLE))
                    .Where(d => !d.HasConfigFlag("state", BlockState.DECOMPRESSION, BlockState.INTRUDER1))
                    .RestoreStates();

                GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(s => s.HasFunction(BlockFunction.SIGN_WARNING))
                    .Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION, BlockState.INTRUDER1))
                    .RestoreStates();

                GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN))
                    .Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION, BlockState.INTRUDER1))
                    .RestoreStates();

                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>()
                    .Where(l => !l.HasConfigFlag("state", BlockState.DECOMPRESSION, BlockState.INTRUDER1))
                    .RestoreStates();

                GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>()
                    .ForEach(b => b.ClearConfigFlag("state", BlockState.BATTLESTATIONS));
            }
        }
    }
}
