// <mdk sortorder="100" />
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public static Boolean DebugMode = true;
        public static Boolean EnableSingleTickCycle = false;

        #region mdk macros
        private const String VERSION = "$MDK_DATE$, $MDK_TIME$";
        #endregion

        static readonly IOrderedEnumerable<StateStyler> StatePriority = new List<StateStyler>
        {
            new StateStyler(1, BlockState.DECOMPRESSION, StyleDecompression),
            new StateStyler(2, BlockState.INTRUDER1, StyleIntruder),
            new StateStyler(3, BlockState.INTRUDER2, StyleIntruder),
            new StateStyler(3, BlockState.BATTLESTATIONS, StyleBattleStations)
        }.OrderBy(s => s.Priority);

        static class BlockFunction
        {
            public static readonly String DOOR_AIRLOCK = "airlock";
            public static readonly String DOOR_SECURITY = "security";
            public static readonly String SIGN_DOOR = "doorsign";
            public static readonly String SIGN_BATTLE = "battle";
            public static readonly String SIGN_WARNING = "warnsign";
            public static readonly String SOUNDBLOCK_SIREN = "siren";
            public static readonly String LIGHT_WARNING = "warnlight";
        }

        static class BlockState
        {
            public static readonly String BATTLESTATIONS = "battle";
            public static readonly String DECOMPRESSION = "decompression";
            public static readonly String INTRUDER1 = "intruder1"; // Turrets
            public static readonly String INTRUDER2 = "intruder1"; // Sensors
        }

        static class Configuration
        {
            public static class Decompression
            {
                public static readonly String ZONE_LABEL = "DECOMPRESSION DANGER";
                public static readonly Color SIGN_FOREGROUND_COLOR = new Color(0, 0, 255);
                public static readonly Color SIGN_BACKGROUND_COLOR = new Color(0, 0, 0);
                public static readonly String SIGN_IMAGE = "Danger";
                public static readonly String ALERT_SOUND = "Alert 2";
                public static readonly Single FONTSIZE = 2.9f / ZONE_LABEL.Split('\n').Length;

                public static readonly Color LIGHT_COLOR = new Color(0, 0, 255);
                public static readonly Single LIGHT_BLINK = 3;
                public static readonly Single LIGHT_DURATION = 66.6f;
                public static readonly Single LIGHT_OFFSET = 0;
            }

            public static class Intruder
            {
                public static readonly String ZONE_LABEL = "INTRUDER ALERT";
                public static readonly Color SIGN_FOREGROUND_COLOR = new Color(255, 0, 0);
                public static readonly Color SIGN_BACKGROUND_COLOR = new Color(0, 0, 0);
                public static readonly String SIGN_IMAGE = "Warning";
                public static readonly String ALERT_SOUND = "Alert 1";
                public static readonly Single FONTSIZE = 2.9f / ZONE_LABEL.Split('\n').Length;

                public static readonly Color LIGHT_COLOR = new Color(255, 0, 0);
                public static readonly Single LIGHT_BLINK = 3;
                public static readonly Single LIGHT_DURATION = 50;
                public static readonly Single LIGHT_OFFSET = 0;
            }

            public static class BattleStations
            {
                public static readonly String ZONE_LABEL = "BATTLE STATIONS";
                public static readonly Color SIGN_FOREGROUND_COLOR = new Color(255, 0, 0);
                public static readonly Color SIGN_BACKGROUND_COLOR = new Color(0, 0, 0);
                public static readonly String ALERT_SOUND = "Alert 1";
                public static readonly Single FONTSIZE = 2.9f / ZONE_LABEL.Split('\n').Length;
                public static readonly String SIGN_IMAGE = "Warning";

                public static readonly Color LIGHT_COLOR = new Color(255, 0, 0);
                public static readonly Single LIGHT_BLINK = 3;
                public static readonly Single LIGHT_DURATION = 33.3f;
                public static readonly Single LIGHT_OFFSET = 0;
            }
        }

        class StateStyler
        {
            public readonly Int32 Priority;
            public readonly String Key;
            public readonly Action<IMyTerminalBlock> Style;

            public StateStyler(Int32 priority, String key, Action<IMyTerminalBlock> style)
            {
                Priority = priority;
                Key = key;
                Style = style;
            }
        }
    }
}