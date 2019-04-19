using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    public partial class Program
    {
        private void StyleBlock(Block<IMyTerminalBlock> block, EntityState states, Single countdown)
        {
            if (StyleDecompression(block, states))
            {
                Echo("Decompression styles applied.");
                return;
            }

            if (StyleSelfDestruct(block, states, countdown))
            {
                Echo("Self Destruct styles applied.");
                return;
            }

            if (StyleIntruderAlert(block, states))
            {
                Echo("Intruder Alert styles applied.");
                return;
            }

            if (StyleBattleStations(block, states))
            {
                Echo("Battle Stations styles applied.");
                return;
            }

            if (StyleLowPower(block, states))
            {
                Echo("Low Power styles applied.");
                return;
            }

            StyleRestore(block);
            Echo("Default styles applied.");
        }

        private Boolean StyleDecompression(Block<IMyTerminalBlock> block, EntityState states)
        {
            if (!states.HasFlag(EntityState.Decompress))
                return false;

            if (block.Target is IMyTextPanel && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block, block.Target as IMyTextPanel, textPanel =>
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

            if (block.Target is IMyTextSurfaceProvider)
            {
                var provider = block.Target as IMyTextSurfaceProvider;

                provider.ForEachSurface((surface, i) =>
                {
                    var function = block.GetEnumConfig<BlockFunction>($"functions-{i}", BlockFunction.None);
                    if (function.HasFlag(BlockFunction.Alert))
                    {
                        SaveAndApply(block, surface, i, _ =>
                        {
                            surface.Font = "Debug";
                            surface.FontColor = Colors.Blue;
                            surface.BackgroundColor = Colors.Black;
                            surface.Alignment = TextAlignment.CENTER;

                            if (surface.IsWideScreen())
                                surface.WriteAndScaleText("DECOMPRESSION");
                            else
                                surface.WriteAndScaleText("DECOMPRESSION\nDO NOT ENTER");
                        });
                    }
                });

                return true;
            }

            if (block.Target is IMyLightingBlock)
            {
                SaveAndApply(block, block.Target as IMyLightingBlock, lightingBlock =>
                {
                    if (block.Functions.HasFlag(BlockFunction.Alert))
                    {
                        lightingBlock.Enabled = true;
                        lightingBlock.Color = Colors.Blue;
                        lightingBlock.BlinkIntervalSeconds = 3.5f;
                        lightingBlock.BlinkLength = 60f;

                    }
                    else
                    {
                        lightingBlock.Enabled = false;
                    }
                });

                return true;
            }

            if (block.Target is IMyDoor && block.Functions.HasFlag(BlockFunction.AirlockDoor))
            {
                SaveAndApply(block, block.Target as IMyDoor, door =>
                {
                    door.ToggleOpenAndEnable(false, false);
                });

                return true;
            }

            if (block.Target is IMySoundBlock && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block, block.Target as IMySoundBlock, soundBlock =>
                {
                    soundBlock.PlaySound("Alert 3");
                    soundBlock.LoopPeriod = 600;
                });

                return true;
            }

            return false;
        }

        private Boolean StyleSelfDestruct(Block<IMyTerminalBlock> block, EntityState states, Single countdown)
        {
            if (states.HasFlag(EntityState.Destruct))
            {
                if (countdown >= 0 && block.Target is IMyWarhead && block.Functions.HasFlag(BlockFunction.SelfDestruct))
                {
                    SaveAndApply(block, block.Target as IMyWarhead, warhead =>
                    {
                        warhead.DetonationTime = countdown;
                        warhead.IsArmed = true;

                        if (!warhead.IsCountingDown)
                            warhead.StartCountdown();
                    });

                    return true;
                }

                if (block.Target is IMyTextPanel && block.Functions.HasFlag(BlockFunction.Alert))
                {
                    var text = "SELF DESTRUCT{0}";
                    if (countdown >= 0)
                    {
                        var timer = TimeSpan.FromSeconds(countdown);

                        if (timer.TotalMinutes < 1)
                            text += $"{timer:ss.fffffff}";
                        else
                            text += $"{timer:mm:ss}";
                    }
                    else
                    {
                        text += "UNAVAILABLE";
                    }

                    SaveAndApply(block, block.Target as IMyTextPanel, textPanel =>
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

                if (block.Target is IMyTextSurfaceProvider)
                {
                    var text = "SELF DESTRUCT{0}";
                    if (countdown >= 0)
                    {
                        var timer = TimeSpan.FromSeconds(countdown);

                        if (timer.TotalMinutes < 1)
                            text += $"{timer:ss.fffffff}";
                        else
                            text += $"{timer:mm:ss}";
                    }
                    else
                    {
                        text += "UNAVAILABLE";
                    }

                    var provider = block.Target as IMyTextSurfaceProvider;
                    var i = 0;

                    provider.ForEachSurface((surface, index) =>
                    {
                        var function = block.GetEnumConfig<BlockFunction>($"functions-{i}", BlockFunction.None);
                        if (function.HasFlag(BlockFunction.Alert))
                        {
                            SaveAndApply(block, surface, i, _ =>
                            {
                                if (surface.IsWideScreen())
                                    text = text.Replace("{0}", ": ");
                                else
                                    text = text.Replace("{0}", "\n\n");

                                surface.Font = "Debug";
                                surface.FontColor = Colors.Red;
                                surface.BackgroundColor = Colors.Black;
                                surface.Alignment = TextAlignment.CENTER;
                                surface.TextPadding = 10f;

                                surface.WriteAndScaleText(text);
                            });
                        }
                    });

                    return true;
                }

                if (block.Target is IMyLightingBlock)
                {
                    SaveAndApply(block, block.Target as IMyLightingBlock, lightingBlock =>
                    {
                        if (block.Functions.HasFlag(BlockFunction.Alert))
                        {
                            lightingBlock.Color = Colors.Red;
                            lightingBlock.BlinkIntervalSeconds = 3;
                            lightingBlock.BlinkLength = 50f;
                        }
                        else
                        {
                            lightingBlock.Enabled = false;
                        }
                    });

                    return true;
                }

                if (block.Target is IMySoundBlock && block.Functions.HasFlag(BlockFunction.Alert))
                {
                    SaveAndApply(block, block.Target as IMySoundBlock, soundBlock =>
                    {
                        soundBlock.PlaySound("Alert 1");
                        soundBlock.LoopPeriod = countdown;
                    });

                    return true;
                }
            }
            else if (block.Target is IMyWarhead && block.Functions.HasFlag(BlockFunction.SelfDestruct))
            {
                var warhead = block.Target as IMyWarhead;
                Restore(block, warhead);

                if (warhead.IsCountingDown)
                    warhead.StopCountdown();

                warhead.IsArmed = false;
            }

            return false;
        }

        private Boolean StyleBattleStations(Block<IMyTerminalBlock> block, EntityState states)
        {
            if (!states.HasFlag(EntityState.Battle))
                return false;

            if (block.Target is IMyTextPanel && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block, block.Target as IMyTextPanel, textPanel =>
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

            if (block.Target is IMyTextSurfaceProvider)
            {
                var provider = block.Target as IMyTextSurfaceProvider;

                provider.ForEachSurface((surface, i) =>
                {
                    var function = block.GetEnumConfig<BlockFunction>($"functions-{i}", BlockFunction.None);
                    if (function.HasFlag(BlockFunction.Alert))
                    {
                        SaveAndApply(block, surface, i, _ =>
                        {
                            surface.Font = "Debug";
                            surface.FontColor = Colors.Red;
                            surface.BackgroundColor = Colors.Black;
                            surface.Alignment = TextAlignment.CENTER;
                            surface.TextPadding = 10f;

                            if (surface.IsWideScreen())
                                surface.WriteAndScaleText("BATTLE STATIONS");
                            else
                                surface.WriteAndScaleText("BATTLE\nSTATIONS");
                        });
                    }
                });

                return true;
            }

            if (block.Target is IMyLightingBlock)
            {
                SaveAndApply(block, block.Target as IMyLightingBlock, lightingBlock =>
                {
                    if (block.Functions.HasFlag(BlockFunction.Alert))
                    {
                        lightingBlock.Color = Colors.Red;
                        lightingBlock.BlinkIntervalSeconds = 2.5f;
                        lightingBlock.BlinkLength = 20f;
                    }
                    else
                    {
                        lightingBlock.Intensity = block.GetSingleStyle(nameof(lightingBlock.Intensity), 1) * 0.4f;
                    }
                });

                return true;
            }

            if (block.Target is IMyDoor && block.Functions.HasFlag(BlockFunction.Door))
            {
                SaveAndApply(block, block.Target as IMyDoor, door =>
                {
                    door.ToggleOpenAndEnable(false, true);
                });

                return true;
            }

            if (block.Target is IMySoundBlock && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block, block.Target as IMySoundBlock, soundBlock =>
                {
                    soundBlock.PlaySound("Alert 2");
                    soundBlock.LoopPeriod = 600;
                });

                return true;
            }

            return false;
        }

        private Boolean StyleIntruderAlert(Block<IMyTerminalBlock> block, EntityState states)
        {
            if (!states.HasFlag(EntityState.Intruder))
                return false;

            if (block.Target is IMyTextPanel && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block, block.Target as IMyTextPanel, textPanel =>
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

            if (block.Target is IMyTextSurfaceProvider)
            {
                var provider = block.Target as IMyTextSurfaceProvider;

                provider.ForEachSurface((surface, i) =>
                {
                    var function = block.GetEnumConfig<BlockFunction>($"functions-{i}", BlockFunction.None);
                    if (function.HasFlag(BlockFunction.Alert))
                    {
                        SaveAndApply(block, surface, i, _ =>
                        {
                            surface.Font = "Debug";
                            surface.FontColor = Colors.Orange;
                            surface.BackgroundColor = Colors.Black;
                            surface.Alignment = TextAlignment.CENTER;
                            surface.TextPadding = 10f;

                            if (surface.IsWideScreen())
                                surface.WriteAndScaleText("INTRUDER");
                            else
                                surface.WriteAndScaleText("INTRUDER\nALERT");
                        });
                    }
                });

                return true;
            }

            if (block.Target is IMyLightingBlock)
            {
                SaveAndApply(block, block.Target as IMyLightingBlock, lightingBlock =>
                {
                    if (block.Functions.HasFlag(BlockFunction.Alert))
                    {
                        lightingBlock.Color = Colors.Orange;
                        lightingBlock.BlinkIntervalSeconds = 2f;
                        lightingBlock.BlinkLength = 75.0f;
                    }
                    else
                    {
                        lightingBlock.Enabled = false;
                    }
                });

                return true;
            }

            if (block.Target is IMyDoor && block.Functions.HasFlag(BlockFunction.SecurityDoor))
            {
                SaveAndApply(block, block.Target as IMyDoor, door =>
                {
                    door.ToggleOpenAndEnable(false, true);
                });

                return true;
            }

            if (block.Target is IMySoundBlock && block.Functions.HasFlag(BlockFunction.Alert))
            {
                SaveAndApply(block, block.Target as IMySoundBlock, soundBlock =>
                {
                    soundBlock.PlaySound("Enemy Detected");
                    soundBlock.LoopPeriod = 4;
                });

                return true;
            }

            return false;
        }

        private Boolean StyleLowPower(Block<IMyTerminalBlock> block, EntityState states)
        {
            if (!states.HasFlag(EntityState.LowPower))
                return false;

            if (block.Target is IMyLightingBlock)
            {
                SaveAndApply(block, block.Target as IMyLightingBlock, lightingBlock =>
                {
                    if (block.Functions.HasFlag(BlockFunction.LowPower))
                    {
                        lightingBlock.Intensity = block.GetSingleStyle(nameof(lightingBlock.Intensity), 1) * 0.7f;
                    }
                    else
                    {
                        lightingBlock.Enabled = false;
                    }
                });

                return true;
            }

            if (block.Target is IMyAssembler && !block.Functions.HasFlag(BlockFunction.LowPower))
            {
                SaveAndApply(block, block.Target as IMyAssembler, assembler =>
                {
                    assembler.Enabled = false;
                });

                return true;
            }

            if (block.Target is IMyRefinery && !block.Functions.HasFlag(BlockFunction.LowPower))
            {
                SaveAndApply(block, block.Target as IMyRefinery, refinery =>
                {
                    refinery.Enabled = false;
                });

                return true;
            }

            return false;
        }

        private void StyleRestore(Block<IMyTerminalBlock> block)
        {
            if (block.HasStyle)
            {
                if (block.Target is IMyWarhead)
                {
                    Restore(block, block.Target as IMyWarhead);
                    return;
                }

                if (block.Target is IMyTextPanel)
                {
                    Restore(block, block.Target as IMyTextPanel);
                    return;
                }

                if (block.Target is IMyTextSurfaceProvider)
                {
                    var provider = block.Target as IMyTextSurfaceProvider;

                    provider.ForEachSurface((surface, i) => Restore(block, surface, i));
                    return;
                }

                if (block.Target is IMyLightingBlock)
                {
                    Restore(block, block.Target as IMyLightingBlock);
                    return;
                }

                if (block.Target is IMyDoor)
                {
                    Restore(block, block.Target as IMyDoor);
                    return;
                }

                if (block.Target is IMyAssembler)
                {
                    Restore(block, block.Target as IMyAssembler);
                    return;
                }

                if (block.Target is IMyRefinery)
                {
                    Restore(block, block.Target as IMyRefinery);
                    return;
                }

                if (block.Target is IMySoundBlock)
                {
                    Restore(block, block.Target as IMySoundBlock);
                    return;
                }
            }
        }

        #region Config Savers

        private void SaveAndApply(IBlockConfiguration block, IMyWarhead warhead, Action<IMyWarhead> action)
        {
            if (block == null)
                return;

            if (!block.HasStyle)
            {
                block.SetStyle(nameof(IMyWarhead.IsArmed), warhead.IsArmed);
                block.SetStyle(nameof(IMyWarhead.DetonationTime), warhead.DetonationTime);
            }

            action(warhead);
        }

        private void SaveAndApply(IBlockConfiguration block, IMyTextPanel textPanel, Action<IMyTextPanel> action)
        {
            if (!block.HasStyle)
            {
                SaveAndApply(block, textPanel, 0, action);
                block.SetStyle(nameof(IMyTextPanel.Enabled), textPanel.Enabled);
            }
        }

        private void SaveAndApply<T>(IBlockConfiguration provider, T surface, Int32 index, Action<T> action = null)
            where T : IMyTextSurface
        {
            if (!provider.HasStyle)
            {
                provider.SetStyle($"Surface {index} Text", surface.GetText());
                provider.SetStyle($"Surface {index} {nameof(IMyTextSurface.FontSize)}", surface.FontSize);
                provider.SetStyle($"Surface {index} {nameof(IMyTextSurface.Font)}", surface.Font);
                provider.SetStyle($"Surface {index} {nameof(IMyTextSurface.Alignment)}", surface.Alignment.ToString());
                provider.SetStyle($"Surface {index} {nameof(IMyTextSurface.FontColor)}", surface.FontColor);
                provider.SetStyle($"Surface {index} {nameof(IMyTextSurface.BackgroundColor)}", surface.BackgroundColor);
                provider.SetStyle($"Surface {index} {nameof(IMyTextSurface.ContentType)}", surface.ContentType.ToString());
            }

            action?.Invoke(surface);
        }

        private void SaveAndApply(IBlockConfiguration block, IMyLightingBlock lightingBlock, Action<IMyLightingBlock> action)
        {
            if (!block.HasStyle)
            {
                block.SetStyle(nameof(IMyLightingBlock.Enabled), lightingBlock.Enabled);
                block.SetStyle(nameof(IMyLightingBlock.BlinkIntervalSeconds), lightingBlock.BlinkIntervalSeconds);
                block.SetStyle(nameof(IMyLightingBlock.BlinkLength), lightingBlock.BlinkLength);
                block.SetStyle(nameof(IMyLightingBlock.BlinkOffset), lightingBlock.BlinkOffset);
                block.SetStyle(nameof(IMyLightingBlock.Color), lightingBlock.Color);
                block.SetStyle(nameof(IMyLightingBlock.Falloff), lightingBlock.Falloff);
                block.SetStyle(nameof(IMyLightingBlock.Intensity), lightingBlock.Intensity);
                block.SetStyle(nameof(IMyLightingBlock.Radius), lightingBlock.Radius);
            }

            action(lightingBlock);
        }

        private void SaveAndApply(IBlockConfiguration block, IMyDoor door, Action<IMyDoor> action)
        {
            if (!block.HasStyle)
            {
                block.SetStyle(nameof(IMyDoor.Enabled), door.Enabled);

                switch (door.Status)
                {
                    case DoorStatus.Open:
                    case DoorStatus.Opening:
                        block.SetStyle("Open", true);
                        break;
                    default:
                        block.SetStyle("Open", false);
                        break;
                }
            }

            action(door);
        }

        private void SaveAndApply(IBlockConfiguration block, IMySoundBlock soundBlock, Action<IMySoundBlock> action)
        {
            if (!block.HasStyle)
            {
                block.SetStyle(nameof(IMySoundBlock.Enabled), soundBlock.Enabled);
                block.SetStyle(nameof(IMySoundBlock.SelectedSound), soundBlock.SelectedSound);
                block.SetStyle(nameof(IMySoundBlock.Volume), soundBlock.Volume);
                block.SetStyle(nameof(IMySoundBlock.Range), soundBlock.Range);
                block.SetStyle(nameof(IMySoundBlock.LoopPeriod), soundBlock.LoopPeriod);
            }

            action(soundBlock);
        }

        private void SaveAndApply(IBlockConfiguration block, IMyAssembler assembler, Action<IMyAssembler> action)
        {
            if (!block.HasStyle)
            {
                block.SetStyle(nameof(IMyAssembler.Enabled), assembler.Enabled);
            }

            action(assembler);
        }

        private void SaveAndApply(IBlockConfiguration block, IMyRefinery refinery, Action<IMyRefinery> action)
        {
            if (!block.HasStyle)
            {
                block.SetStyle(nameof(IMyRefinery.Enabled), refinery.Enabled);
            }

            action(refinery);
        }

        #endregion

        #region Config Restorers

        private void Restore(IBlockConfiguration block, IMyWarhead warhead)
        {
            warhead.IsArmed = block.GetBooleanStyle(nameof(IMyWarhead.IsArmed));
            warhead.DetonationTime = block.GetSingleStyle(nameof(IMyWarhead.DetonationTime), Countdown);
        }

        private void Restore(IBlockConfiguration block, IMyTextPanel textPanel)
        {
            Restore(block, textPanel, 0);
            textPanel.Enabled = block.GetBooleanStyle(nameof(IMyTextPanel.Enabled), true);
        }

        private void Restore(IBlockConfiguration provider, IMyTextSurface surface, Int32 index)
        {
            surface.WriteText(provider.GetStringStyle($"Surface {index} Text"));
            surface.FontSize = provider.GetSingleStyle($"Surface {index} {nameof(IMyTextSurface.FontSize)}", 1);
            surface.Font = provider.GetStringStyle($"Surface {index} {nameof(IMyTextSurface.Font)}", "Debug");
            surface.Alignment = provider.GetEnumStyle<TextAlignment>($"Surface {index} {nameof(IMyTextSurface.Alignment)}", TextAlignment.LEFT);
            surface.FontColor = provider.GetColorStyle($"Surface {index} {nameof(IMyTextSurface.FontColor)}", Colors.White);
            surface.BackgroundColor = provider.GetColorStyle($"Surface {index} {nameof(IMyTextSurface.BackgroundColor)}", Colors.Black);
            surface.ContentType = provider.GetEnumStyle<ContentType>($"Surface {index} {nameof(IMyTextSurface.ContentType)}", ContentType.NONE);
        }

        private void Restore(IBlockConfiguration block, IMyLightingBlock lightingBlock)
        {
            lightingBlock.Enabled = block.GetBooleanStyle(nameof(IMyLightingBlock.Enabled), true);
            lightingBlock.BlinkIntervalSeconds = block.GetSingleStyle(nameof(IMyLightingBlock.BlinkIntervalSeconds));
            lightingBlock.BlinkLength = block.GetSingleStyle(nameof(IMyLightingBlock.BlinkLength));
            lightingBlock.BlinkOffset = block.GetSingleStyle(nameof(IMyLightingBlock.BlinkOffset));
            lightingBlock.Color = block.GetColorStyle(nameof(IMyLightingBlock.Color), Colors.White);
            lightingBlock.Falloff = block.GetSingleStyle(nameof(IMyLightingBlock.Falloff));
            lightingBlock.Intensity = block.GetSingleStyle(nameof(IMyLightingBlock.Intensity));
            lightingBlock.Radius = block.GetSingleStyle(nameof(IMyLightingBlock.Radius));
        }

        private void Restore(IBlockConfiguration block, IMyDoor door) 
            => door.Enabled = block.GetBooleanStyle(nameof(IMyDoor.Enabled), true);

        private void Restore(IBlockConfiguration block, IMySoundBlock soundBlock)
        {
            soundBlock.Enabled = block.GetBooleanStyle(nameof(IMySoundBlock.Enabled), true);
            // TODO: Currently assuming that sound blocks are off by default.
            // https://support.keenswh.com/spaceengineers/general/topic/improvements-to-imysoundblock-interface
            var selectedSound = block.GetStringStyle(nameof(IMySoundBlock.SelectedSound));
            if (selectedSound != soundBlock.SelectedSound)
            {
                soundBlock.Stop();
                soundBlock.SelectedSound = selectedSound;
            }
            soundBlock.Volume = block.GetSingleStyle(nameof(IMySoundBlock.Volume), 1);
            soundBlock.Range = block.GetSingleStyle(nameof(IMySoundBlock.Range), 1);
            soundBlock.LoopPeriod = block.GetSingleStyle(nameof(IMySoundBlock.LoopPeriod));
        }

        private void Restore(IBlockConfiguration block, IMyAssembler assembler)
            => assembler.Enabled = block.GetBooleanStyle(nameof(IMyAssembler.Enabled), true);

        private void Restore(IBlockConfiguration block, IMyRefinery refinery)
            => refinery.Enabled = block.GetBooleanStyle(nameof(IMyRefinery.Enabled), true);

        #endregion
    }
}
