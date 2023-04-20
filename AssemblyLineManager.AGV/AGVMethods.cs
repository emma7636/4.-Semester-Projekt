using System.Text;
using Newtonsoft.Json;

namespace AssemblyLineManager.AGV
{
    public partial class AGVClient
    {
        public enum AGVState
        {
            Idle = 1,
            Executing = 2,
            Charging = 3
        }

        // Get the current status of the AGV
        public async Task<string> GetStatus()
        {
            HttpResponseMessage response = await httpClient.GetAsync(baseUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        // Load desired program into AGV
        public async Task<string> LoadProgram(string programName)
        {
            Payload payload = new Payload
            {
                Program_name = programName,
                State = (int)AGVState.Idle
            };
            return await SendPutRequest(payload);
        }

        // Execute previously specified program on the AGV
        public async Task<string> ExecuteProgram()
        {
            var payload = new
            {
                State = (int)AGVState.Executing
            };
            return await SendPutRequest(payload);
        }

        // Send currently active PUT request (Load, Execute)
        private async Task<string> SendPutRequest(object payload)
        {
            string json = JsonConvert.SerializeObject(payload);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PutAsync(baseUrl, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }

    //Fixing Program name Json issue
    public class Payload
    {
        [JsonProperty("Program name")]
        public string Program_name { get; set; }

        public Payload()
        {
            Program_name = string.Empty;
        }
        public int State { get; set; }
    }
}
