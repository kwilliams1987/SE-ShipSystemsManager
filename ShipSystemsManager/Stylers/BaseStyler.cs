using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    public partial class Program
    {
        public abstract class BaseStyler
        {
            public Int32 Priority { get; protected set; }
            public String State { get; protected set; }

            protected abstract String Prefix { get; }
            public static IReadOnlyDictionary<String, Object> DefaultStyles { get; } = new Dictionary<String, Object>()
            {
                { "battle.text", "BATTLE\nSTATIONS" },
                { "battle.text.font", "Debug" },
                { "battle.text.color", new Color(255, 0, 0) },
                { "battle.light.color", new Color(255, 0, 0) },
                { "battle.light.interval", 3f },
                { "battle.light.duration", 33.3f },
                { "battle.light.offset", 0f },
                { "battle.sign.color", new Color(0, 0, 0) },
                { "battle.sign.images", "Cross" },
                { "battle.sound", "Alert 1" },
                { "battle.rotor.rpm", 20 },

                { "decompression.text", "DECOMPRESSION\nDANGER" },
                { "decompression.text.font", "Debug" },
                { "decompression.text.color", new Color(0, 0, 255) },
                { "decompression.light.color", new Color(0, 0, 255) },
                { "decompression.light.interval", 3f },
                { "decompression.light.duration", 66.6f },
                { "decompression.light.offset", 0f },
                { "decompression.sign.color", new Color(0, 0, 0) },
                { "decompression.sign.images", "Danger" },
                { "decompression.sound", "Alert 2" },
                { "decompression.rotor.rpm", 20 },

                { "destruct.text", "SELF DESTRUCT\n{0}" },
                { "destruct.text.color", new Color(255, 0, 0) },
                { "destruct.text.fail", "SELF DESTRUCT\nUNAVAILABLE" },
                { "destruct.text.fail.color", new Color(255, 255, 0) },
                { "destruct.sign.images", "Danger" },
                { "destruct.timer", 180f },

                { "intruder.text", "INTRUDER ALERT" },
                { "intruder.text.color", new Color(255, 0, 0) },
                { "intruder.sign.color", new Color(0, 0, 0) },
                { "intruder.sign.images", "Danger" },
                { "intruder.light.color", new Color(255, 0, 0) },
                { "intruder.sound", "Alert 1" },
                { "intruder.rotor.rpm", 20 },

                { "lowpower.light.intensity", 0.3f },
                { "lowpower.light.radius", 0.3f }
            };

            protected IMyProgrammableBlock ProgrammableBlock { get; }
            protected MyConfig ProgrammableBlockConfig { get; }

            protected BaseStyler(Int32 priority, String key, IMyProgrammableBlock block)
            {
                Priority = priority;
                State = key;
                ProgrammableBlock = block;
                ProgrammableBlockConfig = new MyConfig(block);
            }

            protected T Get<T>(String key)
            {
                key = Prefix + "." + key;

                if (ProgrammableBlockConfig.ContainsKey(key))
                {
                    return ProgrammableBlockConfig.GetValue(key).ToType<T>();
                }
                else if (DefaultStyles.ContainsKey(key))
                {
                    return (T)DefaultStyles[key];
                }
                else
                {
                    throw new Exception($"The requested style {key} is not present.");
                }
            }

            public abstract void Style(IMyTerminalBlock block, MyIni storage);
        }
    }
}
