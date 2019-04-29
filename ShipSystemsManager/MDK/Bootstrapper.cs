using System;
using System.Linq;
using IngameScript.Mockups;
using IngameScript.Mockups.Asserts;
using IngameScript.Mockups.Base;
using IngameScript.Mockups.Blocks;
using Malware.MDKUtilities;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;
using static IngameScript.Program;

namespace IngameScript.MDK
{
    public class TestBootstrapper
    {
        // All the files in this folder, as well as all files containing the file ".debug.", will be excluded
        // from the build process. You can use this to create utilites for testing your scripts directly in 
        // Visual Studio.

        static Int64 NextEntityId = 1L;
        static TestBootstrapper()
        {
            // Initialize the MDK utility framework
            MDKUtilityFramework.Load();
        }

        public static void RunCycle(MockProgrammableBlock programmableBlock)
            => RunCycle(programmableBlock.Program as Program);

        public static void RunUntil(MockProgrammableBlock programmableBlock, Func<Program, Boolean> predicate)
            => RunUntil(programmableBlock.Program as Program, predicate);

        public static void RunCycle(Program program)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            MDKFactory.Run(program, updateType: UpdateType.Once);
            MDKFactory.Run(program, updateType: UpdateType.Once);
            MDKFactory.Run(program, updateType: UpdateType.Once);

            RunUntil(program, p => p.CurrentTick == 2);
        }

        public static void RunUntil(Program program, Func<Program, Boolean> predicate)
        {
            while(!predicate(program))
                MDKFactory.Run(program, updateType: UpdateType.Once);
        }

