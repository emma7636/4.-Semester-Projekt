using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyLineManager.AssemblyStation
{
    public partial class AssemblyStation
    {
        private static MqttFactory? mqttFactory;
        private static IMqttClient? mqttClient;
        private static MqttClientOptions? options;
        private static async Task ConnectToClient()
        {
            mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();
            options = new MqttClientOptionsBuilder()
                .WithClientId("AssemblyStationClient")
                .WithTcpServer("localhost", 1883)
                .WithCleanSession()
                .Build();
            try
            {
                await mqttClient.ConnectAsync(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connecting to MQTT Brokers failed.");
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task DisconnectFromClient()
        {
            try
            {
                await mqttClient.DisconnectAsync(MqttClientDisconnectReason.ServerShuttingDown);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Disconnect from Broker failed.");
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task SubscribeToTopics()
        {
            if (mqttFactory != null && mqttClient != null)
            {
                if (mqttClient.IsConnected)
                {
                    var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("emulator/status");
                    }
                ).WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("emulator/checkhealth");
                    }
                ).Build();

                    await mqttClient.SubscribeAsync(subscribeOptions);
                }
                else
                {
                    Console.WriteLine("Lost connection to broker");
                }
            }
            else
            {
                throw new Exception("This shouldn't have happened!");
            }
        }
    }
}
