// <mdk sortorder="0" />
using Sandbox.ModAPI.Ingame;
using System.Linq;
using System;
using System.Collections.Generic;

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

        private IEnumerator<Int32> StateMachine { get; set; }

        public Program()
        {
            Echo = message => { };
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
                        case "":
                            Execute = true;
                            Runtime.UpdateFrequency |= UpdateFrequency.Once;
                            break;
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
                        case "":
                            Execute = false;
                            Runtime.UpdateFrequency |= ~UpdateFrequency.Once;
                            break;
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
                        case "":
                            if (Execute)
                            {
                                Execute = false;
                                Runtime.UpdateFrequency |= ~UpdateFrequency.Once;
                            }
                            else
                            {
                                Execute = true;
                                Runtime.UpdateFrequency |= UpdateFrequency.Once;
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
                            var powerValue = words.ElementAtOrDefault(2);
                            var powerParsed = PowerThreshold;

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
                            var countdownParsed = Countdown;

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
            }
        }
    }
}