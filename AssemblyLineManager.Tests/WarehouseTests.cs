using AssemblyLineManager;
using NUnit.Framework;
using System.Xml.Linq;

namespace AssemblyLineManager.Warehouse
{
    public class WarehouseTests
    {
        string result = "";
        string expected = "";
        string item = "Test Item";
        int id = 1;
        [SetUp]
        public void Setup()
        {
            Warehouse.RunAsync().Wait();
            Warehouse.PickItem(1).Wait();
        }
        [Test]
        public void GetConnected_Connect_ReturnTrue()
        {
            Assert.IsTrue(Warehouse.Connected);
        }
        [Test]
        public void PickItem_PickedItem_ReturnTrue()
        {
            result = Warehouse.PickItem(id).Result;
            expected = "Picked item with id: " + id;
            StringAssert.Contains(expected, result);
        }
        [Test]
        public void InsertItem_InsertedItem_ReturnTrue()
        {
            result = Warehouse.InsertItem(id, item).Result;
            expected = "Inserted item "+ item + " on " + id;
            StringAssert.Contains(expected, result);
        }
        [Test]
        public void InsertItem_InsertItemOutOfBound_ReturnTrue()
        { //Planen er at se hvad der sker hvis man prøver at indsætte item på en plads der ikke er plads til i Inventory
            Warehouse.InsertItem(id, item).Wait();
            result = Warehouse.InsertItem(id, item).Result;
            expected = "Failed to insert item";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void GetInventoryAsync_ReturnsJSON_ReturnTrue()
        {
            Assert.AreSame("What are returned here?", Warehouse.GetInventoryAsync().Result);
        }

        [Test]
        public void GetInventory_ReturnsString_ReturnTrue()
        {
            
        }
    }
}