using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    public partial class Program
    {
        private void StyleBlock<T>(Block<T> block, EntityState states, Single countdown)
            where T : IMyTerminalBlock
        {
            if (StyleDecompression(block, states))
                return;

            if (StyleSelfDestruct(block, states, countdown))
                return;

            if (StyleIntruderAlert(block, states))
                return;

            if (StyleBattleStations(block, states))
                return;

            if (StyleLowPower(block, states))
                return;

            StyleRestore(block);
        }

        private Boolean StyleDecompression<T>(Block<T> block, EntityState states)
            where T : IMyTerminalBlock
        {
            if (!states.HasFlag(EntityState.Decompress))
                return false;

            if (typeof(T) == typeof(IMyTextPanel) && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block as Block<IMyTextPanel>, textPanel =>
                {
                    textPanel.Font = "Debug";
                    textPanel.FontColor = Colors.Blue;
                    textPanel.BackgroundColor = Colors.Black;
                    textPanel.Alignment = TextAlignment.CENTER;

                    if (textPanel.IsWideScreen())
                        textPanel.WriteAndScaleText("DECOMPRESSION");
                    else
                        textPanel.WriteAndScaleText("DECOMPRESSION\nDO NOT ENTER");
                });

                return true;
            }

            if (typeof(T) == typeof(IMyLightingBlock) && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block as Block<IMyLightingBlock>, lightingBlock =>
                {
                    lightingBlock.Color = Colors.Blue;
                    lightingBlock.BlinkIntervalSeconds = 3.5f;
                    lightingBlock.BlinkLength = 0.6f;
                });

                return true;
            }

            if (typeof(T) == typeof(IMyDoor) && block.Functions.HasFlag(BlockFunction.AirlockDoor))
            {
                SaveAndApply(block as Block<IMyDoor>, door =>
                {
                    door.ToggleOpenAndEnable(false, false);
                });

                return true;
            }

            return false;
        }

        private Boolean StyleSelfDestruct<T>(Block<T> block, EntityState states, Single countdown)
            where T : IMyTerminalBlock
        {
            if (states.HasFlag(EntityState.Destruct))
            {
                if (typeof(T) == typeof(IMyWarhead) && block.Functions.HasFlag(BlockFunction.SelfDestruct))
                {
                    SaveAndApply(block as Block<IMyWarhead>, warhead =>
                    {
                        warhead.DetonationTime = countdown;
                        warhead.IsArmed = true;

                        if (!warhead.IsCountingDown)
                            warhead.StartCountdown();
                    });

                    return true;
                }

                if (typeof(T) == typeof(IMyTextPanel) && block.Functions.HasFlag(BlockFunction.Alert))
                {
                    var timer = TimeSpan.FromSeconds(countdown);
                    var text = "SELF DESTRUCT{0}";

                    if (timer.TotalMinutes < 1)
                        text += $"{timer:ss.fffffff}";
                    else
                        text += $"{timer:mm:ss}";

                    SaveAndApply(block as Block<IMyTextPanel>, textPanel =>
                    {
                        if (textPanel.IsWideScreen())
                            text = text.Replace("{0}", ": ");
                        else
                            text = text.Replace("{0}", "\n\n");

                        textPanel.Font = "Debug";
                        textPanel.FontColor = Colors.Red;
                        textPanel.BackgroundColor = Colors.Black;
                        textPanel.Alignment = TextAlignment.CENTER;
                        textPanel.TextPadding = 10f;

                        textPanel.WriteAndScaleText(text);
                    });

                    return true;
                }

                if (typeof(T) == typeof(IMyLightingBlock) && block.Functions.HasFlag(BlockFunction.Alert))
                {
                    SaveAndApply(block as Block<IMyLightingBlock>, lightingBlock =>
                    {
                        lightingBlock.Color = Colors.Red;
                        lightingBlock.BlinkIntervalSeconds = 3;
                        lightingBlock.BlinkLength = 0.5f;
                    });

                    return true;
                }

                if (typeof(T) == typeof(IMySoundBlock) && block.Functions.HasFlag(BlockFunction.Alert))
                {
                    SaveAndApply(block as Block<IMySoundBlock>, soundBlock =>
                    {
                        soundBlock.PlaySound("Alert 1");
                        soundBlock.LoopPeriod = countdown;
                    });
                }
            }
            else if (typeof(T) == typeof(IMyWarhead) && block.Functions.HasFlag(BlockFunction.SelfDestruct))
            {
                Restore(block as Block<IMyWarhead>, warhead =>
                {
                    if (warhead.IsCountingDown)
                        warhead.StopCountdown();

                    warhead.IsArmed = false;
                });
            }

            return false;
        }

        private Boolean StyleBattleStations<T>(Block<T> block, EntityState states)
            where T : IMyTerminalBlock
        {
            if (!states.HasFlag(EntityState.Battle))
                return false;

            if (typeof(T) == typeof(IMyTextPanel) && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block as Block<IMyTextPanel>, textPanel =>
                {
                    textPanel.Font = "Debug";
                    textPanel.FontColor = Colors.Red;
                    textPanel.BackgroundColor = Colors.Black;
                    textPanel.Alignment = TextAlignment.CENTER;
                    textPanel.TextPadding = 10f;

                    if (textPanel.IsWideScreen())
                        textPanel.WriteAndScaleText("BATTLE STATIONS");
                    else
                        textPanel.WriteAndScaleText("BATTLE\nSTATIONS");
                });

                return true;
            }

            if (typeof(T) == typeof(IMyLightingBlock) && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block as Block<IMyLightingBlock>, lightingBlock =>
                {
                    lightingBlock.Color = Colors.Red;
                    lightingBlock.BlinkIntervalSeconds = 2.5f;
                    lightingBlock.BlinkLength = 0.5f;
                });

                return true;
            }

            if (typeof(T) == typeof(IMyDoor) && block.Functions.HasFlag(BlockFunction.Door))
            {
                SaveAndApply(block as Block<IMyDoor>, door =>
                {
                    door.ToggleOpenAndEnable(false, true);
                });

                return true;
            }

            return false;
        }

        private Boolean StyleIntruderAlert<T>(Block<T> block, EntityState states)
            where T : IMyTerminalBlock
        {
            if (!states.HasFlag(EntityState.Intruder))
                return false;

            if (typeof(T) == typeof(IMyTextPanel) && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block as Block<IMyTextPanel>, textPanel =>
                {
                    textPanel.Font = "Debug";
                    textPanel.FontColor = Colors.Orange;
                    textPanel.BackgroundColor = Colors.Black;
                    textPanel.Alignment = TextAlignment.CENTER;
                    textPanel.TextPadding = 10f;

                    if (textPanel.IsWideScreen())
                        textPanel.WriteAndScaleText("INTRUDER");
                    else
                        textPanel.WriteAndScaleText("INTRUDER\nALERT");
                });

                return true;
            }

            if (typeof(T) == typeof(IMyLightingBlock) && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block as Block<IMyLightingBlock>, lightingBlock =>
                {
                    lightingBlock.Color = Colors.Orange;
                    lightingBlock.BlinkIntervalSeconds = 2f;
                    lightingBlock.BlinkLength = 0.75f;
                });

                return true;
            }

            if (typeof(T) == typeof(IMyDoor) && block.Functions.HasFlag(BlockFunction.SecurityDoor))
            {
                SaveAndApply(block as Block<IMyDoor>, door =>
                {
                    door.ToggleOpenAndEnable(false, true);
                });

                return true;
            }

            return false;
        }

        private Boolean StyleLowPower<T>(Block<T> block, EntityState states)
            where T : IMyTerminalBlock
        {
            if (!states.HasFlag(EntityState.LowPower))
                return false;

            if (typeof(T) == typeof(IMyLightingBlock))
            {
                SaveAndApply(block as Block<IMyLightingBlock>, lightingBlock =>
                {
                    if (block.Functions.HasFlag(BlockFunction.LowPower))
                    {
                        lightingBlock.Intensity = lightingBlock.Intensity * 0.7f;
                    }
                    else
                    {
                        lightingBlock.Enabled = false;
                    }
                });

                return true;
            }

            if (typeof(T) == typeof(IMyAssembler) && !block.Functions.HasFlag(BlockFunction.LowPower))
            {
                SaveAndApply(block as Block<IMyAssembler>, assembler =>
                {
                    assembler.Enabled = false;
                });

                return true;
            }

            if (typeof(T) == typeof(IMyRefinery) && !block.Functions.HasFlag(BlockFunction.LowPower))
            {
                SaveAndApply(block as Block<IMyRefinery>, refinery =>
                {
                    refinery.Enabled = false;
                });

                return true;
            }

            return false;
        }

        private void StyleRestore<T>(Block<T> block, Action<T> then = null)
            where T : IMyTerminalBlock
        {
            if (typeof(T) == typeof(IMyWarhead))
            {
                Restore(block as Block<IMyWarhead>, then as Action<IMyWarhead>);
                return;
            }

            if (typeof(T) == typeof(IMyTextPanel))
            {
                Restore(block as Block<IMyTextPanel>, then as Action<IMyTextPanel>);
                return;
            }

            if (typeof(T) == typeof(IMyLightingBlock))
            {
                Restore(block as Block<IMyLightingBlock>, then as Action<IMyLightingBlock>);
                return;
            }

            if (typeof(T) == typeof(IMyDoor))
            {
                Restore(block as Block<IMyDoor>, then as Action<IMyDoor>);
                return;
            }

            if (typeof(T) == typeof(IMyAssembler))
            {
                Restore(block as Block<IMyAssembler>, then as Action<IMyAssembler>);
                return;
            }

            if (typeof(T) == typeof(IMyRefinery))
            {
                Restore(block as Block<IMyRefinery>, then as Action<IMyRefinery>);
                return;
            }
        }

        #region Config Savers

        private void SaveAndApply(Block<IMyWarhead> warhead, Action<IMyWarhead> action)
        {
            if (warhead == null)
                return;

            if (!warhead.HasStyle)
            {
                warhead.SetStyle(nameof(IMyWarhead.IsArmed), warhead.Target.IsArmed);
                warhead.SetStyle(nameof(IMyWarhead.DetonationTime), warhead.Target.DetonationTime);
            }

            action(warhead.Target);
        }

        private void SaveAndApply(Block<IMyTextPanel> textPanel, Action<IMyTextPanel> action)
        {
            if (!textPanel.HasStyle)
            {
                textPanel.SetStyle("Text", textPanel.Target.GetText());
                textPanel.SetStyle(nameof(IMyTextPanel.Enabled), textPanel.Target.Enabled);
                textPanel.SetStyle(nameof(IMyTextPanel.FontSize), textPanel.Target.FontSize);
                textPanel.SetStyle(nameof(IMyTextPanel.Font), textPanel.Target.Font);
                textPanel.SetStyle(nameof(IMyTextPanel.Alignment), textPanel.Target.Alignment.ToString());
                textPanel.SetStyle(nameof(IMyTextPanel.FontColor), textPanel.Target.FontColor);
                textPanel.SetStyle(nameof(IMyTextPanel.BackgroundColor), textPanel.Target.BackgroundColor);
                textPanel.SetStyle(nameof(IMyTextPanel.ContentType), textPanel.Target.ContentType.ToString());
            }

            action(textPanel.Target);
        }

        private void SaveAndApply(Block<IMyLightingBlock> lightingBlock, Action<IMyLightingBlock> action)
        {
            if (!lightingBlock.HasStyle)
            {
                lightingBlock.SetStyle(nameof(IMyLightingBlock.Enabled), lightingBlock.Target.Enabled);
                lightingBlock.SetStyle(nameof(IMyLightingBlock.BlinkIntervalSeconds), lightingBlock.Target.BlinkIntervalSeconds);
                lightingBlock.SetStyle(nameof(IMyLightingBlock.BlinkLength), lightingBlock.Target.BlinkLength);
                lightingBlock.SetStyle(nameof(IMyLightingBlock.BlinkOffset), lightingBlock.Target.BlinkOffset);
                lightingBlock.SetStyle(nameof(IMyLightingBlock.Color), lightingBlock.Target.Color);
                lightingBlock.SetStyle(nameof(IMyLightingBlock.Falloff), lightingBlock.Target.Falloff);
                lightingBlock.SetStyle(nameof(IMyLightingBlock.Intensity), lightingBlock.Target.Intensity);
                lightingBlock.SetStyle(nameof(IMyLightingBlock.Radius), lightingBlock.Target.Radius);
            }

            action(lightingBlock.Target);
        }


        private void SaveAndApply(Block<IMyDoor> door, Action<IMyDoor> action)
        {
            if (!door.HasStyle)
            {
                door.SetStyle(nameof(IMyDoor.Enabled), door.Target.Enabled);

                switch (door.Target.Status)
                {
                    case DoorStatus.Open:
                    case DoorStatus.Opening:
                        door.SetStyle("Open", true);
                        break;
                    default:
                        door.SetStyle("Open", false);
                        break;
                }
            }

            action(door.Target);
        }

        private void SaveAndApply(Block<IMySoundBlock> soundBlock, Action<IMySoundBlock> action)
        {
            soundBlock.SetStyle(nameof(IMySoundBlock.Enabled), soundBlock.Target.Enabled);
            soundBlock.SetStyle(nameof(IMySoundBlock.SelectedSound), soundBlock.Target.SelectedSound);
            soundBlock.SetStyle(nameof(IMySoundBlock.Volume), soundBlock.Target.Volume);
            soundBlock.SetStyle(nameof(IMySoundBlock.Range), soundBlock.Target.Range);
            soundBlock.SetStyle(nameof(IMySoundBlock.LoopPeriod), soundBlock.Target.LoopPeriod);

            action(soundBlock.Target);
        }

        private void SaveAndApply(Block<IMyAssembler> assembler, Action<IMyAssembler> action)
        {
            assembler.SetStyle(nameof(IMyAssembler.Enabled), assembler.Target.Enabled);

            action(assembler.Target);
        }

        private void SaveAndApply(Block<IMyRefinery> refinery, Action<IMyRefinery> action)
        {
            refinery.SetStyle(nameof(IMyRefinery.Enabled), refinery.Target.Enabled);

            action(refinery.Target);
        }

        #endregion

        #region Config Restorers

        private void Restore(Block<IMyWarhead> warhead, Action<IMyWarhead> then = null)
        {
            warhead.Target.IsArmed = warhead.GetBooleanStyle(nameof(IMyWarhead.IsArmed));
            warhead.Target.DetonationTime = warhead.GetSingleStyle(nameof(IMyWarhead.DetonationTime), Countdown);

            then?.Invoke(warhead.Target);
        }

        private void Restore(Block<IMyTextPanel> textPanel, Action<IMyTextPanel> then = null)
        {
            textPanel.Target.Enabled = textPanel.GetBooleanStyle(nameof(IMyTextPanel.Enabled), true);
            textPanel.Target.WriteText(textPanel.GetStringStyle("Text"));
            textPanel.Target.FontSize = textPanel.GetSingleStyle(nameof(IMyTextPanel.FontSize), 1f);
            textPanel.Target.Font = textPanel.GetStringStyle(nameof(IMyTextPanel.Font), "Debug");
            textPanel.Target.Alignment = textPanel.GetEnumStyle<TextAlignment>(nameof(IMyTextPanel.Alignment), TextAlignment.LEFT);
            textPanel.Target.FontColor = textPanel.GetColorStyle(nameof(IMyTextPanel.FontColor), Colors.White);
            textPanel.Target.BackgroundColor = textPanel.GetColorStyle(nameof(IMyTextPanel.BackgroundColor));
            textPanel.Target.ContentType = textPanel.GetEnumStyle<ContentType>(nameof(IMyTextPanel.ContentType));

            then?.Invoke(textPanel.Target);
        }

        private void Restore(Block<IMyLightingBlock> lightingBlock, Action<IMyLightingBlock> then = null)
        {
            lightingBlock.Target.Enabled = lightingBlock.GetBooleanStyle(nameof(IMyLightingBlock.Enabled), true);
            lightingBlock.Target.BlinkIntervalSeconds = lightingBlock.GetSingleStyle(nameof(IMyLightingBlock.BlinkIntervalSeconds));
            lightingBlock.Target.BlinkLength = lightingBlock.GetSingleStyle(nameof(IMyLightingBlock.BlinkLength));
            lightingBlock.Target.BlinkOffset = lightingBlock.GetSingleStyle(nameof(IMyLightingBlock.BlinkOffset));
            lightingBlock.Target.Color = lightingBlock.GetColorStyle(nameof(IMyLightingBlock.Color), Colors.White);
            lightingBlock.Target.Falloff = lightingBlock.GetSingleStyle(nameof(IMyLightingBlock.Falloff));
            lightingBlock.Target.Intensity = lightingBlock.GetSingleStyle(nameof(IMyLightingBlock.Intensity));
            lightingBlock.Target.Radius = lightingBlock.GetSingleStyle(nameof(IMyLightingBlock.Radius));

            then?.Invoke(lightingBlock.Target);
        }

        private void Restore(Block<IMyDoor> door, Action<IMyDoor> then = null)
        {
            var enabled = door.GetBooleanStyle(nameof(IMyDoor.Enabled), true);
            var open = door.GetBooleanStyle("Open", false);

            door.Target.ToggleOpenAndEnable(open, enabled);
            then?.Invoke(door.Target);
        }

        private void Restore(Block<IMySoundBlock> soundBlock, Action<IMySoundBlock> then = null)
        {
            soundBlock.Target.Enabled = soundBlock.GetBooleanStyle(nameof(IMySoundBlock.Enabled), true);
            // TODO: Currently assuming that sound blocks are off by default.
            // https://support.keenswh.com/spaceengineers/general/topic/improvements-to-imysoundblock-interface
            var selectedSound = soundBlock.GetStringStyle(nameof(IMySoundBlock.SelectedSound));
            if (selectedSound != soundBlock.Target.SelectedSound)
            {
                soundBlock.Target.Stop();
                soundBlock.Target.SelectedSound = selectedSound;
            }
            soundBlock.Target.Volume = soundBlock.GetSingleStyle(nameof(IMySoundBlock.Volume), 1);
            soundBlock.Target.Range = soundBlock.GetSingleStyle(nameof(IMySoundBlock.Range), 1);
            soundBlock.Target.LoopPeriod = soundBlock.GetSingleStyle(nameof(IMySoundBlock.LoopPeriod));

            then?.Invoke(soundBlock.Target);
        }

        private void Restore(Block<IMyAssembler> assembler, Action<IMyAssembler> then = null)
        {
            assembler.Target.Enabled = assembler.GetBooleanStyle(nameof(IMyAssembler.Enabled), true);

            then?.Invoke(assembler.Target);
        }

        private void Restore(Block<IMyRefinery> refinery, Action<IMyRefinery> then = null)
        {
            refinery.Target.Enabled = refinery.GetBooleanStyle(nameof(IMyRefinery.Enabled), true);

            then?.Invoke(refinery.Target);
        }

        #endregion
    }
}
