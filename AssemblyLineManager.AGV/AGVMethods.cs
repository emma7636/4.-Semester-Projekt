using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public bool CheckIsIdle()
        {
            string statusJson = GetStatus().GetAwaiter().GetResult();
            JObject status = JObject.Parse(statusJson);
            foreach (var value in status)
            {
                if (value.Key == "state" && value.Value != null)
                {
                    if (value.Value.Value<int>() == (int)AGVState.Idle)
                    {
                        // The State value has changed to Idle, set isStatusChecked to true
                        return true;
                    }
                    else
                    {
                        // The State value is no longer Idle, set isStatusChecked to false
                        return false;
                    }
                }
            }
            return false;
        }

        // Get the current status of the AGV
        public async Task<string> GetStatus()
        {
            HttpResponseMessage response = httpClient.GetAsync(baseUrl).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        // Load desired program into AGV
        public string LoadProgram(string programName)
        {
            // Check if the programName is empty or null
            if (string.IsNullOrEmpty(programName))
            {
                throw new ArgumentException("Invalid program name");
            }

            // Check if the status thread has finished checking the status
            if (!CheckIsIdle())
            {
                throw new InvalidOperationException("Cannot execute until previous job is finished");
            }

            Payload payload = new Payload
            {
                Program_name = programName,
                State = (int)AGVState.Idle
            };
            return SendPutRequest(payload);
        }


        // Execute previously specified program on the AGV
        public string ExecuteProgram()
        {
            // Check if the status thread has finished checking the status
            if (!CheckIsIdle())
            {
                throw new InvalidOperationException("Cannot execute until previous job is finished");
            }

            var payload = new
            {
                State = (int)AGVState.Executing
            };
            return SendPutRequest(payload);
        }

        // Send currently active PUT request (Load, Execute)
        public string SendPutRequest(object payload)
        {
            // Check if the status thread has finished checking the status
            if (!CheckIsIdle())
            {
                throw new InvalidOperationException("Cannot execute until previous job is finished");
            }

            string json = JsonConvert.SerializeObject(payload);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PutAsync(baseUrl, content).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                //isIdle = false;
                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Failed to send");
                Console.Out.WriteLine(json);
                Console.WriteLine($"Error: {response.StatusCode}");
                return "";
            }
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
