using System.Text;
using Newtonsoft.Json;

namespace AssemblyLineManager.AGV
{
    public partial class AGVClient
    {
        private bool isStatusChecked = false;
        private Thread statusThread;

        public enum AGVState
        {
            Idle = 1,
            Executing = 2,
            Charging = 3
        }

        // Start the thread to check the status and send the request
        public void StartStatusThread()
        {
            statusThread = new Thread(async () =>
            {
                // Keep checking the status until the State value changes to 1
                while (true)
                {
                    string statusJson = await GetStatus();
                    dynamic status = JObject.Parse(statusJson);

                    if ((int)status.State == 1)
                    {
                        break;
                    }

                    // Sleep for 1 second before checking the status again
                    await Task.Delay(1000);
                }

                // The State value has changed to 1, set isStatusChecked to true
                isStatusChecked = true;
            });

            statusThread.Start();
        }

        // Stop the status thread
        public void StopStatusThread()
        {
            statusThread?.Abort();
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
        public string LoadProgram(string programName)
        {
            // Check if the status thread has finished checking the status
            if (!isStatusChecked)
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
            if (!isStatusChecked)
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
        private string SendPutRequest(object payload)
        {
            // Check if the status thread has finished checking the status
            if (!isStatusChecked)
            {
                throw new InvalidOperationException("Cannot execute until previous job is finished");
            }

            string json = JsonConvert.SerializeObject(payload);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PutAsync(baseUrl, content).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
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
