using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System.Text;

namespace AssemblyLineManager.AssemblyStation;

public partial class AssemblyStation
{
    //Storing the packets we receive so we can access them when needed
    private Echo? latestEcho;
    private Status latestStatus = new Status();
    private CheckHealth? latestCheckHealth;

    private async Task SendPayload(int operation)
    {
        //Building a payload
        MqttApplicationMessage var = new MqttApplicationMessageBuilder()
        .WithTopic("emulator/operation")
        .WithPayload($"{{\n\"ProcessID\": {operation}\n}}") //We have to manually create this JSON as the assembly station doesn't understand what the library spits out.
        .WithPayloadFormatIndicator(MqttPayloadFormatIndicator.CharacterData)
        .WithContentType("schema:JSON")
        .WithAtLeastOnceQoS()
        .Build();
        if (_mqttClient.IsConnected) //Checking we are connected
        {
            await _mqttClient.PublishAsync(var); //Send the payload
        }
        else
        {
            Console.WriteLine("Lost connection to broker");
        }
    }

    //A method used as a delegate for the MqttClient.Connected event
    private void MessageReceivedHandling(MqttApplicationMessageReceivedEventArgs e)
    {
        string json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        switch (e.ApplicationMessage.Topic){ //Checking what topic the message is from so we know how to handle it
            case "emulator/echo":
                Console.WriteLine("echo");
                Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                Console.WriteLine("QoS: " + e.ApplicationMessage.QualityOfServiceLevel.ToString());

                Echo? newEcho = newEcho = JsonConvert.DeserializeObject<Echo>(json);
                if (newEcho != null)
                {
                    latestEcho = newEcho;
                }
                else //In case the json is corrupt and cannot be deserialized
                {
                    Console.WriteLine("Corrupt network packet received!");
                    e.ProcessingFailed = true; //Don't know if this does anything for us, just thought it would be best practice to do.
                }

                break;
            case "emulator/status":
                Console.WriteLine("status");
                Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                Console.WriteLine("QoS: " + e.ApplicationMessage.QualityOfServiceLevel.ToString());

                Status? newStatus = JsonConvert.DeserializeObject<Status>(json);
                if (newStatus != null)
                {
                    latestStatus = newStatus;
                }
                else //In case the json is corrupt and cannot be deserialized
                {
                    Console.WriteLine("Corrupt network packet received!");
                    e.ProcessingFailed = true; //Don't know if this does anything for us, just thought it would be best practice to do.
                }
                break;
            case "emulator/checkhealth":
                Console.WriteLine("checkhealth");
                Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                Console.WriteLine("QoS: " + e.ApplicationMessage.QualityOfServiceLevel.ToString());

                json = json[1..^1]; //This removes the quotation marks that seem to have been accidentally malplaced by the assembly station simulator.
                CheckHealth? newCheckHealth = JsonConvert.DeserializeObject<CheckHealth>(json);
                if (newCheckHealth != null)
                {
                    latestCheckHealth = newCheckHealth;
                }
                else //In case the json is corrupt and cannot be deserialized
                {
                    Console.WriteLine("Corrupt network packet received!");
                    e.ProcessingFailed = true; //Don't know if this does anything for us, just thought it would be best practice to do.
                }
                break;
            default: //In case we receive a packet with a topic we aren't handling
                Console.WriteLine("What is this?");
                Console.WriteLine(e.ToString());
                e.IsHandled = false; //Best practice?
                break;
        }
    }
}

//These classes are used to serialise/deserialise the JSON we receive from the assembly station simulator
public class Operation
{
    public int ProcessId { get; set; }

    public Operation(int processId)
    {
        ProcessId = processId;
    }
}

public class Echo
{
    public int ProcessID { get; set; }

    public Echo() { }
}

public class Status
{
    public int LastOperation { get; set; }
    public int CurrentOperation { get; set; }
    public int State { get; set; }
    public DateTime TimeStamp { get; set; }

    public Status() { }
}

public class CheckHealth
{
    public bool IsHealthy { get; set; }
    public CheckHealth() { }
}