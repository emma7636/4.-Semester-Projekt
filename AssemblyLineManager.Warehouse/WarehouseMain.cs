﻿using System;
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
            //Warehouse.PickItem(7).Wait();
            //Console.WriteLine(Warehouse.InsertItem(7, "Marcus").Result);
            Console.WriteLine(Warehouse.GetInventoryItem(7));
            Warehouse warehouse = new Warehouse();
            warehouse.GetState();
        } 

    }
}