using MQTTnet;
using MQTTnet.Client;
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
        private static async Task SendCommand()
        {
            MqttApplicationMessage var = new MqttApplicationMessageBuilder()
            .WithTopic("emulator/operation")
            .WithPayload("{\n\"ProcessID\": 12345\n}")
            .WithPayloadFormatIndicator(MqttPayloadFormatIndicator.CharacterData)
            .WithContentType("schema:JSON")
            .Build();
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.PublishAsync(var);
            }
            else
            {
                Console.WriteLine("Lost connection to broker");
            }
        }
    }
}