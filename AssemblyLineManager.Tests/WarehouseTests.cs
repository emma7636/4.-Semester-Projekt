using AssemblyLineManager;
using NUnit.Framework;
using System.Xml.Linq;

namespace AssemblyLineManager.Warehouse
{
    public class WarehouseTests
    {
        [SetUp]
        public void Setup()
        {
            Warehouse.RunAsync().Wait();
        }
        [Test]
        public void GetConnected_Connect_ReturnTrue()
        {
            Assert.IsTrue(Warehouse.Connected);
        }
        [Test]
        public void PickItem_ReceivedPickOperation_ReturnTrue()
        {
            Assert.AreSame("Inserted item Test Item on 1" , Warehouse.PickItem(1));
        }
        [Test]
        public void InsertItem_ReceivedInsertOperation_ReturnTrue()
        {   //Jeg fandt ud af hvordan vi får en string ud som return type i stedet for en eller anden Thread
            //DEN PASSER :CUM:
            string result = Warehouse.InsertItem(1, "Test Item").Result;
            string expected = "Inserted item Test Item on 1";
            Assert.IsTrue(expected == result);
        }
        [Test]
        public void InsertItem_InsertItemOutOfBound_ReturnTrue(int trayId, string name)
        { //Planen er at se hvad der sker hvis man prøver at indsætte item på en plads der ikke er plads til i Inventory
            Assert.AreSame("What returned here?", Warehouse.InsertItem(trayId, name).Result);
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