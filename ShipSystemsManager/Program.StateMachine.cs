using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public partial class Program
    {
        private MyIni Configuration { get; set; }

        private IEnumerator<Int32> StateMachineExecutor()
        {
            Echo("Starting cycle.");
            yield return 0;

            Echo("Restoring global statuses.");
            Configuration = new MyIni();

            if (Configuration.TryParse(Storage))
            {
                var state = Convert.ToInt32(GridState);
                var power = PowerThreshold;
                var countdown = Countdown;

                Configuration.Get(IniSection, nameof(GridState)).TryGetInt32(out state);
                GridState = (EntityState) state;

                Configuration.Get(IniSection, nameof(PowerThreshold)).TryGetDouble(out power);
                PowerThreshold = power;

                Configuration.Get(IniSection, nameof(Countdown)).TryGetSingle(out countdown);
                Countdown = countdown;
            }
            else
            {
                Echo("The ships system configuration is empty, outdated or corrupt. Resetting to known good values.");

                Configuration.Set(IniSection, nameof(GridState), Convert.ToInt32(GridState));
                Configuration.Set(IniSection, nameof(PowerThreshold), PowerThreshold);

                Storage = Configuration.ToString();
            }

            yield return 1;

            while (true)
            {
                Echo("Refreshing grid cache.");
                var grid = GetGridBlocks();
                var zones = new Dictionary<String, EntityState>();
                var countdown = Countdown;

                var warheads = grid.Where(b => b.Functions.HasFlag(BlockFunction.SelfDestruct)).Select(b => b.Target).OfType<IMyWarhead>().Where(w => w.IsCountingDown);
                if (warheads.Any())
                {
                    countdown = Math.Min(countdown, warheads.Min(w => w.DetonationTime));
                }

                foreach (var zone in grid.SelectMany(b => b.Zones).Distinct())
                {
                    zones.Add(zone, EntityState.Default);
                }

                yield return 2;

                Echo("Enumerating zone statuses.");
                foreach (var zone in zones)
                {
                    if (TestDecompression(zone.Key, grid.Where(b => b.Zones.Contains(zone.Key))))
                        zones[zone.Key] |= EntityState.Decompress;
                    else
                        zones[zone.Key] &= ~EntityState.Decompress;

                    if (TestIntruder(zone.Key, grid.Where(b => b.Zones.Contains(zone.Key))))
                        zones[zone.Key] |= EntityState.Intruder;
                    else
                        zones[zone.Key] &= ~EntityState.Intruder;
                }

                yield return 3;

                Echo("Enumerating global statuses.");
                if (TestLowPower(grid))
                    GridState |= EntityState.LowPower;
                else
                    GridState &= ~EntityState.LowPower;
                
                yield return 4;

                Echo("Applying block styles.");

                foreach (var block in grid)
                {
                    var states = GridState;
                    foreach (var zone in block.Zones)
                    {
                        states |= zones[zone];
                    }

                    StyleBlock(block, states, countdown);
                }

                yield return 5;

                Echo("Saving global statuses.");
                Configuration.Set(IniSection, nameof(GridState), Convert.ToInt32(GridState));
                Configuration.Set(IniSection, nameof(PowerThreshold), PowerThreshold);
                Configuration.Set(IniSection, nameof(Countdown), Countdown);

                Storage = Configuration.ToString();

                foreach (var block in grid)
                    block.Save();

                yield return 6;
            }
        }

        private IEnumerable<Block<IMyTerminalBlock>> GetGridBlocks()
        {
            var grid = new List<IMyTerminalBlock>();
            var blocks = new List<Block<IMyTerminalBlock>>();
            var zones = new List<String>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(grid, b => b.IsWorking);

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
    }
}
