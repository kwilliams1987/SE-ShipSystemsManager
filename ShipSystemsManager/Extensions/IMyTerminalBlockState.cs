using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRageMath;

namespace IngameScript
{
    static class IMyTerminalBlockState
    {
        public static void SaveState(this IMyTextPanel panel)
        {
            if (!String.IsNullOrWhiteSpace(panel.GetConfig("lcdtext")))
                return;

            var images = new List<String>();
            panel.GetSelectedImages(images);

            panel.SaveProp("lcdtext", p => p.GetPublicText(), true);
            panel.SaveProp("lcdcolor", p => p.FontColor.PackedValue);
            panel.SaveProp("bgcolor", p => p.BackgroundColor.PackedValue);
            panel.SaveProp("lcdsize", p => p.FontSize);
            panel.SaveProp("lcdfont", p => p.Font);
            panel.SaveProp("enabled", p => p.Enabled);
            panel.SaveProp("interval", p => p.ChangeInterval);
            panel.SaveProp("showtext", p => p.ShowText);
            panel.SetConfig("images", String.Join(";", images));
        }

        public static Boolean RestoreState(this IMyTextPanel panel)
        {
            if (String.IsNullOrWhiteSpace(panel.GetConfig("lcdtext")))
                return false;

            try
            {
                panel.WritePublicText(panel.GetConfig("lcdtext", true));
                panel.FontColor = new Color(panel.GetConfig<UInt32>("lcdcolor"));
                panel.FontSize = panel.GetConfig<Single>("lcdsize");
                panel.Font = panel.GetConfig("lcdfont");
                panel.Enabled = panel.GetConfig<Boolean>("enabled");
                panel.ChangeInterval = panel.GetConfig<Single>("interval");
                panel.BackgroundColor = new Color(panel.GetConfig<UInt32>("bgcolor"));

                var images = panel.GetConfig("images").Split(';').ToList();
                panel.ClearImagesFromSelection();
                panel.AddImagesToSelection(images);

                if (panel.GetConfig<Boolean>("showtext"))
                {
                    panel.ShowPublicTextOnScreen();
                }
                else
                {
                    panel.ShowTextureOnScreen();
                }

                return true;
            }
            catch
            {
                if (Program.DebugMode)
                {
                    throw;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void SaveState(this IMyDoor door)
        {
            if (!String.IsNullOrEmpty(door.GetConfig("enabled")))
                return;

            door.SaveProp("enabled", d => d.Enabled);
        }

        public static Boolean RestoreState(this IMyDoor door)
        {
            if (String.IsNullOrWhiteSpace(door.GetConfig("enabled")))
                return false;

            door.Enabled = door.GetConfig<Boolean>("enabled");
            return true;
        }

        public static void SaveState(this IMySoundBlock soundBlock)
        {
            if (!String.IsNullOrEmpty(soundBlock.GetConfig("enabled")))
                return;

            soundBlock.SaveProp("sound", s => s.SelectedSound);
            soundBlock.SaveProp("duration", s => s.LoopPeriod);
            soundBlock.SaveProp("enabled", s => s.Enabled);
        }

        public static Boolean RestoreState(this IMySoundBlock soundBlock)
        {
            if (String.IsNullOrEmpty(soundBlock.GetConfig("enabled")))
                return false;

            soundBlock.SelectedSound = soundBlock.GetConfig("sound");
            soundBlock.LoopPeriod = soundBlock.GetConfig<Single>("duration");
            soundBlock.Enabled = soundBlock.GetConfig<Boolean>("enabled");

            return true;
        }

        public static void SaveState(this IMyLightingBlock lightingBlock)
        {
            if (!String.IsNullOrWhiteSpace(lightingBlock.GetConfig("enabled")))
                return;

            lightingBlock.SaveProp("color", l => l.Color.PackedValue);
            lightingBlock.SaveProp("interval", l => l.BlinkIntervalSeconds);
            lightingBlock.SaveProp("length", l => l.BlinkLength);
            lightingBlock.SaveProp("offset", l => l.BlinkOffset);
            lightingBlock.SaveProp("enabled", l => l.Enabled);
        }

        public static Boolean RestoreState(this IMyLightingBlock lightingBlock)
        {
            if (String.IsNullOrEmpty(lightingBlock.GetConfig("enabled")))
                return false;

            lightingBlock.Color = new Color(lightingBlock.GetConfig<UInt32>("color"));
            lightingBlock.BlinkIntervalSeconds = lightingBlock.GetConfig<Single>("interval");
            lightingBlock.BlinkLength = lightingBlock.GetConfig<Single>("length");
            lightingBlock.BlinkOffset = lightingBlock.GetConfig<Single>("offset");
            lightingBlock.Enabled = lightingBlock.GetConfig<Boolean>("enabled");

            return true;
        }

        private static void SaveProp<TBlockType, TPropType>(this TBlockType block, String name, Func<TBlockType, TPropType> selector, Boolean multiline = false)
            where TBlockType : IMyTerminalBlock
        {
            block.SetConfig(name, selector(block), multiline);
        }

    }
}