        public static void Main()
        {
            // In order for your program to actually run, you will need to provide a mockup of all the facilities 
            // your script uses from the game, since they're not available outside of the game.
            var grid = new MockGridTerminalSystem();
            var cubeGrid = new MockCubeGrid()
            {
                CustomName = "Debug Test Station",
                IsStatic = true,
                GridSizeEnum = VRage.Game.MyCubeSize.Large
            };

            var programmableBlock = Mock<MockProgrammableBlock>(options: b => b.CubeGrid = cubeGrid);
            programmableBlock.Program = new Program(new DebugInfo
            {
                ProgrammableBlock = programmableBlock,
                GridTerminalSystem = grid,
                RuntimeInfo = new MockGridProgramRuntimeInfo()
            });

            var echo = programmableBlock.Program.Echo;
            programmableBlock.Program.Echo = message =>
            {
                echo(message);
                Console.WriteLine(message);
            };

            var ventZone1 = Mock<MockAirVent>(options: b => b.CanPressurize = true, zones: "room-1");
            var lightNormalZone1 = Mock<MockInteriorLight>(zones: "room-1");
            var lightAlertZone1 = Mock<MockInteriorLight>(BlockFunction.Alert, zones: "room-1");
            var ventZone2 = Mock<MockAirVent>(options: b => b.CanPressurize = true, zones: "room-2");
            var lightNormalZone2 = Mock<MockInteriorLight>(zones: "room-2");
            var lightAlertZone2 = Mock<MockInteriorLight>(BlockFunction.Alert, zones: "room-2");

            grid.Add(new IMyTerminalBlock[] 
            {
                programmableBlock,
                ventZone1, lightNormalZone1, lightAlertZone1,
                ventZone2, lightNormalZone2, lightAlertZone2
            });
            
            {
                RunUntil(programmableBlock, p => p.CurrentTick > 2);
                Assert.Equals("", programmableBlock.GetSurface(0).GetText(), "Programmable Block Grid Status");
            }

            {
                RunCycle(programmableBlock);

                var pbSurface0 = programmableBlock.GetSurface(0) as MockTextSurface;
                var pbSprite0 = pbSurface0.SpriteBuffer.First();
                
                var pbSurface1 = programmableBlock.GetSurface(1) as MockTextSurface;
                var pbSprite1 = pbSurface1.SpriteBuffer.First();

                Assert.True(SpriteType.TEXT == pbSprite0.Type, "Programmable Block main screen texture type");
                Assert.Equals("Station Normal\r\n room-1 Normal\r\n room-2 Normal\r\n", pbSprite0.Data, "Programmable Block main screen text");

                Assert.True(SpriteType.TEXT == pbSprite1.Type, "Programmable Block keyboard texture type");
                Assert.Equals("Time: 0ms (0ms max)\r\nIOPS: 0/0, (0 max)\r\nTick: 2/7\r\n", pbSprite1.Data, "Programmable Block keyboard text");

                Assert.Equals(lightNormalZone1.Color, new Color(255, 255, 255), "Zone 1 normal light color");
                Assert.True(lightNormalZone1.Enabled, "Zone 2 normal light should be enabled");
                Assert.Equals(lightAlertZone1.Color, new Color(255, 255, 255), "Zone 1 alert light color");
                Assert.True(lightAlertZone1.Enabled, "Zone 1 alert light should be enabled");

                Assert.Equals(lightNormalZone2.Color, new Color(255, 255, 255), "Zone 1 normal light color");
                Assert.True(lightNormalZone2.Enabled, "Zone 2 normal light should be enabled");
                Assert.Equals(lightAlertZone2.Color, new Color(255, 255, 255), "Zone 1 alert light color");
                Assert.True(lightAlertZone2.Enabled, "Zone 1 alert light should be enabled");
            }

            ventZone1.CanPressurize = false;

            {
                RunCycle(programmableBlock);

                Assert.Equals("Station Normal\r\n room-1 Decompressed\r\n room-2 Normal\r\n", GetMockSpriteText(programmableBlock, 0), "Programmable Block main screen text");
                Assert.Equals("Time: 0ms (0ms max)\r\nIOPS: 0/0, (0 max)\r\nTick: 2/7\r\n", GetMockSpriteText(programmableBlock, 1), "Programmable Block keyboard text");

                Assert.Equals(lightNormalZone1.Color, new Color(255, 255, 255), "Zone 1 normal light color");
                Assert.False(lightNormalZone1.Enabled, "Zone 1 normal light should be disabled");
                Assert.Equals(lightAlertZone1.Color, new Color(0, 0, 255, 255), "Zone 1 alert light color");
                Assert.True(lightAlertZone1.Enabled, "Zone 1 alert light should be enabled");

                Assert.Equals(lightNormalZone2.Color, new Color(255, 255, 255), "Zone 1 normal light color");
                Assert.True(lightNormalZone2.Enabled, "Zone 2 normal light should be enabled");
                Assert.Equals(lightAlertZone2.Color, new Color(255, 255, 255), "Zone 1 alert light color");
                Assert.True(lightAlertZone2.Enabled, "Zone 2 alert light should be enabled");
            }

            programmableBlock.Run("toggle battle", UpdateType.Terminal);

            {
                RunCycle(programmableBlock);

                Assert.Equals("Station Battle Stations\r\n room-1 Decompressed\r\n room-2 Normal\r\n", GetMockSpriteText(programmableBlock, 0), "Programmable Block main screen text");
                Assert.Equals("Time: 0ms (0ms max)\r\nIOPS: 0/0, (0 max)\r\nTick: 2/7\r\n", GetMockSpriteText(programmableBlock, 1), "Programmable Block keyboard text");

                Assert.Equals(lightNormalZone1.Color, new Color(255, 255, 255), "Zone 1 normal light color");
                Assert.False(lightNormalZone1.Enabled, "Zone 1 normal light should be disabled");
                Assert.Equals(lightAlertZone1.Color, new Color(0, 0, 255, 255), "Zone 1 alert light color");
                Assert.True(lightAlertZone1.Enabled, "Zone 1 alert light should be enabled");

                Assert.Equals(lightNormalZone2.Color, new Color(255, 255, 255), "Zone 1 normal light color");
                Assert.False(lightNormalZone1.Enabled, "Zone 2 normal light should be disabled");
                Assert.Equals(lightAlertZone2.Color, new Color(255, 0, 0), "Zone 1 alert light color");
                Assert.True(lightAlertZone2.Enabled, "Zone 2 alert light should be enabled");
            }

            programmableBlock.Run("toggle battle", UpdateType.Terminal);
            ventZone1.CanPressurize = true;
            ventZone2.CanPressurize = false;

            {
                RunCycle(programmableBlock);

                Assert.Equals("Station Normal\r\n room-1 Normal\r\n room-2 Decompressed\r\n", GetMockSpriteText(programmableBlock, 0), "Programmable Block main screen text");
                Assert.Equals("Time: 0ms (0ms max)\r\nIOPS: 0/0, (0 max)\r\nTick: 2/7\r\n", GetMockSpriteText(programmableBlock, 1), "Programmable Block keyboard text");

                Assert.Equals(lightNormalZone1.Color, new Color(255, 255, 255), "Zone 1 normal light color");
                Assert.True(lightNormalZone1.Enabled, "Zone 1 normal light should be enabled");
                Assert.Equals(lightAlertZone1.Color, new Color(255, 255, 255, 255), "Zone 1 alert light color");
                Assert.True(lightAlertZone1.Enabled, "Zone 1 alert light should be enabled");

                Assert.Equals(lightNormalZone2.Color, new Color(255, 255, 255), "Zone 1 normal light color");
                Assert.False(lightNormalZone2.Enabled, "Zone 1 normal light should be disabled");
                Assert.Equals(lightAlertZone2.Color, new Color(0, 0, 255), "Zone 1 alert light color");
                Assert.True(lightAlertZone2.Enabled, "Zone 1 alert light should be enabled");
            }

            Console.WriteLine("Mock completed, press any key to exit.");
            Console.ReadKey(true);
        }

        private static T Mock<T>(BlockFunction functions = BlockFunction.None, Action<T> options = null, params String[] zones)
            where T : class, new()
        {
            if (!typeof(MockTerminalBlock).IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException();

            var serializer = new MyIni();
            var block = new T() as MockTerminalBlock;
            block.EntityId = NextEntityId++;

            var name = $"{typeof(T).Name.Substring(4)}";

            if (functions != BlockFunction.None)
            {
                serializer.Set("SSM Configuration", "functions", Convert.ToInt32(functions));
                name += $" ({functions})";
            }

            if (zones.Any())
            {
                serializer.Set("SSM Configuration", "zones", String.Join("\n", zones));
                name += $" [{String.Join(", ", zones)}]";
            }

            block.CustomName = name;
            block.DisplayName = name;
            block.CustomData = serializer.ToString();

            var cast = block as T;
            options?.Invoke(cast);
            return cast;
        }

        private static String GetMockSpriteText(IMyTextSurfaceProvider provider, Int32 surfaceIndex, Int32 spriteIndex = 0)
            => GetMockSpriteText(provider.GetSurface(surfaceIndex), spriteIndex);

        private static String GetMockSpriteText(IMyTextSurface surface, Int32 index = 0)
        {
            var mockSurface = surface as MockTextSurface;
            if (mockSurface == null)
                return "";

            var sprite = mockSurface.SpriteBuffer.ElementAtOrDefault(index);
            if (sprite.Type == SpriteType.TEXT)
                return sprite.Data;

            return "";
        }
    }
}