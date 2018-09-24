using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRageMath;

namespace IngameScript
{
    static class IMyTerminalBlockState
    {
        /// <summary>
        /// Save the current configuration of the block into <see cref="IMyTerminalBlock.CustomData"/>, if no previous save was done.
        /// </summary>
        public static void SaveState(this IMyTerminalBlock block)
        {
            if (block.GetConfig<Boolean>("saved"))
                return;

            var properties = new List<ITerminalProperty>();
            block.GetProperties(properties, p => p.Id != "CustomData");

            foreach (var property in properties)
            {
                switch (property.TypeName)
                {
                    case "bool":
                        block.SetConfig(property.Id, property.AsBool().GetValue(block) ? "true" : "false");
                        break;
                    case "float":
                        block.SetConfig(property.Id, property.AsFloat().GetValue(block));
                        break;
                    case "color":
                        block.SetConfig(property.Id, property.AsColor().GetValue(block).PackedValue);
                        break;
                    default:
                        var value = property.As<String>().GetValue(block);
                        block.SetConfig(property.Id, value);
                        break;
                }
            }

            var lcd = block as IMyTextPanel;
            if (lcd != default(IMyTextPanel))
            {
                lcd.SetConfig("PublicText", lcd.GetPublicText(), true);
                lcd.SetConfig("PublicTitle", lcd.GetPublicTitle());

                var images = new List<String>();
                lcd.GetSelectedImages(images);
                lcd.SetConfigs("SelectedImages", images);
                lcd.SetConfig("ShowText", lcd.ShowText);
            }

            //var soundBlock = block as IMySoundBlock;
            //if (soundBlock != default(IMySoundBlock))
            //{
            //    soundBlock.SetConfig("IsPlaying", soundBlock.IsPlaying);
            //}

            block.SetConfig("saved", true);
        }

        /// <summary>
        /// Restore the configuration of the block from <see cref="IMyTerminalBlock.CustomData"/>, if previous 
        /// <see cref="SaveState(IMyTerminalBlock)"/> was done.
        /// </summary>
        public static void RestoreState(this IMyTerminalBlock block)
        {
            if (block.GetConfig<Boolean>("saved"))
                return;

            var properties = new List<ITerminalProperty>();
            block.GetProperties(properties, p => p.Id != "CustomData");

            foreach (var property in properties)
            {
                switch (property.TypeName)
                {
                    case "bool":
                        block.SetValueBool(property.Id, block.GetConfig<Boolean>(property.Id));
                        break;
                    case "float":
                        block.SetValueFloat(property.Id, block.GetConfig<Single>(property.Id));
                        break;
                    case "color":
                        block.SetValueColor(property.Id, new Color(block.GetConfig<UInt32>(property.Id)));
                        break;
                    default:
                        block.SetValue(property.Id, block.GetConfig(property.Id));
                        break;
                }
            }

            var lcd = block as IMyTextPanel;
            if (lcd != default(IMyTextPanel))
            {
                lcd.WritePublicText(lcd.GetConfig("PublicText"), true);
                lcd.WritePublicTitle(lcd.GetConfig("PublicTitle"));

                var images = lcd.GetConfigs("SelectedImages");

                lcd.ClearImagesFromSelection();
                lcd.AddImagesToSelection(images);

                if (lcd.GetConfig<Boolean>("ShowText"))
                {
                    lcd.ShowPublicTextOnScreen();
                }
                else
                {
                    lcd.ShowTextureOnScreen();
                }
            }
        }

        /// <summary>
        /// Saves the current configuration of all blocks in the collection to their respective <see cref="IMyTerminalBlock.CustomData"/>, 
        /// if no previous save was done on the block.
        /// </summary>
        public static IEnumerable<IMyTerminalBlock> SaveStates(this IEnumerable<IMyTerminalBlock> blocks)
        {
            foreach (var block in blocks)
            {
                block.SaveState();
            }

            return blocks;
        }

        /// <summary>
        /// Restores the configuration of all blocks in the collection from their respective <see cref="IMyTerminalBlock.CustomData"/>, 
        /// if previous <see cref="SaveState(IMyTerminalBlock)"/> was done on the block.
        /// </summary>
        public static IEnumerable<IMyTerminalBlock> RestoreStates(this IEnumerable<IMyTerminalBlock> blocks)
        {
            foreach (var block in blocks)
            {
                block.RestoreState();
            }

            return blocks;
        }
    }
}
