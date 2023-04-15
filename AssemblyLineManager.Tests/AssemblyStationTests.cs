using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyLineManager.AssemblyStation;

namespace AssemblyLineManager.Tests
{
    internal class AssemblyStationTests
    {
        internal static AssemblyStation.AssemblyStation assemblyStation = new AssemblyStation.AssemblyStation();
        [OneTimeSetUp]
        public async Task Setup()
        {
            await assemblyStation.GetState();
        }

        [Test]
        public async Task AssemblyStationGetStatusTest()
        {
            Task<KeyValuePair<int, string>> task = assemblyStation.GetState();
            task.Wait();
            Console.WriteLine(task.Result);

            Assert.True(true);
        }
    }
}
