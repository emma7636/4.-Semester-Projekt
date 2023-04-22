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
                assemblyStation.SendCommand("name", "emulator/operation", new string[] { "123" });
                KeyValuePair<string, string>[] test = assemblyStation.GetState();
                Console.WriteLine(test.ToString());
                Assert.That(test, Is.All.Not.Null);
                Assert.That(test, Contains.Item(new KeyValuePair<string, string>("currentOperation", "123")));
            }

            {
                assemblyStation.SendCommand("name", "emulator/operation", new string[] { "456" });
                KeyValuePair<string, string>[] test = assemblyStation.GetState();
                Console.WriteLine(test.ToString());
                Assert.That(test, Is.All.Not.Null);
                Assert.That(test, Contains.Item(new KeyValuePair<string, string>("lastOperation", "123")));
                Assert.That(test, Contains.Item(new KeyValuePair<string, string>("currentOperation", "456")));
            }

            {
                assemblyStation.SendCommand("name", "emulator/operation", new string[] { "789" });
                KeyValuePair<string, string>[] test = assemblyStation.GetState();
                Console.WriteLine(test.ToString());
                Assert.That(test, Is.All.Not.Null);
                Assert.That(test, Contains.Item(new KeyValuePair<string, string>("lastOperation", "456")));
                Assert.That(test, Contains.Item(new KeyValuePair<string, string>("currentOperation", "789")));
            }
        }

        [Test]
        public void AssemblyStationSendCommandDefaultTest()
        {
            Assert.That(assemblyStation.SendCommand("name", "emulator/operation"), Is.Not.False);
            Assert.That(assemblyStation.GetState(), Contains.Item(new KeyValuePair<string, string>("currentOperation", "12345")));
        }

        [Test]
        public void AssemblyStationSendCommandSpecifyIDTest()
        {
            Assert.That(assemblyStation.SendCommand("name", "emulator/operation", new string[] { "789" }), Is.Not.False);
            Assert.That(assemblyStation.GetState(), Contains.Item(new KeyValuePair<string, string>("currentOperation", "789")));
        }

        [Test]
        public void AssemblyStationSendCommandSpecifyFalseIDTest()
        {
            Assert.Throws<ArgumentException>(() => assemblyStation.SendCommand("name", "emulator/operation", new string[] { "sdasfg" }), "Please insert numerical values only for processID");
        }

        [Test]
        public void AssemblyStationSendWrongCommandTest()
        {
            Assert.Throws<ArgumentException>(() => assemblyStation.SendCommand("name", "emulator/operator", new string[] { "789" }), "This command doesn't exist");
        }

        [Test]
        public void AssemblyStationSendCommandHealthFailureTest()
        {
            Assert.That(assemblyStation.SendCommand("name", "emulator/operation", new string[] { "9999" }), Is.Not.True);
        }
    }
}
