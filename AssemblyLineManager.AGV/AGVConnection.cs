using AssemblyLineManager.CommonLib;

namespace AssemblyLineManager.AGV
{
    //Setup connection to AGV
    public partial class AGVClient: ICommunicationController
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl = "http://localhost:8082/v1/status/";


        public AGVClient() => httpClient = new HttpClient();

        // Constructor that accepts an HttpClient parameter
        public AGVClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        //ICommunicationController methods implemented
        public KeyValuePair<string, string>[] GetState()
        {
            throw new NotImplementedException();
        }

        public bool SendCommand(string machineName, string command, string[]? commandParameters = null)
        {
            throw new NotImplementedException();
        }
    }
}