using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System;
using VRage.Game;

namespace IngameScript
{
    partial class Program
    {
        private void HandleInteriorWeapons(String zone)
        {
            var interiorTurrets = GridTerminalSystem.GetBlocksOfType<IMyLargeInteriorTurret>(s => s.IsWorking && s.IsInZone(zone));

            if (interiorTurrets.Any(s => s.HasTarget && s.GetTargetedEntity().Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
            {
                Output("Turret detected enemy in zone " + zone + "!");

                var doors = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK);
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
                    sign.WritePublicText(Configuration.Intruder.ZONE_LABEL);
                    sign.FontColor = Configuration.Intruder.SIGN_FOREGROUND_COLOR;
                    sign.BackgroundColor = Configuration.Intruder.SIGN_BACKGROUND_COLOR;
                    sign.FontSize = 2.9f / Configuration.Intruder.ZONE_LABEL.Split('\n').Count();
                }

                var signs = GridTerminalSystem.GetZoneBlocksByFunction<IMyTextPanel>(zone, BlockFunction.SIGN_WARNING);
                foreach (var sign in doorsigns)
                {
                    sign.SaveState();
                    sign.ClearImagesFromSelection();
                    sign.AddImageToSelection(Configuration.Intruder.SIGN_IMAGE);
                    sign.ShowTextureOnScreen();
                    sign.Enabled = true;
                }

                var soundBlocks = GridTerminalSystem.GetZoneBlocksByFunction<IMySoundBlock>(zone, BlockFunction.SOUNDBLOCK_SIREN);
                foreach (var soundBlock in soundBlocks)
                {
                    soundBlock.SaveState();
                    soundBlock.SelectedSound = Configuration.Intruder.ALERT_SOUND;
                    soundBlock.LoopPeriod = 3600;
                    soundBlock.Enabled = true;
                    soundBlock.Play();
                }

            }
            else
            {
                var doorGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK).GroupBy(d => d.GetZones());
                foreach (var doorGroup in doorGroups)
                {
                    if (GridTerminalSystem.GetBlocksOfType<IMyAirVent>(v => v.IsInAnyZone(doorGroup.Key.ToArray())).All(v => v.CanPressurize))
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
                    if (GridTerminalSystem.GetBlocksOfType<IMyAirVent>(v => v.IsInAnyZone(doorSignGroup.Key.ToArray())).All(v => v.CanPressurize))
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
                    if (GridTerminalSystem.GetBlocksOfType<IMyAirVent>(v => v.IsInAnyZone(signGroup.Key.ToArray())).All(v => v.CanPressurize))
                    {
                        foreach (var sign in signGroup)
                        {
                            sign.RestoreState();
                        }
                    }
                }

            }
        }
    }
}
