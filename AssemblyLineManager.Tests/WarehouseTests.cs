using AssemblyLineManager;
using NUnit.Framework;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace AssemblyLineManager.Warehouse
{
    public class WarehouseTests
    {
        Warehouse warehouse = new Warehouse();
        [SetUp]
        public void Setup()
        {
          
        }
        [Test]
        public void GetConnected_Connect_ReturnTrue()
        {
            Warehouse.RunAsync().Wait();
            Assert.IsTrue(Warehouse.Connected);
        }
        [Test]
        public void SendCommand_SendsTheCommand_ReturnsTrue()
        {
            string[] commands = { "1", "Test Item" };
            warehouse.SendCommand("Pick Item", commands);
            bool shouldBeTrue = warehouse.SendCommand("Insert Item", commands);
            Assert.IsTrue(shouldBeTrue);
        }
        [Test]
        public void GetState_IdleState_ReturnsTrue()
        {
            // HELP I DONT KNOW HOW TO TEST THIS
            List<KeyValuePair<string, string>> expectedList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("State: ", "0")
            };
            KeyValuePair<string, string>[] expected = expectedList.ToArray();
            KeyValuePair<string, string>[] result = warehouse.GetState();

           
        }
    }
}