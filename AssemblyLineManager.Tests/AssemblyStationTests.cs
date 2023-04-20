using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssemblyLineManager.AssemblyStation;
using AssemblyLineManager.CommonLib;

namespace AssemblyLineManager.Tests
{
    internal class AssemblyStationTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        AssemblyStation.AssemblyStation assemblyStation;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Preparing AssemblyStation tests");
            assemblyStation = new AssemblyStation.AssemblyStation();
        }

        [Test]
        public void AssemblyStationGetStatusTest()
        {
            {
                assemblyStation.SendCommand("name", "emulator/operation");
                KeyValuePair<string, string>[] test = assemblyStation.GetState();
                Console.WriteLine(test.ToString());
                Assert.That(test, Is.All.Not.Null);
                Assert.That(test, Contains.Item(new KeyValuePair<string, string>("currentOperation", "12345")));
            }

            {
                assemblyStation.SendCommand("name", "emulator/operation");
                KeyValuePair<string, string>[] test = assemblyStation.GetState();
                Console.WriteLine(test.ToString());
                Assert.That(test, Is.All.Not.Null);
                Assert.That(test, Contains.Item(new KeyValuePair<string, string>("lastOperation", "12345")));
                Assert.That(test, Contains.Item(new KeyValuePair<string, string>("currentOperation", "12345")));
            }
        }

        [Test]
        public void AssemblyStationSendCommandTest()
        {

        }
    }
}
