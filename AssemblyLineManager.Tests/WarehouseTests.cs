using AssemblyLineManager;
using NUnit.Framework;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace AssemblyLineManager.Warehouse
{
    public class WarehouseTests
    {
  
        string item = "Test Item";
        int id = 1;
        
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
        public void PickItem_PickedItem_ReturnTrue()
        {
            string result = Warehouse.PickItem(id).Result;
            string expected = "Picked item with id: " + id;
            StringAssert.Contains(expected, result);
        }
        [Test]
        public void InsertItem_InsertedItem_ReturnTrue()
        {
            Warehouse.PickItem(id).Wait();
            string result = Warehouse.InsertItem(id, item).Result;
            string expected = "Inserted item "+ item + " on " + id;
            StringAssert.Contains(expected, result);
        }
        [Test]
        public void InsertItem_InsertItemOutOfBound_ReturnTrue()
        { //Planen er at se hvad der sker hvis man prøver at indsætte item på en plads der ikke er plads til i Inventory
            Warehouse.InsertItem(id, item).Wait();
            string result = Warehouse.InsertItem(id, item).Result;
            string expected = "Failed to insert item";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void GetInventoryItem_FindsCorrectItem_ReturnsTrue()
        {
            Warehouse.InsertItem(id, item).Wait();
            Item testItem = new Item(1, "Test Item");
            string expected = testItem.ToString();
            string result = Warehouse.GetInventoryItem(1).ToString();
            StringAssert.Contains(expected, result);
        }
        [Test]
        public void GetInventoryCount_ReturnsTen_ReturnsTrue()
        {
            Warehouse.RunAsync().Wait();
            int result = Warehouse.GetInventoryCount();
            int expected = 10;
            Assert.AreEqual(expected, result);
        }
        [Test]
        public void SendCommand_SendsTheCommand_ReturnsTrue()
        {
            Warehouse.RunAsync().Wait();
            string[] commands = { "1", "Test Item" };
            Warehouse warehouse = new Warehouse();
            bool shouldBeTrue = warehouse.SendCommand("Warehouse", "Insert Item", commands);
            Assert.IsTrue(shouldBeTrue);
        }
        [Test]
        public void GetState_IdleState_ReturnsTrue()
        {
            // HELP I DONT KNOW HOW TO TEST THIS.
            Warehouse.RunAsync().Wait();
            Warehouse warehouse = new Warehouse();
            List<KeyValuePair<string, string>> expectedList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("State: ", "0")
            };
            KeyValuePair<string, string>[] expected = expectedList.ToArray();
            KeyValuePair<string, string>[] result = warehouse.GetState();

           
        }
    }
}