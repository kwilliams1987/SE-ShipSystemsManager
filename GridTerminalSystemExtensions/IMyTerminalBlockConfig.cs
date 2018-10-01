using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    static class IMyTerminalBlockCconfigExtensions
    {
        public static Boolean HasConfigFlag(this IMyTerminalBlock block, String key, params String[] values)
        {
            return values.Any(value => block.GetConfigs(key).Contains(value));
        }

        public static void SetConfigFlag(this IMyTerminalBlock block, String key, String value)
        {
            var values = block.GetConfigs(key).ToList();

            if (!values.Contains(value))
            {
                values.Add(value);
            }

            block.SetConfigs(key, values);
        }

        public static void ClearConfigFlag(this IMyTerminalBlock block, String key, String value)
        {
            var values = block.GetConfigs(key).ToList();

            if (values.Contains(value))
            {
                values.Remove(value);
            }

            block.SetConfigs(key, values);
        }

        public static IEnumerable<IMyTerminalBlock> SetStates(this IEnumerable<IMyTerminalBlock> blocks, params String[] states)
        {
            foreach (var block in blocks)
            {
                var blockStates = block.GetConfigs("state").ToList();
                blockStates.AddRange(states);

                block.SetConfigs("state", blockStates.Distinct());
            }

            return blocks;
        }

        public static IEnumerable<IMyTerminalBlock> ClearStates(this IEnumerable<IMyTerminalBlock> blocks, params String[] states)
        {
            foreach (var block in blocks)
            {
                var blockStates = block.GetConfigs("state").ToList();
                blockStates.RemoveAll(s => states.Contains(s));

                block.SetConfigs("state", blockStates.Distinct());
            }

            return blocks;
        }

        public static IEnumerable<String> GetConfigs(this IMyTerminalBlock block, String key, Char denominator = ';')
        {
            return block.GetConfigs<String>(key, denominator).Where(z => !String.IsNullOrWhiteSpace(z));
        }

        public static IEnumerable<T> GetConfigs<T>(this IMyTerminalBlock block, String key, Char denominator = ';')
        {
            return block.GetConfig(key).Split(denominator).Where(c => !String.IsNullOrWhiteSpace(c)).Select(c => (T)Convert.ChangeType(c, typeof(T)));
        }

        public static T GetConfig<T>(this IMyTerminalBlock block, String key)
        {
            var config = block.GetConfig();
            var value = "";

            if (config.ContainsKey(key))
            {
                value = block.GetConfig()[key];
            }

            if (String.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        public static String GetConfig(this IMyTerminalBlock block, String key, Boolean multiline = false)
        {
            var config = block.GetConfig();
            var value = "";

            if (config.ContainsKey(key))
            {
                value = block.GetConfig()[key];
            }

            if (String.IsNullOrWhiteSpace(value))
            {
                value = "";
            }

            if (multiline)
            {
                value = value.Replace("#N#", "\n");
            }

            return value;
        }

        public static void SetConfig(this IMyTerminalBlock block, String key, Object value, Boolean multiline = false)
        {
            var textvalue = value.ToString();

            if (multiline)
            {
                textvalue = textvalue.Replace("\n", "#N#");
            }

            var customData = block.CustomData;
            if (String.IsNullOrWhiteSpace(customData))
            {
                customData = "";
            }

            var lines = customData.Split('\n').ToList();
            var found = false;

            for (var l = 0; l < lines.Count; l++)
            {
                var parts = lines[l].Split(':');
                if (parts.Length > 1 && parts[0] == key)
                {
                    lines[l] = key + ":" + textvalue;
                    found = true;
                    break;
                }
            }

            if (found == false)
            {
                lines.Add(key + ":" + textvalue);
            }

            block.CustomData = String.Join("\n", lines).Trim();
        }

        public static void SetConfigs(this IMyTerminalBlock block, String key, IEnumerable<Object> values, Char denominator = ';')
        {
            var value = String.Join(denominator.ToString(), values.Select(v => v.ToString()));

            block.SetConfig(key, value);
        }

        public static Dictionary<String, String> GetConfig(this IMyTerminalBlock block)
        {
            var config = new Dictionary<String, String>();

            var customData = block.CustomData;
            if (String.IsNullOrWhiteSpace(customData))
                customData = "";

            var lines = customData.Split('\n').Where(l => l.Contains(':'));
            foreach (var line in lines)
            {
                var split = line.Split(':');
                config.Add(split.First(), String.Join(":", split.Skip(1)));
            }

            return config;
        }
    }
}
