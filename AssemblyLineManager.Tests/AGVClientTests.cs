using System.Net;
using AssemblyLineManager.AGV;
using Newtonsoft.Json.Linq;


namespace AssemblyLineManager.Tests
{
    public class AGVClientTests
    {
        private readonly AGVClient _agvClient;
        private readonly FakeHttpMessageHandler _fakeHttpMessageHandler;
        public AGVClientTests()

        {
            _fakeHttpMessageHandler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(_fakeHttpMessageHandler);
            _agvClient = new AGVClient(httpClient);
        }

        [Test]
        public async Task GetStatus_ReturnsValidStatus()
        {
            // Arrange
            var expectedPayload = new JObject
            {
                ["battery"] = 100,
                ["program name"] = "no program loaded",
                ["state"] = 1,
                ["timestamp"] = "2023-04-27T11:41:52.2793486+00:00"
            };
            _fakeHttpMessageHandler.SetResponseContent(expectedPayload.ToString());

            // Act
            var result = await _agvClient.GetStatus();

            // Assert
            Assert.AreEqual(expectedPayload.ToString(), result);
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private string _responseContent = string.Empty;

        public void SetResponseContent(string responseContent)
        {
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responseContent)
            };
            return Task.FromResult(response);
        }
    }
}
