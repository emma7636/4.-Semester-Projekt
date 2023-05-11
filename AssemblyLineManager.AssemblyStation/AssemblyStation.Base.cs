using AssemblyLineManager.CommonLib;
using MQTTnet;

namespace AssemblyLineManager.AssemblyStation;

public partial class AssemblyStation : ICommunicationController
{
    private static Dictionary<int, string> stateLUT = new Dictionary<int, string>(); //Lookup table for the various states of the assembly station

    public string Name
    {
        get
        {
            return "AssemblyStation";
        }
    }

    static AssemblyStation(){
        //Generate a lookup table for the various states
        stateLUT.Add(0, "Idle");
        stateLUT.Add(1, "Executing");
        stateLUT.Add(2, "Error");

        //Just shut up already
        _mqttFactory = new MqttFactory();
    }

    //Establish connection to the MQTT broker
    public AssemblyStation()
    {
        //Just shut up already (again)
        _mqttClient = _mqttFactory.CreateMqttClient();

        ConnectToClient().Wait();
    }

    public KeyValuePair<string, string>[] GetState()
    {
        //Generate a labeled array of the current state
        KeyValuePair<string, string>[] stateArray = new KeyValuePair<string, string>[4];
        stateArray[0] = new KeyValuePair<string, string>("lastOperation", latestStatus.LastOperation.ToString());
        stateArray[1] = new KeyValuePair<string, string>("currentOperation", latestStatus.CurrentOperation.ToString());
        stateArray[2] = new KeyValuePair<string, string>("state", stateLUT[latestStatus.State]);
        stateArray[3] = new KeyValuePair<string, string>("timeStamp", latestStatus.TimeStamp.ToString());
        return stateArray;
    }

    public bool SendCommand(string command, string[]? commandParameters = null)
    {
        //Deleting old data so we don't get confused
        latestEcho = null;
        latestCheckHealth = null;

        if(command.Equals("emulator/operation")) //Checking what command they want us to send
        {
            if (!_mqttClient.IsConnected) //Checking we are connected
            {
                Console.WriteLine("Connection to assembly station lost!");
                return false;
            }

            if (commandParameters!=null&&commandParameters.Length>0) //If the user wants to send a specific processID
            {
                if (int.TryParse(commandParameters[0], out int process)) //Make sure the user actually entered a number
                {
                    SendPayload(process).Wait(); //Send the command
                }
                else
                {
                    throw new ArgumentException("Please insert numerical values only for processID");
                }
            }
            else
            {
                SendPayload(12345).Wait(); //Send the command with default numbering
            }

            while (_mqttClient.IsConnected) //Just in case we lose connection while waiting
            {
                DateTime time = DateTime.Now;
                while (latestCheckHealth == null) //Wait for the checkHealth packet to arrive
                {
                    if ((DateTime.Now - time).TotalSeconds > 12) //If we don't get a response in 12 seconds, something is wrong
                    {
                        Console.WriteLine("CheckHealth packet didn't arrive, is everything okay?\nContinuing...");
                        return false;
                    }
                    try
                    {
                        Thread.Sleep(100);
                    }
                    catch (ThreadInterruptedException){ }
                }
                return latestCheckHealth.IsHealthy; //Return the success of the assembled product
            }

            Console.WriteLine("Connection to assembly station lost!");
            return false;
        }
        else
        {
            throw new ArgumentException("This command doesn't exist"); //You made a spelling mistake fool
        }
    }

    //Make sure to close the connection when we're done
    ~AssemblyStation()
    {
        DisconnectFromClient().Wait();
    }

    /*public static void Main(string[] args)
    {
        AssemblyStation assemblyStation = new AssemblyStation();
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine();
            Console.WriteLine(assemblyStation.GetState());
            Console.WriteLine(assemblyStation.SendCommand("name", "emulator/operation"));
        }
    }*/
}