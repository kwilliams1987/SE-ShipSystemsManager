using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    // This template is intended for extension classes. For most purposes you're going to want a normal
    // utility class.
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
    static class BlockExtensions
    {
        // TODO: Workaround for sounds being distorted when calling IMySoundBlock.Play() on a block which is already playing.
        // https://support.keenswh.com/spaceengineers/general/topic/improvements-to-imysoundblock-interface
        public static void PlaySound(this IMySoundBlock soundBlock, String sound)
        {
            if (soundBlock.SelectedSound != sound)
            {
                soundBlock.SelectedSound = sound;
                soundBlock.Play();
            }
        }

        public static void WriteAndScaleText(this IMyTextSurface textSurface, String text)
        {
            if (textSurface.ContentType != ContentType.TEXT_AND_IMAGE)
                return;

            var textBuilder = new StringBuilder(text);
            var width = textSurface.SurfaceSize.X - textSurface.TextPadding;
            var height = textSurface.SurfaceSize.Y - textSurface.TextPadding;

            var fontSize = 10f;

            var size = textSurface.MeasureStringInPixels(textBuilder, textSurface.Font, fontSize);

            while (width < size.X || height < size.Y)
            {
                fontSize = fontSize * 0.9f;
                size = textSurface.MeasureStringInPixels(textBuilder, textSurface.Font, fontSize);
            }

            textSurface.FontSize = fontSize;
            textSurface.WriteText(text);
        }

        public static Boolean IsWideScreen(this IMyTextSurface textSurface) 
            => textSurface.SurfaceSize.X > textSurface.SurfaceSize.Y * 1.5;

        public static void ToggleOpenAndEnable(this IMyDoor door, Boolean open, Boolean enabled)
        {
            switch (door.Status)
            {
                case DoorStatus.Open:
                    if (open)
                    {
                        door.Enabled = enabled;
                    }
                    else
                    {
                        door.Enabled = true;
                        door.CloseDoor();
                    }
                    break;
                case DoorStatus.Opening:
                    if (!open)
                    {
                        door.Enabled = true;
                        door.CloseDoor();
                    }
                    break;
                case DoorStatus.Closing:
                    if (open)
                    {
                        door.Enabled = true;
                        door.OpenDoor();
                    }
                    break;
                case DoorStatus.Closed:
                    if (open)
                    {
                        door.Enabled = true;
                        door.OpenDoor();
                    }
                    else
                    {
                        door.Enabled = enabled;
                    }
                    break;
            }
        }
    }
}
