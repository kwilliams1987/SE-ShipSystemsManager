using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;

namespace IngameScript
{
    partial class Program
    {
        private void HandleAirVents(String zone)
        {
            var vents = GridTerminalSystem.GetBlocksOfType<IMyAirVent>(v => v.IsWorking && v.IsInZone(zone) && !v.Depressurize);
            if (vents.Any(v => !v.CanPressurize))
            {
                Output("Depressurization detected in " + vents.Count() + " Air Vents.");

                var doors = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK, true);
                if (doors.Any())
                {
                    Output("Locking down " + doors.Count() + " doors to zone " + zone + ".");
                    foreach (var door in doors)
                    {
                        door.SaveState();
                        if (door.OpenRatio > 0)
                        {
                            door.Enabled = true;
                            door.CloseDoor();
                        }
                        else
                        {
                            door.Enabled = false;
                        }
                    }
                }
                else
                {
                    Output("ALERT: Zone " + zone + " cannot be sealed, no functional doors were found!");
                }

                var doorsigns = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR);
                foreach (var sign in doorsigns)
                {
                    sign.SaveState();
                    sign.SetConfigFlag("state", BlockState.DECOMPRESSION);
                    sign.WritePublicText(Configuration.Decompression.ZONE_LABEL);
                    sign.FontColor = Configuration.Decompression.SIGN_FOREGROUND_COLOR;
                    sign.BackgroundColor = Configuration.Decompression.SIGN_BACKGROUND_COLOR;
                    sign.FontSize = 2.9f / Configuration.Decompression.ZONE_LABEL.Split('\n').Count();
                }

                var signs = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING);
                foreach (var sign in doorsigns)
                {
                    sign.SaveState();
                    sign.SetConfigFlag("state", BlockState.DECOMPRESSION);
                    sign.ClearImagesFromSelection();
                    sign.AddImageToSelection(Configuration.Decompression.SIGN_IMAGE);
                    sign.ShowTextureOnScreen();
                    sign.Enabled = true;
                }

                var soundBlocks = GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN);
                foreach (var soundBlock in soundBlocks)
                {
                    soundBlock.SaveState();
                    soundBlock.SetConfigFlag("state", BlockState.DECOMPRESSION);
                    soundBlock.SelectedSound = Configuration.Decompression.ALERT_SOUND;
                    soundBlock.LoopPeriod = 3600;
                    soundBlock.Enabled = true;
                    soundBlock.Play();
                }

                var lights = GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.IsInZone(zone));
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
                var doorGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK).GroupBy(d => d.GetZones());
                foreach (var doorGroup in doorGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize))
                    {
                        foreach (var door in doorGroup)
                        {
                            door.RestoreState();
                        }
                    }
                }

                var doorSignGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_DOOR).GroupBy(d => d.GetZones());
                foreach (var doorSignGroup in doorSignGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize))
                    {
                        foreach (var doorSign in doorSignGroup)
                        {
                            doorSign.RestoreState();
                        }
                    }
                }

                var signGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING).GroupBy(d => d.GetZones());
                foreach (var signGroup in signGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize))
                    {
                        foreach (var sign in signGroup)
                        {
                            sign.RestoreState();
                        }
                    }
                }

                var soundBlockGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN).GroupBy(s => s.GetZones());
                foreach (var soundBlockGroup in soundBlockGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize))
                    {
                        foreach (var soundBlock in soundBlockGroup)
                        {
                            soundBlock.RestoreState();
                            soundBlock.ClearConfigFlag("state", BlockState.DECOMPRESSION);
                        }
                    }
                }

                var lightGroups = GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.IsInZone(zone)).GroupBy(d => d.GetZones());
                foreach (var lightGroup in lightGroups)
                {
                    if (GridTerminalSystem.AdjacentZonesTest<IMyAirVent>(v => v.CanPressurize))
                    {
                        foreach (var light in lightGroup)
                        {
                            light.RestoreState();
                        }
                    }
                }
            }
        }
    }
}
