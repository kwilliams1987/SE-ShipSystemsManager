using Sandbox.ModAPI.Ingame;
using System;

namespace IngameScript
{
    public partial class Program
    {
        abstract class BaseStyler
        {
            public Int32 Priority { get; protected set; }
            public String State { get; protected set; }

            protected BaseStyler(Int32 priority, String key)
            {
                Priority = priority;
                State = key;
            }

            public abstract void Style(IMyTerminalBlock block);
        }
    }
}
