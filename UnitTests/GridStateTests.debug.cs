using System;
using IngameScript;
using Malware.MDKUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sandbox.ModAPI.Ingame;

namespace UnitTests
{
    [TestClass]
    public class GridStateTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            MDKUtilityFramework.Load();
        }

        [TestMethod]
        public void TestDefault()
        {
            var program = MDKFactory.CreateProgram<Program>();
            MDKFactory.Run(program, updateType: UpdateType.Once);
        }
    }
}
