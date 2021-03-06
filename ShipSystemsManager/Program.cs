﻿// <mdk sortorder="0" />
using Sandbox.ModAPI.Ingame;
using System.Linq;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.GUI.TextPanel;
using System.Text;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public const String ConfigSection = "SSM Configuration";
        public const String StyleSection = "SSM Original Style";

        public class BreakpointException : Exception { }

        public Int32 CurrentTick => StateMachine?.Current ?? -1;

        public EntityState GridState { get; private set; } = EntityState.Default;
        public Double PowerThreshold { get; private set; } = 0.1;
        public Single Countdown { get; private set; } = 300;
        public Boolean Execute { get; private set; } = true;
        public Int32 MaxIOPs { get; private set; } = 0;
        public Double MaxTime { get; private set; } = 0;
        public Boolean UpdateNeeded { get; private set; } = false;

        private IEnumerator<Int32> StateMachine { get; set; }
        private IMyTextSurface TickStatSurface { get; set; }

        public Program()
        {
            Init();
        }

#if DEBUG
        public class DebugInfo
        {
            public IMyGridTerminalSystem GridTerminalSystem { get; set; }
            public IMyProgrammableBlock ProgrammableBlock { get; set; }
            public IMyGridProgramRuntimeInfo RuntimeInfo { get; set; }
        }

        public Program(DebugInfo debugInfo)
        {
            Me = debugInfo.ProgrammableBlock;
            Runtime = debugInfo.RuntimeInfo;
            GridTerminalSystem = debugInfo.GridTerminalSystem;
            Init();
        }
