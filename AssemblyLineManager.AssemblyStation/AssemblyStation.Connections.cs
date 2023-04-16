using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;

namespace AssemblyLineManager.AssemblyStation;

public partial class AssemblyStation
{
    private static readonly MqttFactory _mqttFactory;
    private static readonly IMqttClient _mqttClient;

    private static async Task ConnectToClient()
    {
        IMqttClientOptions options = new MqttClientOptionsBuilder()
            .WithClientId("AssemblyLineManagerClient")
            .WithTcpServer("localhost", 1883)
            .WithCleanSession(true)
            .Build();

        _mqttClient.UseConnectedHandler(e =>
        {
            Console.WriteLine("Connected successfully with MQTT Broker.");
            SubscribeToTopics().Wait();
            Console.WriteLine("Subscribed to topics.");
        });

        _mqttClient.UseDisconnectedHandler(e =>
        {
            Console.WriteLine("Disconnected from MQTT Broker unexpectedly.");
        });

        _mqttClient.UseApplicationMessageReceivedHandler(MessageReceivedHandling);

        await _mqttClient.ConnectAsync(options);
    }

    private static async Task DisconnectFromClient()
    {
        try
        {
            await _mqttClient.DisconnectAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Disconnect from Broker failed.");
            Console.WriteLine(ex.Message);
        }
    }

    private static async Task SubscribeToTopics()
    {
        var subscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter("emulator/status")
            .WithTopicFilter("emulator/checkhealth")
            .WithTopicFilter("emulator/echo")
            .Build();

        await _mqttClient.SubscribeAsync(subscribeOptions);
    }
}
