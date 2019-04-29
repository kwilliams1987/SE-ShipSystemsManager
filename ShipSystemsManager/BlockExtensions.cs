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

        public static void DrawScaledSpriteText(this IMyTextSurface textSurface, StringBuilder text, String fontId, Color color, Single? scale = null, TextAlignment textAlignment = TextAlignment.CENTER)
        {
            textSurface.Script = "";
            textSurface.ContentType = ContentType.SCRIPT;

            using (var frame = textSurface.DrawFrame())
            {
                var fillArea = textSurface.TextureSize - new Vector2(textSurface.TextPadding * 2);
                
                if (scale == null)
                {
                    var tryScale = 10f;
                    var currentSize = textSurface.MeasureStringInPixels(text, fontId, tryScale);

                    while (currentSize.X > fillArea.X || currentSize.Y > fillArea.Y)
                    {
                        tryScale *= 0.9f;
                        currentSize = textSurface.MeasureStringInPixels(text, fontId, tryScale);
                    }

                    scale = tryScale;
                }

                var sprite = MySprite.CreateText(text.ToString(), fontId, color, scale.Value, textAlignment);
                sprite.Position = new Vector2(textSurface.TextPadding, textSurface.TextPadding);
                frame.Add(sprite);
            }
        }

        public static void DrawScaledSpriteText(this IMyTextSurface textSurface, String text, String fontId, Color color, Single? scale = null, TextAlignment textAlignment = TextAlignment.CENTER)
            => DrawScaledSpriteText(textSurface, new StringBuilder(text), fontId, color, scale, textAlignment);

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

        public static void ForEachSurface(this IMyTextSurfaceProvider surfaceProvider, Action<IMyTextSurface, Int32> action)
        {
            var surfaces = surfaceProvider.SurfaceCount;
            var i = 0;

            while (i < surfaces)
            {
                action(surfaceProvider.GetSurface(i), i);
                i++;
            }
        }
    }
}
