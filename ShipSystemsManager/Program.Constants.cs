// <mdk sortorder="1" />
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

        static class BlockFunction
        {
            public static readonly String DOOR_AIRLOCK = "airlock";
            public static readonly String DOOR_SECURITY = "security";
            public static readonly String SIGN_DOOR = "doorsign";
            public static readonly String SIGN_BATTLE = "battle";
            public static readonly String SIGN_WARNING = "warnsign";
            public static readonly String SOUNDBLOCK_SIREN = "siren";
            public static readonly String LIGHT_WARNING = "warnlight";
            public static readonly String WARHEAD_DESTRUCT = "selfdestruct";
        }

        static class BlockState
        {
            public static readonly String BATTLESTATIONS = "battle";
            public static readonly String DECOMPRESSION = "decompression";
            public static readonly String INTRUDER1 = "intruder1"; // Turrets
            public static readonly String INTRUDER2 = "intruder1"; // Sensors
            public static readonly String SELFDESTRUCT = "selfdestruct";
        }
    }
}