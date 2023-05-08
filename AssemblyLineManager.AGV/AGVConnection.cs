using AssemblyLineManager.CommonLib;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AssemblyLineManager.AGV
{
    //Setup connection to AGV
    public partial class AGVClient: ICommunicationController
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl = "http://localhost:8082/v1/status/";


        public AGVClient() => httpClient = new HttpClient();

        // Constructor that accepts an HttpClient parameter
        public AGVClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        //ICommunicationController methods implemented
        public KeyValuePair<string, string>[] GetState()
        {
            AGVClient agvClient = new AGVClient();
            var status = agvClient.GetStatus().Result;
            //Deserialize the JSON Object to a dictionary
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(status);
            
            //Console.WriteLine("Test Dictionary Solution + \n");
       
            if (dict != null)
            {
                // Makes the dictionary into an array to match the desired return datatype for this method.
                var list = dict.ToArray();
                /*// Not important but nice to see what is in the dictionary
                foreach (var kv in list)
                {
                    Console.WriteLine(kv.Key + ": " + kv.Value);
                   
                }*/

                return list;
            }
            // In the event we are unable to obtain the information from the AGV. This may be restructured so that a null case is handled elsewhere.

            else 
            { 
                Console.WriteLine("Dictionary was null + \n");
                return new KeyValuePair<string, string>[1]{ new KeyValuePair<string, string>("Error", "Could not connect") };  // Tomt array eller kast en fejl
            }
        }

        public bool SendCommand(string machineName, string command, string[]? commandParameters = null)
        {
            //This ifelse statement is not needed currently but as we redesign the code it may be useful should we implement further criteria for the command argument
            AGVClient agvClient = new AGVClient();
            if (command != "") // bedre gate senere
            {
                switch (command)
                {   
                    // If execute is received
                    case "execute":
                        Console.WriteLine("Program will execute");
                        //string loadProgramResult = agvClient.LoadProgram(command);
                        return true;

                    // Commands to move AGV
                    case "MoveToChargerOperation":
                        Console.WriteLine("Moving to Charger");
                        return true;

                    case "MoveToAssemblyOperation":
                        Console.WriteLine("Moving to Assembly Station");
                        return true;
                    case "MoveToStorageOperation":
                        Console.WriteLine("Moving to Warehouse");
                        return true;

                    // Pick or Put commands:
                     // Assembly Station:
                    case "PutAssemblyOperation":
                        Console.WriteLine("Placing item on the Assembly Station");
                        return true;
                    case "PickAssemblyOperation":
                        Console.WriteLine("Picking item from the Assembly Station");
                        return true;

                     //Warehouse:
                    case "PutWarehouseOperation":
                        Console.WriteLine("Placing item on the Warehouse");
                        return true;
                    case "PickWarehouseOperation":
                        Console.WriteLine("Picking item from the Warehouse");
                        return true;


                    // if the command received is not recognized, then return false
                    default:
                        Console.WriteLine("Command not understood, no command will be executed");
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}