using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        private void HandleBattleStations()
        {
            if (Me.HasConfigFlag("custom-states", "battle"))
            {
                var doors = GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY));
                if (doors.Any())
                {
                    Output("Locking down " + doors.Count() + " security doors.");
                    foreach (var door in doors)
                    {
                        door.SaveState();
                        if (door.OpenRatio > 0)
                        {
                            door.Enabled = true;
                            door.CloseDoor();
                        }
                    }
                }

                var doorsigns = GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(d => d.HasFunction(BlockFunction.SIGN_BATTLE));
                foreach (var sign in doorsigns.Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION))) // Decompression superseeds battle stations.
                {
                    sign.SaveState();
                    sign.WritePublicText(Configuration.Decompression.ZONE_LABEL);
                    sign.FontColor = Configuration.Decompression.SIGN_FOREGROUND_COLOR;
                    sign.BackgroundColor = Configuration.Decompression.SIGN_BACKGROUND_COLOR;
                    sign.FontSize = 2.9f / Configuration.Decompression.ZONE_LABEL.Split('\n').Count();
                }

                var signs = GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(s => s.HasFunction(BlockFunction.SIGN_WARNING));
                foreach (var sign in doorsigns.Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION))) // Decompression superseeds battle stations.
                {
                    sign.SaveState();
                    sign.ClearImagesFromSelection();
                    sign.AddImageToSelection(Configuration.Decompression.SIGN_IMAGE);
                    sign.ShowTextureOnScreen();
                    sign.Enabled = true;
                }

                var soundBlocks = GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN));
                foreach (var soundBlock in soundBlocks.Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION))) // Decompression superseeds battle stations.
                {
                    soundBlock.SaveState();
                    soundBlock.SelectedSound = Configuration.Decompression.ALERT_SOUND;
                    soundBlock.LoopPeriod = 3600;
                    soundBlock.Enabled = true;
                    soundBlock.Play();
                }

                var lights = GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>();
                foreach (var light in lights)
                {
                    light.SaveState();
                    if (light.HasFunction(BlockFunction.LIGHT_WARNING))
                    {
                        light.Color = Configuration.Decompression.LIGHT_COLOR;
                        light.BlinkIntervalSeconds = Configuration.Decompression.LIGHT_BLINK;
                        light.BlinkLength = Configuration.Decompression.LIGHT_DURATION;
                        light.BlinkOffset = Configuration.Decompression.LIGHT_OFFSET;
                        light.Enabled = true;
                    }
                    else
                    {
                        light.Enabled = false;
                    }
                }
            }
            else
            {
                var doors = GridTerminalSystem.GetBlocksOfType<IMyDoor>(d => d.HasFunction(BlockFunction.DOOR_SECURITY));
                foreach (var door in doors)
                {
                    door.RestoreState();
                }

                var doorsigns = GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(d => d.HasFunction(BlockFunction.SIGN_BATTLE));
                foreach (var sign in doorsigns.Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION)))
                {
                    sign.RestoreState();
                }

                var signs = GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(s => s.HasFunction(BlockFunction.SIGN_WARNING));
                foreach (var sign in signs.Where(s => !s.HasConfigFlag("state", BlockState.DECOMPRESSION)))
                {
                    sign.RestoreState();
                }

                var soundBlocks = GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(s => s.HasFunction(BlockFunction.SOUNDBLOCK_SIREN) && !s.HasConfigFlag("state", BlockState.DECOMPRESSION));
                foreach (var soundBlock in soundBlocks)
                {
                    soundBlock.RestoreState();
                }

                var lights = GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>();
                foreach (var light in lights)
                {
                    light.RestoreState();
                }
            }
        }
    }
}
