using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using System;
using VRageMath;

namespace IngameScript
{
    static class Extensions
    {
        public static void ApplyConfig<T>(this T block, Dictionary<String, Object> configValues)
            where T : IMyTerminalBlock
        {
            block.SaveState();

            var door = block as IMyDoor;
            var textPanel = block as IMyTextPanel;
            var soundBlock = block as IMySoundBlock;
            var warhead = block as IMyWarhead;

            foreach (var config in configValues)
            {
                switch (config.Key)
                {
                    case "Closed":
                        if (door != default(IMyDoor))
                        {
                            if (Object.Equals(config.Value, true))
                            {
                                if (door.OpenRatio > 0)
                                {
                                    door.Enabled = true;
                                    door.CloseDoor();
                                }
                            }
                        }
                        break;
                    case "Countdown":
                        if (warhead != default(IMyWarhead))
                        {
                            if (Object.Equals(config.Value, true))
                            {
                                warhead.StartCountdown();
                            }
                            else
                            {
                                warhead.StopCountdown();
                            }
                        }
                        break;
                    case "Locked":
                        if (door != default(IMyDoor))
                        {
                            if (Object.Equals(config.Value, true))
                            {
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
                            else
                            {
                                door.Enabled = true;
                            }
                        }
                        break;
                    case "PublicText":
                        if (textPanel != default(IMyTextPanel))
                        {
                            textPanel.WritePublicText(config.Value.ToString());
                        }
                        break;
                    case "PublicTitle":
                        if (textPanel != default(IMyTextPanel))
                        {
                            textPanel.WritePublicTitle(config.Value.ToString());
                        }
                        break;
                    case "Images":
                        if (textPanel != default(IMyTextPanel))
                        {
                            textPanel.ClearImagesFromSelection();
                            textPanel.AddImagesToSelection(config.Value.ToString().Split(';').ToList());
                            textPanel.ShowTextureOnScreen();
                        }
                        break;
                    case "Play":
                        if (soundBlock != default(IMySoundBlock))
                        {
                            if (Object.Equals(config.Value, true))
                            {
                                soundBlock.Play();
                            }
                            else
                            {
                                soundBlock.Stop();
                            }
                        }
                        break;
                    default:
                        if (config.Value is Color)
                        {
                            block.SetValueColor(config.Key, (Color)config.Value);
                        }

                        else if (config.Value is Single)
                        {
                            block.SetValueFloat(config.Key, (Single)config.Value);

                        }

                        else if (config.Value is Boolean)
                        {
                            block.SetValueBool(config.Key, (Boolean)config.Value);
                        }

                        else
                        {
                            block.SetValue(config.Key, config.Value.ToString());
                        }

                        break;
                }
            }
        }

        public static void ApplyConfig<T>(this IEnumerable<T> blocks, Dictionary<String, Object> configValues)
            where T: IMyTerminalBlock
        {
            foreach (var block in blocks)
            {
                block.ApplyConfig(configValues);
            }
        }
    }
}
