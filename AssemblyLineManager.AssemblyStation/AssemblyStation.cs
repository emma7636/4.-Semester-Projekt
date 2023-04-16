using AssemblyLineManager.CommonLib;
using MQTTnet.Client;
using MQTTnet;

namespace AssemblyLineManager.AssemblyStation;

public partial class AssemblyStation : ICommunicationController
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static MqttFactory _mqttFactory;
    private static IMqttClient _mqttClient;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AssemblyStation()
    {
        ConnectToClient().Wait();
    }

    public KeyValuePair<int, string> GetState()
    {
        
        throw new NotImplementedException();
    }

    public bool SendCommand(string machineName, string command, string[]? commandParameters = null)
    {
        throw new NotImplementedException();
    }

    ~AssemblyStation()
    {
        DisconnectFromClient().Wait();
    }

    public static void Main(string[] args)
    {
        AssemblyStation assemblyStation = new AssemblyStation();
        while (true)
        {
            SendCommand().Wait();
            Thread.Sleep(10000);
        }
    }
}