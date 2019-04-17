// <mdk sortorder="2" />
#pragma warning disable S112
using Sandbox.ModAPI.Ingame;
using System.Linq;
using System;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public const String IniSection = "SSM Configuration";
        public const String IniStyleSection = "SSM Original Style";

#pragma warning disable S3925
        public class BreakpointException : Exception { }
#pragma warning disable S3925

        public Int32 CurrentTick => StateMachine?.Current ?? -1;

        public EntityState GridState { get; private set; } = EntityState.Default;
        public Double PowerThreshold { get; private set; } = 0.1;
        public Single Countdown { get; private set; } = 300;

        private IEnumerator<Int32> StateMachine { get; set; }

        public Program()
        {
            var output = Echo;
            Echo = message => output($"[{DateTime.Now:HH:mm:ss}] {message}");

            StateMachine = StateMachineExecutor();
            Runtime.UpdateFrequency |= UpdateFrequency.Once;
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

            if (StateMachine.MoveNext())
            {
                Echo($"Executed cycle {CurrentTick}.");
                Runtime.UpdateFrequency |= UpdateFrequency.Once;
            }
            else
            {
                Echo($"State machine has entered a stop state.");
                StateMachine.Dispose();
                StateMachine = null;
            }
        }

        private void ParseCommand(String argument)
        {
            var words = argument.Split(' ');
            var command = words.ElementAtOrDefault(0).ToLower();

            switch (command)
            {
                case "activate":
                    switch (words.ElementAtOrDefault(1).ToLower())
                    {
                        case "battle":
                            GridState |= EntityState.Battle;
                            break;
                        case "destruct":
                            GridState |= EntityState.Destruct;
                            break;
                    }
                    break;
                case "deactivate":
                    switch (words.ElementAtOrDefault(1).ToLower())
                    {
                        case "battle":
                            GridState &= ~EntityState.Battle;
                            break;
                        case "destruct":
                            GridState &= ~EntityState.Destruct;
                            break;
                    }
                    break;
                case "toggle":
                    switch (words.ElementAtOrDefault(1).ToLower())
                    {
                        case "battle":
                            if (GridState.HasFlag(EntityState.Battle))
                            {
                                GridState &= ~EntityState.Battle;
                            }
                            else
                            {
                                GridState |= EntityState.Battle;
                            }
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
                            break;
                    }
                    break;
                case "set":
                    switch (words.ElementAtOrDefault(1).ToLower())
                    {
                        case "lowpower":
                            var value = words.ElementAtOrDefault(2);
                            var parsed = PowerThreshold;

                            if (Double.TryParse(value, out parsed) && parsed > 0 && parsed < 1)
                            {
                                PowerThreshold = parsed;
                                Echo($"Low power threshold has been set to {parsed}.");
                            }
                            else
                            {
                                Echo($"Low power threshold must be a decimal value between 0 and 1.");
                            }
                            break;
                    }
                    break;
            }
        }
    }
}