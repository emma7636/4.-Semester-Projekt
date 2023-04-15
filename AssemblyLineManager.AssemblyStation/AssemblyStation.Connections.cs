using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyLineManager.AssemblyStation
{
    public partial class AssemblyStation
    {
        private static string res = "";
        private static async Task ConnectToClient()
        {
            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithClientId("AssemblyLineManagerClient")
                .WithTcpServer("localhost", 1883)
                .WithCleanSession(true)
                .Build();

            _mqttClient.UseConnectedHandler(e =>
            {
                Console.WriteLine("Connected successfully with MQTT Brokers.");
                SubscribeToTopics().Wait();
                Console.WriteLine("Subscribed to topics.");
            });

            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("### RECEIVED APPLICATION MESSAGE ###")
                .AppendLine($"+ Topic = {e.ApplicationMessage.Topic}")
                .AppendLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}")
                .AppendLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}")
                .AppendLine($"+ Retain = {e.ApplicationMessage.Retain}\n");
                Console.WriteLine(sb.ToString());
            });

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
}
