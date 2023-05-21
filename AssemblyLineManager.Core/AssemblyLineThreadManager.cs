using AssemblyLineManager.CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyLineManager.Core
{
    internal class AssemblyLineThreadManager
    {
        Dictionary<string, ICommunicationController> instances;
        Thread thread;
        ManualResetEvent manualResetEvent = new ManualResetEvent(true);

        public AssemblyLineThreadManager(Dictionary<string, ICommunicationController> instances)
        {
            this.instances = instances;

            thread = new Thread(AssemblyLineThreadAll);
        }

        public bool isRunning()
        {
            return manualResetEvent.WaitOne(0);
        }
        /**
        * Starts the production thread, checks if it needs to assemble all items or just the given id
        * 
        * @param bool allItems
        * @param int id = 0
        */
        public void StartThread(bool allItems, int id)
        {
            manualResetEvent.Set();
            if (allItems)
            {
                thread = new Thread(AssemblyLineThreadAll);
            } else
            {
                thread = new Thread(() => AssemblyLineThread(id.ToString()));
                
            }
            thread.Start();
        }

        public void ResumeThread()
        {
            manualResetEvent.Set();
        }

        public void PauseThread()
        {
            manualResetEvent.Reset();
        }

        public void StopThread()
        {
            manualResetEvent.Reset();
            thread.Interrupt();
            thread.Join();
        }

        public void AssemblyLineThread(string id)
        {
            Func<bool>[] orderOfOperations = new Func<bool>[] {
                //() => instances["Warehouse"].SendCommand("Put Item", new string[] { "1", "Assemblen't Piece" }),
                () => instances["Warehouse"].SendCommand("Pick Item", new string[] { id }),
                () => instances["AGV"].SendCommand("MoveToStorageOperation"),
                () => instances["AGV"].SendCommand("PickWarehouseOperation"),
                () => instances["AGV"].SendCommand("MoveToAssemblyOperation"),
                () => instances["AGV"].SendCommand("PutAssemblyOperation"),
                () => instances["AssemblyStation"].SendCommand("emulator/operation"),
                () => instances["AGV"].SendCommand("PickAssemblyOperation"),
                () => instances["AGV"].SendCommand("MoveToStorageOperation"),
                () => instances["AGV"].SendCommand("PutWarehouseOperation"),
                () => instances["Warehouse"].SendCommand("Insert Item", new string[] { id, "Assembled Piece" })
            };

            foreach (var operation in orderOfOperations)
            {
                manualResetEvent.WaitOne();
                if (operation())
                {
                    Console.WriteLine(operation.Method.ToString() + " completed successfully!");
                }
                else
                {
                    Console.WriteLine(operation.Method.ToString() + " did not complete successfully!");
                }
            }
        }
        /**
         * Runs the assembly function on all current spaces in the Warehouse inventory
         */
        public void AssemblyLineThreadAll()
        {
            for (int i = 1; i < 11; i++)
            {
                AssemblyLineThread(i.ToString());
            }
        }
    }
}
