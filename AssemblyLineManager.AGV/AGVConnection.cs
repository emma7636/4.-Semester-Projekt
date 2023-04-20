namespace AssemblyLineManager.AGV
{
    //Setup connection to AGV
    public partial class AGVClient
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl = "http://localhost:8082/v1/status/";

        public AGVClient() => httpClient = new HttpClient();
    }
}