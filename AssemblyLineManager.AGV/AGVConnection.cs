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
                            var test = agvClient.ExecuteProgram();
                            test.Wait();
                            response = test.Result;
                            return true;
                        } catch (Exception)
                        {
                            return false;
                        }

                    // Commands to move AGV
                    case "MoveToChargerOperation":
                        Console.WriteLine("Moving to Charger");
                        var moveCharge  = agvClient.LoadProgram(command);
                        moveCharge.Wait(); 
                        response = moveCharge.Result;
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    case "MoveToAssemblyOperation":
                        Console.WriteLine("Moving to Assembly Station");
                        var moveAssemb = agvClient.LoadProgram(command);
                        moveAssemb.Wait();
                        response = moveAssemb.Result;
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    case "MoveToStorageOperation":
                        Console.WriteLine("Moving to Warehouse");
                        var moveWare = agvClient.LoadProgram(command);
                        moveWare.Wait();
                        response = moveWare.Result;
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    // Pick or Put commands:
                    // Assembly Station:
                    case "PutAssemblyOperation":
                        Console.WriteLine("Placing item on the Assembly Station");
                        var putAssemb = agvClient.LoadProgram(command);
                        putAssemb.Wait();
                        response = putAssemb.Result;
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    case "PickAssemblyOperation":
                        Console.WriteLine("Picking item from the Assembly Station");
                        var pickAssemb = agvClient.LoadProgram(command);
                        pickAssemb.Wait();
                        response = pickAssemb.Result;
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    //Warehouse:
                    case "PutWarehouseOperation":
                        Console.WriteLine("Placing item on the Warehouse");
                        var putWare = agvClient.LoadProgram(command);
                        putWare.Wait();
                        response = putWare.Result;
                        if (response != null)
                        {
                            return SendCommand("execute");
                        }
                        return false;

                    case "PickWarehouseOperation":
                        Console.WriteLine("Picking item from the Warehouse");
                        var pickWare = agvClient.LoadProgram(command);
                        pickWare.Wait();
                        response = pickWare.Result;
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