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
        public void Setup()
        {
            Console.WriteLine("Preparing AssemblyStation tests");
        }

        [Test]
        public void AssemblyStationGetStatusTest()
        {
            KeyValuePair<string, string>[] test = assemblyStation.GetState();
            Console.WriteLine(test.ToString());
            Assert.That(test, Is.All.Not.Null);
            Assert.That(test, Contains.Item(new KeyValuePair<string,string>("lastOperation", "12345")));
            Assert.That(test, Contains.Item(new KeyValuePair<string, string>("currentOperation", "12345")));
        }
    }
}
