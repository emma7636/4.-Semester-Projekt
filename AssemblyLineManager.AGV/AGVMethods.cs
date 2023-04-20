using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AssemblyLineManager.AGV
{
    public partial class AGVClient
    {
        //Get the current status of the AGV
        public async Task<string> GetStatus()
        {
            HttpResponseMessage response = await httpClient.GetAsync(baseUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        //Load desired program into AGV
        public async Task<string> LoadProgram(string programName)
        {
            var payload = new
            {
                ProgramName = programName,
                State = 1
            };
            return await SendPutRequest(payload);
        }

        //Execute previously specified program on the AGV
        public async Task<string> ExecuteProgram()
        {
            var payload = new
            {
                State = 2
            };
            return await SendPutRequest(payload);
        }

        //Send currently active PUT request (Load, Execute)
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
}
