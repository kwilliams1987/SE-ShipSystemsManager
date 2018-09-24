using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using System;
using VRage.Game;

namespace IngameScript
{
    partial class Program
    {
        private void HandleSensors(String zone)
        {
            var sensors = GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(s => s.IsWorking && s.IsInZone(zone) && s.DetectEnemy);

            foreach (var sensor in sensors)
            {
                var entities = new List<MyDetectedEntityInfo>();
                sensor.DetectedEntities(entities);

                if (entities.Any(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies))
                {
                    Output("Sensor detected enemy in zone " + zone + "!");

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

                    var lights = GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.IsInZone(zone));
                    foreach (var light in lights)
                    {
                        light.SaveState();
                        if (light.HasFunction(BlockFunction.LIGHT_WARNING))
                        {
                            light.Color = Configuration.Intruder.LIGHT_COLOR;
                            light.BlinkIntervalSeconds = Configuration.Intruder.LIGHT_BLINK;
                            light.BlinkLength = Configuration.Intruder.LIGHT_DURATION;
                            light.BlinkOffset = Configuration.Intruder.LIGHT_OFFSET;
                            light.Enabled = true;
                        }
                        else
                        {
                            light.Enabled = false;
                        }
                    }

                    return;
                }
            }

            var doorGroups = GridTerminalSystem.GetZoneBlocksByFunction<IMyDoor>(zone, BlockFunction.DOOR_AIRLOCK).GroupBy(d => d.GetZones());
            foreach (var doorGroup in doorGroups)
            {
                if (GridTerminalSystem.AdjacentZonesTest<IMySensorBlock>(s =>
                {
                    var entities = new List<MyDetectedEntityInfo>();
                    s.DetectedEntities(entities);

                    return !entities.Any(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies);
                }))
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
                if (GridTerminalSystem.AdjacentZonesTest<IMySensorBlock>(s =>
                {
                    var entities = new List<MyDetectedEntityInfo>();
                    s.DetectedEntities(entities);

                    return !entities.Any(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies);
                }))
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
                if (GridTerminalSystem.AdjacentZonesTest<IMySensorBlock>(s =>
                {
                    var entities = new List<MyDetectedEntityInfo>();
                    s.DetectedEntities(entities);

                    return !entities.Any(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies);
                }))
                {
                    foreach (var sign in signGroup)
                    {
                        sign.RestoreState();
                    }
                }
            }

            var lightGroups = GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(l => l.IsInZone(zone)).GroupBy(d => d.GetZones());
            foreach (var lightGroup in lightGroups)
            {
                if (GridTerminalSystem.AdjacentZonesTest<IMySensorBlock>(s =>
                {
                    var entities = new List<MyDetectedEntityInfo>();
                    s.DetectedEntities(entities);

                    return !entities.Any(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies);
                }))
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
