using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyLineManager.Warehouse
{
    internal class WarehouseMain
    {
        static void Main(string[] args)
        {
            Warehouse.RunAsync().Wait();
            Console.WriteLine(Warehouse.GetInventoryAsync());
        }

    }
}
