using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using IngameScript.Mockups;
using IngameScript.Mockups.Asserts;
using IngameScript.Mockups.Blocks;
using Malware.MDKUtilities;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;
using static IngameScript.Program;

namespace IngameScript.MDK
{
    public class Bootstrapper
    {
        // All the files in this folder, as well as all files containing the file ".debug.", will be excluded
        // from the build process. You can use this to create utilites for testing your scripts directly in 
        // Visual Studio.

        static Bootstrapper()
        {
            // Initialize the MDK utility framework
            MDKUtilityFramework.Load();
        }

        public static void RunCycle(Program program)
        {
            MDKFactory.Run(program, updateType: UpdateType.Once);

            while (program.CurrentTick > 0)
                MDKFactory.Run(program, updateType: UpdateType.Once);

            Assert.Equals(program.CurrentTick, 0, "State machine was unexpected terminated.");
        }

        public static void Main()
        {

        }
    }
}