#endif

        private void Init()
        {
            Echo = message => { };
            StateMachine = StateMachineExecutor();

            TickStatSurface = Me.GetSurface(1);
            TickStatSurface.ContentType = ContentType.TEXT_AND_IMAGE;
            TickStatSurface.Font = "DEBUG";
            TickStatSurface.FontSize = 2.0f;

            QueueOnce();
        }

        public void Main(String argument, UpdateType updateType)
        {
            if (String.IsNullOrWhiteSpace(argument) && updateType.HasFlag(UpdateType.Once))
                StateMachineTick();

            if (!String.IsNullOrWhiteSpace(argument))
                ParseCommand(argument);
        }

        private void StateMachineTick()
        {
            if (StateMachine == null)
                return;

            if (Execute && StateMachine.MoveNext())
            {
                Echo($"Executed cycle {CurrentTick}.");
                QueueOnce();
            }
            else
            {
                Echo($"State machine has entered a stop state.");
                StateMachine.Reset();
            }

            UpdateTickStatistics();
        }

        private void QueueOnce() => Runtime.UpdateFrequency |= UpdateFrequency.Once;

        private void ParseCommand(String argument)
        {
            var words = argument.Split(' ');
            var command = words.ElementAtOrDefault(0).ToLower();

            switch (command)
            {
                case "activate":
                    switch (words.ElementAtOrDefault(1).ToLower())
                    {
                        case "":
                            Execute = true;
                            QueueOnce();
                            break;
                        case "battle":
                            if (!GridState.HasFlag(EntityState.Battle))
                            {
                                UpdateNeeded = true;
                                GridState |= EntityState.Battle;
                            }
                            break;
                        case "destruct":
                            if (!GridState.HasFlag(EntityState.Destruct))
                            {
                                UpdateNeeded = true;
                                GridState |= EntityState.Destruct;
                            }
                            break;
                    }
                    break;
                case "deactivate":
                    switch (words.ElementAtOrDefault(1).ToLower())
                    {
                        case "":
                            Execute = false;
                            break;
                        case "battle":
                            if (GridState.HasFlag(EntityState.Battle))
                            {
                                UpdateNeeded = true;
                                GridState &= ~EntityState.Battle;
                            }
                            break;
                        case "destruct":
                            if (GridState.HasFlag(EntityState.Destruct))
                            {
                                UpdateNeeded = true;
                                GridState &= ~EntityState.Destruct;
                            }
                            break;
                    }
                    break;
                case "toggle":
                    switch (words.ElementAtOrDefault(1).ToLower())
                    {
                        case "":
                            if (Execute)
                            {
                                Execute = false;
                            }
                            else
                            {
                                Execute = true;
                                QueueOnce();
                            }
                            break;
                        case "battle":
                            if (GridState.HasFlag(EntityState.Battle))
                            {
                                GridState &= ~EntityState.Battle;
                            }
                            else
                            {
                                GridState |= EntityState.Battle;
                            }
                            UpdateNeeded = true;
                            break;
                        case "destruct":
                            if (GridState.HasFlag(EntityState.Destruct))
                            {
                                GridState &= ~EntityState.Destruct;
                            }
                            else
                            {
                                GridState |= EntityState.Destruct;
                            }
                            UpdateNeeded = true;
                            break;
                    }
                    break;
                case "set":
                    switch (words.ElementAtOrDefault(1).ToLower())
                    {
                        case "lowpower":
                            var powerValue = words.ElementAtOrDefault(2);
                            Double powerParsed;

                            if (Double.TryParse(powerValue, out powerParsed) && powerParsed >= 0 && powerParsed < 1)
                            {
                                PowerThreshold = powerParsed;
                                Echo($"Low power threshold has been set to {powerParsed}.");
                            }
                            else
                            {
                                Echo($"Low power threshold must be a decimal value >= 0 < 1.");
                            }
                            break;
                        case "countdown":
                            var countdownValue = words.ElementAtOrDefault(2);
                            Single countdownParsed;

                            if (Single.TryParse(countdownValue, out countdownParsed) && countdownParsed > 0)
                            {
                                Countdown = countdownParsed;
                                Echo($"Self Destruct timer has been set to {TimeSpan.FromSeconds(Countdown)}.");
                            }
                            else
                            {
                                Echo($"Self Destruct timer must be a decimal value greater than 0.");
                            }
                            break;
                    }
                    break;
                case "group":
                    var verb = String.Join(" ", words.Skip(1).Take(2));
                    var parameters = String.Join(" ", words.Skip(3)).Split(';');

                    if (parameters.Count() == 0 || parameters.Count() > 2)
                        break;

                    var group = parameters.ElementAt(0);
                    var target = parameters.ElementAtOrDefault(1);

                    var blocks = new List<IMyTerminalBlock>();
                    var config = new MyIni();
                    var values = new List<String>();

                    GridTerminalSystem.GetBlockGroupWithName(group).GetBlocks(blocks);

                    if (!blocks.Any())
                        break;

                    switch (verb)
                    {
                        case "add function":
                        case "function add":
                            if (target == "")
                                break;

                            foreach (var block in blocks)
                            {
                                config.TryParse(block.CustomData);
                                config.Get(ConfigSection, "functions").GetLines(values);

                                if (!values.Contains(target))
                                {
                                    values.Add(target);
                                    config.Set(ConfigSection, "functions", String.Join("\n", values));

                                    block.CustomData = config.ToString();
                                }
                            }
                            break;
                        case "remove function":
                        case "function remove":
                            foreach (var block in blocks)
                            {
                                config.TryParse(block.CustomData);
                                config.Get(ConfigSection, "functions").GetLines(values);

                                if (target == "")
                                {
                                    config.Delete(ConfigSection, "functions");
                                    block.CustomData = config.ToString();
                                }
                                else if (values.Contains(target))
                                {
                                    values.Remove(target);
                                    config.Set(ConfigSection, "functions", String.Join("\n", values));

                                }
                            }
                            break;
                        case "add zone":
                        case "zone add":
                            if (target == "")
                                break;

                            foreach (var block in blocks)
                            {
                                config.TryParse(block.CustomData);
                                config.Get(ConfigSection, "zones").GetLines(values);

                                if (!values.Contains(target))
                                {
                                    values.Add(target);
                                    config.Set(ConfigSection, "zones", String.Join("\n", values));

                                    block.CustomData = config.ToString();
                                }
                            }
                            break;
                        case "remove zone":
                        case "zone remove":
                            foreach (var block in blocks)
                            {
                                config.TryParse(block.CustomData);
                                config.Get(ConfigSection, "zones").GetLines(values);

                                if (target == "")
                                {
                                    config.Delete(ConfigSection, "zones");
                                    block.CustomData = config.ToString();
                                }
                                else if (values.Contains(target))
                                {
                                    values.Remove(target);
                                    config.Set(ConfigSection, "zones", String.Join("\n", values));

                                }
                            }
                            break;
                    }
                    break;
            }
        }

        private void UpdateTickStatistics()
        {
            var builder = new StringBuilder();

            MaxTime = Math.Max(MaxTime, Runtime.LastRunTimeMs);
            MaxIOPs = Math.Max(MaxIOPs, Runtime.CurrentInstructionCount);

            builder.AppendLine($"Time: {Runtime.LastRunTimeMs}ms ({MaxTime}ms max)");
            builder.AppendLine($"IOPS: {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount}, ({MaxIOPs} max)");
            builder.AppendLine($"Tick: {CurrentTick}/7");

            TickStatSurface.DrawScaledSpriteText(builder, TickStatSurface.Font, TickStatSurface.FontColor);
        }
    }
}