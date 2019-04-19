// <mdk sortorder="1" />
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public partial class Program
    {
        private MyIni Configuration { get; set; }

        private IEnumerator<Int32> StateMachineExecutor()
        {
            Echo("Starting cycle.");

            var gridState = GridState;
            var debugDisplays = 0;

            var zoneStatSurface = Me.GetSurface(0);
            zoneStatSurface.ContentType = ContentType.TEXT_AND_IMAGE;

            yield return 0;

            Echo("Restoring global statuses.");
            Configuration = new MyIni();

            if (Configuration.TryParse(Storage))
            {
                var state = Convert.ToInt32(GridState);
                var power = PowerThreshold;
                var countdown = Countdown;

                Configuration.Get(ConfigSection, nameof(GridState)).TryGetInt32(out state);
                GridState = (EntityState) state;

                Configuration.Get(ConfigSection, nameof(PowerThreshold)).TryGetDouble(out power);
                PowerThreshold = power;

                Configuration.Get(ConfigSection, nameof(Countdown)).TryGetSingle(out countdown);
                Countdown = countdown;
            }
            else
            {
                Echo("The ships system configuration is empty, outdated or corrupt. Resetting to known good values.");

                Configuration.Set(ConfigSection, nameof(GridState), Convert.ToInt32(GridState));
                Configuration.Set(ConfigSection, nameof(PowerThreshold), PowerThreshold);
                Configuration.Set(ConfigSection, nameof(Countdown), Countdown);

                Storage = Configuration.ToString();
            }

            yield return 1;

            while (Execute)
            {
                Echo("Refreshing grid cache.");
                var grid = GetGridBlocks();
                var zones = new Dictionary<String, EntityState>();
                var countdown = Countdown;

                var outputs = grid.Where(b => b.Functions.HasFlag(BlockFunction.Debugger))
                                    .Select(b => b.Target).OfType<IMyTextSurface>().ToList();

                foreach (var provider in grid.Where(b => b.Target is IMyTextSurfaceProvider))
                {
                    var target = provider.Target as IMyTextSurfaceProvider;
                    for (var i = 0; i < target.SurfaceCount; i++)
                    {
                        if (provider.GetEnumConfig<BlockFunction>($"functions-{i}", BlockFunction.None).HasFlag(BlockFunction.Debugger))
                            outputs.Add(target.GetSurface(i));
                    }
                }

                if (outputs.Any())
                {
                    Echo = message =>
                    {
                        var current = outputs.First().GetText().Split('\n').ToList();
                        while (current.Count() > 32)
                        {
                            current.RemoveAt(0);
                        }

                        current.Add(message);
                        var messages = String.Join("\n", current);
                        foreach (var output in outputs)
                        {
                            output.FontSize = 0.5f;
                            output.TextPadding = 2;
                            output.Font = "Monospace";
                            output.WriteText(messages);
                        }
                    };

                    if (debugDisplays != outputs.Count)
                    {
                        Echo($"Found {outputs.Count()} debugging displays.");
                        debugDisplays = outputs.Count;
                    }
                }
                else
                {
                    Echo = message => { };
                    debugDisplays = 0;
                }

                var warheads = grid.Where(b => b.Functions.HasFlag(BlockFunction.SelfDestruct))
                    .Select(b => b.Target).OfType<IMyWarhead>()
                    .Where(w => w.IsWorking);

                if (warheads.Any())
                {
                    var activeWarheads = warheads.Where(w => w.IsCountingDown);
                    if (activeWarheads.Any())
                    {
                        countdown = Math.Min(countdown, activeWarheads.Min(w => w.DetonationTime));
                    }
                }
                else
                {
                    Echo($"Self destruct is unavailable.");
                    countdown = -1;
                }

                foreach (var zone in grid.SelectMany(b => b.Zones).Distinct())
                {
                    zones.Add(zone, EntityState.Default);
                }

                yield return 2;

                Echo("Enumerating zone statuses.");
                foreach (var zone in zones.ToList())
                {
                    if (TestDecompression(zone.Key, grid.Where(b => b.Zones.Contains(zone.Key))) && !zones[zone.Key].HasFlag(EntityState.Decompress))
                    {
                        Echo($"Decompression detected in zone {zone.Key}!");
                        zones[zone.Key] |= EntityState.Decompress;
                        UpdateNeeded = true;
                    }
                    else if (zones[zone.Key].HasFlag(EntityState.Decompress))
                    {
                        zones[zone.Key] &= ~EntityState.Decompress;
                        UpdateNeeded = true;
                    }

                    if (TestIntruder(zone.Key, grid.Where(b => b.Zones.Contains(zone.Key))) && !zones[zone.Key].HasFlag(EntityState.Intruder))
                    {
                        Echo($"Intruder detected in zone {zone.Key}!");
                        zones[zone.Key] |= EntityState.Intruder;
                        UpdateNeeded = true;
                    }
                    else if (zones[zone.Key].HasFlag(EntityState.Intruder))
                    {
                        zones[zone.Key] &= ~EntityState.Intruder;
                        UpdateNeeded = true;
                    }
                }

                yield return 3;

                Echo("Enumerating global statuses.");
                if (TestLowPower(grid) && !GridState.HasFlag(EntityState.LowPower))
                {
                    Echo("Low power detected");
                    GridState |= EntityState.LowPower;
                    UpdateNeeded = true;
                }
                else if (GridState.HasFlag(EntityState.LowPower))
                {
                    GridState &= ~EntityState.LowPower;
                    UpdateNeeded = true;
                }
                
                yield return 4;

                // Force updates;
                UpdateNeeded = true;
                if (UpdateNeeded)
                {
                    Echo("Applying block styles.");

                    foreach (var block in grid)
                    {
                        var states = GridState;
                        foreach (var zone in block.Zones)
                        {
                            states |= zones[zone];
                        }

                        Echo($"Styling {block.Target.DisplayNameText} in zones {String.Join(", ", block.Zones)}");
                        StyleBlock(block, states, countdown);
                    }

                    UpdateNeeded = false;
                }

                yield return 5;

                Echo("Saving global statuses.");
                Configuration.Set(ConfigSection, nameof(GridState), Convert.ToInt32(GridState));
                Configuration.Set(ConfigSection, nameof(PowerThreshold), PowerThreshold);
                Configuration.Set(ConfigSection, nameof(Countdown), Countdown);

                Storage = Configuration.ToString();

                foreach (var block in grid)
                    block.Save();

                yield return 6;

                Echo("Updating status screen.");
                UpdateZoneStatistics(zoneStatSurface, zones);

                yield return 7;
            }

            yield break;
        }

        private IEnumerable<Block<IMyTerminalBlock>> GetGridBlocks()
        {
            var grid = new List<IMyTerminalBlock>();
            var blocks = new List<Block<IMyTerminalBlock>>();
            var zones = new List<String>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(grid);

            foreach (var block in grid)
            {
                var config = new MyIni();
                if (config.TryParse(block.CustomData))
                {
                    blocks.Add(new Block<IMyTerminalBlock>(block, config));
                }
            }

            return blocks;
        }

        private void UpdateZoneStatistics(IMyTextSurface textSurface, IReadOnlyDictionary<String, EntityState> zones)
        {
            var builder = new StringBuilder();
            var zonestatus = new List<String>();
            var gridType = "Ship";

            if (Me.CubeGrid.IsStatic)
                gridType = "Station";

            var zonepad = gridType.Length;

            if (zones.Any())
                zonepad = Math.Max(zones.Max(z => z.Key.Length), gridType.Length);

            builder.Append($"{gridType.PadLeft(zonepad)} ");

            if (GridState.HasFlag(EntityState.Destruct))
                zonestatus.Add("Self Destruct");

            if (GridState.HasFlag(EntityState.Battle))
                zonestatus.Add("Battle Stations");

            if (GridState.HasFlag(EntityState.LowPower))
                zonestatus.Add("Low Power");

            builder.AppendLine(String.Join(", ", zonestatus.DefaultIfEmpty("Normal")));

            foreach (var zone in zones.OrderBy(z => z.Key))
            {
                zonestatus.Clear();
                builder.Append($"{zone.Key.PadLeft(zonepad)} ");

                if (zone.Value.HasFlag(EntityState.Decompress))
                    zonestatus.Add("Decompressed");

                if (zone.Value.HasFlag(EntityState.Intruder))
                    zonestatus.Add("Intruder");

                builder.AppendLine(String.Join(", ", zonestatus.DefaultIfEmpty("Normal")));
            }

            textSurface.WriteAndScaleText(builder);
        }
    }
}
