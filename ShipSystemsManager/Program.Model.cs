using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    public partial class Program
    {
        private static class Colors
        {
            public static readonly Color Red = new Color(255, 0, 0);
            public static readonly Color White = new Color(255, 255, 255);
            public static readonly Color Orange = new Color(255, 102, 51);
            public static readonly Color Blue = new Color(0, 0, 255);
            public static readonly Color Black = new Color(0, 0, 0);
        }

        private interface IBlockConfiguration
        {
            Boolean HasConfig { get; }
            Boolean HasStyle { get; }

            void Save();

            void SetStyle(String key, Single value);
            void SetStyle(String key, Boolean value);
            void SetStyle(String key, String value);
            void SetStyle(String key, UInt32 value);
            void SetStyle(String key, Color value);

            Single GetSingleStyle(String key, Single defaultValue = 0);
            Boolean GetBooleanStyle(String key, Boolean defaultValue = false);
            String GetStringStyle(String key, String defaultValue = "");
            UInt32 GetUInt32Style(String key, UInt32 defaultValue = 0);
            Color GetColorStyle(String key, Color? defaultValue = null);
            TEnumType GetEnumStyle<TEnumType>(String key, TEnumType? defaultValue = null)
                where TEnumType : struct;
        }

        private class Block<T> : IBlockConfiguration
            where T : IMyTerminalBlock
        {
            public T Target { get; }
            public IEnumerable<String> Zones { get; }
            public BlockFunction Functions { get; }
            public MyIni Configuration { get; }

            public Block(T block, MyIni config)
            {
                Target = block;
                Configuration = config;

                var zoneConfig = config.Get(IniSection, "zones");
                if (!zoneConfig.IsEmpty)
                {
                    var zones = new List<String>();
                    zoneConfig.GetLines(zones);
                    Zones = zones;
                }

                var functionConfig = config.Get(IniSection, "functions");
                if (!functionConfig.IsEmpty)
                {
                    var functions = new List<String>();
                    var func = default(BlockFunction);
                    functionConfig.GetLines(functions);
                    foreach (var function in functions)
                    {
                        if (Enum.TryParse(function, out func))
                        {
                            Functions &= func;
                        }
                    }
                }
            }

            public Boolean HasConfig => Configuration.ContainsSection(IniSection);
            public Boolean HasStyle => Configuration.ContainsSection(IniStyleSection);

            public void Save() => Target.CustomData = Configuration.ToString();

            public void SetStyle(String key, Single value) => Configuration.Set(IniStyleSection, key, value);
            public void SetStyle(String key, Boolean value) => Configuration.Set(IniStyleSection, key, value);
            public void SetStyle(String key, String value) => Configuration.Set(IniStyleSection, key, value);
            public void SetStyle(String key, UInt32 value) => Configuration.Set(IniStyleSection, key, value);
            public void SetStyle(String key, Color value) => Configuration.Set(IniStyleSection, key, value.PackedValue);

            public Single GetSingleStyle(String key, Single defaultValue = 0) => Configuration.Get(IniStyleSection, key).ToSingle(defaultValue);
            public Boolean GetBooleanStyle(String key, Boolean defaultValue = false) => Configuration.Get(IniStyleSection, key).ToBoolean(defaultValue);
            public String GetStringStyle(String key, String defaultValue = "") => Configuration.Get(IniStyleSection, key).ToString(defaultValue);
            public UInt32 GetUInt32Style(String key, UInt32 defaultValue = 0) => Configuration.Get(IniStyleSection, key).ToUInt32(defaultValue);
            public Color GetColorStyle(String key, Color? defaultValue = null)
                => new Color(Configuration.Get(IniStyleSection, key).ToUInt32(defaultValue.GetValueOrDefault(Colors.Black).PackedValue));
            public TEnumType GetEnumStyle<TEnumType>(String key, TEnumType? defaultValue = null)
                where TEnumType : struct
            {
                if (!defaultValue.HasValue)
                    defaultValue = default(TEnumType);

                var value = Configuration.Get(IniStyleSection, key).ToString();

                if (String.IsNullOrWhiteSpace(value))
                    return (TEnumType)defaultValue;

                return (TEnumType)Enum.Parse(typeof(TEnumType), value);
            }
        }

        [Flags]
        public enum BlockFunction
        {
            None = 0,
            AirlockDoor = 1 << 0,
            SecurityDoor = 1 << 1,
            Door = AirlockDoor | SecurityDoor,
            Battle = 1 << 2,
            LowPower = 1 << 3,
            SelfDestruct = 1 << 4,
            Alert = 1 << 5
        }

        [Flags]
        public enum EntityState
        {
            Default = 0,
            Decompress = 1 << 0,
            Intruder = 1 << 1,
            Destruct = 1 << 2,
            LowPower = 1 << 3,
            Battle = 1 << 4
        }
    }
}
