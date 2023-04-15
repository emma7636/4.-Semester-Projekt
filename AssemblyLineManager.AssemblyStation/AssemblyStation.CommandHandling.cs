using MQTTnet;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyLineManager.AssemblyStation
{
    public partial class AssemblyStation
    {
        public static async Task SendCommand()
        {
            MqttApplicationMessage var = new MqttApplicationMessageBuilder()
            .WithTopic("emulator/operation")
            .WithPayload("{\n\"ProcessID\": 12345\n}")
            .WithPayloadFormatIndicator(MqttPayloadFormatIndicator.CharacterData)
            .WithContentType("schema:JSON")
            .Build();
            if (mqttClient != null) {
                if (mqttClient.IsConnected) {
                    await mqttClient.PublishAsync(var);
                }
                else {
                    Console.WriteLine("Lost connection to broker");
                }
            }
            else {
                Console.WriteLine("This shouldn't have happened");
             }
        }
    }
}