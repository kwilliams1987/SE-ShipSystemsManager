// <mdk sortorder="1" />
using System;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        private readonly IOrderedEnumerable<BaseStyler> StatePriority;

        #region mdk macros
        private const String VERSION = "$MDK_DATE$, $MDK_TIME$";
        #endregion

        public static class BlockType
        {
            public const String Airlock = "airlock";
            public const String Security = "security";
            public const String DoorSign = "doorsign";
            public const String BattleSign = "battle";
            public const String Warning = "warnsign";
            public const String Siren = "siren";
            public const String SelfDestruct = "selfdestruct";
            public const String AlwaysOn = "lowpower";
        }

        public static class BlockState
        {
            public const String LowPower = "lowpower";
            public const String BattleStations = "battle";
            public const String Decompression = "decompression";
            public const String Intruder1 = "intruder1"; // Turrets
            public const String Intruder2 = "intruder2"; // Sensors
            public const String Destruct = "selfdestruct";
        }
    }
}