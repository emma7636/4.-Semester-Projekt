using AssemblyLineManager.CommonLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private static Dictionary<int, string> stateLUT = new Dictionary<int, string>()
        {
            {1, "Idle" },
            {2, "Executing" },
            {3, "Charging" }
        };
        // Constructor that accepts an HttpClient parameter
        public AGVClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;

        }

        public string Name
        {
            get
            {
                return "AGV";
            }
        }

        //ICommunicationController methods implemented
        public KeyValuePair<string, string>[] GetState()
        {   
            AGVClient agvClient = new AGVClient();
            
            var status = agvClient.GetStatus().Result;
            dynamic dyna = JObject.Parse(status);
            int battery = (int)dyna["battery"];
            string program = (string)dyna["program name"];
            int state = (int)dyna["state"];
            string timestamp = (string)dyna["timestamp"];
            //Deserialize the JSON Object to a dictionary
            //var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(status);
            KeyValuePair<string, string>[] stateArray = new KeyValuePair<string, string>[4];
            stateArray[0] = new KeyValuePair<string, string>("battery", battery.ToString());
            stateArray[1] = new KeyValuePair<string, string>("program name", program);
            stateArray[2] = new KeyValuePair<string, string>("state", stateLUT[state]);
            stateArray[3] = new KeyValuePair<string, string>("timestamp", timestamp.ToString());

            //Console.WriteLine("Test Dictionary Solution + \n");

            if (stateArray != null)
            {
                // Makes the dictionary into an array to match the desired return datatype for this method.
                //var list = dict.ToArray();
                /*// Not important but nice to see what is in the dictionary
                foreach (var kv in list)
                {
                    Console.WriteLine(kv.Key + ": " + kv.Value);
                   
                }*/

                //return list;
                return stateArray;
            }
            // In the event we are unable to obtain the information from the AGV. This may be restructured so that a null case is handled elsewhere.

            else 
            { 
                Console.WriteLine("Dictionary was null + \n");
                return new KeyValuePair<string, string>[1]{ new KeyValuePair<string, string>("Error", "Could not connect") };  // Tomt array eller kast en fejl
            }
        }

        public bool SendCommand(string command, string[]? commandParameters = null)
        {
            //This ifelse statement is not needed currently but as we redesign the code it may be useful should we implement further criteria for the command argument
            AGVClient agvClient = new AGVClient();
            if (command != "") // bedre gate senere
            {
                string response;
                switch (command)
                {
                    // If execute is received
                    case "execute":
                        Console.WriteLine("Program will execute");
                        //string loadProgramResult = agvClient.LoadProgram(command);
                        try
                        {
                            response = agvClient.ExecuteProgram();
                            while (!CheckIsIdle())
                            {
                                Thread.Sleep(100);
                            }
                            return true;
                        } catch (Exception)
                        {
                            return false;
                        }

                    // Commands to move AGV
                    case "MoveToChargerOperation":
                        Console.WriteLine("Moving to Charger");
                        response = agvClient.LoadProgram(command);
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    case "MoveToAssemblyOperation":
                        Console.WriteLine("Moving to Assembly Station");
                        response = agvClient.LoadProgram(command);
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    case "MoveToStorageOperation":
                        Console.WriteLine("Moving to Warehouse");
                        response = agvClient.LoadProgram(command);
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    // Pick or Put commands:
                    // Assembly Station:
                    case "PutAssemblyOperation":
                        Console.WriteLine("Placing item on the Assembly Station");
                        response = agvClient.LoadProgram(command);
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    case "PickAssemblyOperation":
                        Console.WriteLine("Picking item from the Assembly Station");
                        response = agvClient.LoadProgram(command);
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    //Warehouse:
                    case "PutWarehouseOperation":
                        Console.WriteLine("Placing item on the Warehouse");
                        response = agvClient.LoadProgram(command);
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    case "PickWarehouseOperation":
                        Console.WriteLine("Picking item from the Warehouse");
                        response = agvClient.LoadProgram(command);
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;


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