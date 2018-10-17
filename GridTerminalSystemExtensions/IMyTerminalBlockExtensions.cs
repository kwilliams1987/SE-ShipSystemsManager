using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    static class IMyTerminalBlockExtensions
    {
        const String Functions = "functions";
        const String Zones = "zones";
        static readonly Dictionary<String, Single> CharWidth = new Dictionary<String, Single>
        {
            { "DEBUG", 1 },
            { "MONOSPACE", 1 }
        };

        public static Boolean IsSmall(this IMyTextPanel block) => block.BlockDefinition.SubtypeId.Contains("BlockCorner_LCD");

        public static void AutoFitText(this IMyTextPanel block)
        {
            var lines = block.GetPublicText().Split('\n');
            var textWidth = lines.Select(r => r.Length).Max();
            var font = block.Font;
            var maxWidth = 0.0;
            var maxLines = 0.0;

            switch (block.BlockDefinition.SubtypeId)
            {
                case "LargeLCDPanelWide":
                case "SmallLCDPanelWide":
                    if (font == "Debug")
                        maxWidth = 59.5;
                    break;
                default:
                    if (font == "Debug")
                        maxWidth = 29.5;
                    break;
            }

            switch (block.BlockDefinition.SubtypeId)
            {
                case "LargeBlockCorner_LCD_1":
                case "LargeBlockCorner_LCD_2":
                    if (font == "Debug")
                        maxLines = 2.5;
                    break;
                case "LargeBlockCorner_LCD_Flat_1":
                case "LargeBlockCorner_LCD_Flat_2":
                    if (font == "Debug")
                        maxLines = 3.0;
                    break;
                case "SmallBlockCorner_LCD_1":
                case "SmallBlockCorner_LCD_2":
                    if (font == "Debug")
                        maxLines = 4.5;
                    break;
                case "SmallBlockCorner_LCD_Flat_1":
                case "SmallBlockCorner_LCD_Flat_2":
                    if (font == "Debug")
                        maxLines = 5.5;
                    break;
                default:
                    if (font == "Debug")
                        maxLines = 17.5;
                    break;
            }

            if (maxWidth > 0 && maxLines > 0 && lines.Any() && textWidth > 0)
            {
                var fontSize = Math.Round(Math.Min(maxWidth / textWidth, maxLines / lines.Count()) * 0.9, 4);
                block.FontSize = Convert.ToSingle(fontSize);
            }
        }
    }
}