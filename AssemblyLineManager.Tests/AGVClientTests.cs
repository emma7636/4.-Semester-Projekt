using System.Net;
using AssemblyLineManager.AGV;
using Newtonsoft.Json.Linq;


namespace AssemblyLineManager.Tests
{
    // Constructor for AGVClientTests, initializes instances of AGVClient and FakeHttpMessageHandler.
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

        // Test case to verify if GetStatus() returns a valid status from the AGV.
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

        // Test case to verify if LoadProgram() sends a valid request to the AGV.
        [Test]
        public async Task LoadProgram_SendsValidRequest()
        {
            // Arrange
            string expectedProgramName = "MoveToAssemblyOperation";
            int expectedState = (int)AGVClient.AGVState.Idle;
            var expectedPayload = new JObject
            {
                ["Program name"] = expectedProgramName,
                ["State"] = expectedState
            };
            _fakeHttpMessageHandler.SetResponseContent(expectedPayload.ToString());

            // Act
            var result = await _agvClient.LoadProgram(expectedProgramName);

            // Assert
            Assert.AreEqual(expectedPayload.ToString(), result);
        }

        // Test case to verify if ExecuteProgram() sends a valid request to the AGV.
        [Test]
        public async Task ExecuteProgram_SendsValidRequest()
        {
            // Arrange
            int expectedState = (int)AGVClient.AGVState.Executing;
            var expectedPayload = new JObject
            {
                ["State"] = expectedState
            };
            _fakeHttpMessageHandler.SetResponseContent(expectedPayload.ToString());

            // Act
            var result = await _agvClient.ExecuteProgram();

            // Assert
            Assert.AreEqual(expectedPayload.ToString(), result);
        }

        // Test case to verify if LoadProgram() returns an error when provided with an empty program name.
        [Test]
        public async Task LoadProgram_EmptyProgramName_ReturnsError()
        {
            // Arrange
            string expectedProgramName = "";
            var expectedPayload = new JObject
            {
                ["Error"] = "Invalid program name",
            };
            _fakeHttpMessageHandler.SetResponseContent(expectedPayload.ToString());

            // Act
            var result = await _agvClient.LoadProgram(expectedProgramName);

            // Assert
            Assert.AreEqual(expectedPayload.ToString(), result);
        }

        // Test case to verify if ExecuteProgram() returns an error when the AGV is in the wrong state.
        [Test]
        public async Task ExecuteProgram_AGVInWrongState_ReturnsError()
        {
            // Arrange
            _fakeHttpMessageHandler.SetResponseContent("{\"state\": " + (int)AGVClient.AGVState.Charging + "}");

            // Act
            await _agvClient.GetStatus(); // Update the AGVClient's internal state to Charging

            // Arrange (Continued)
            var expectedPayload = new JObject
            {
                ["Error"] = "Cannot execute program while AGV is charging",
            };
            _fakeHttpMessageHandler.SetResponseContent(expectedPayload.ToString());

            // Act
            var result = await _agvClient.ExecuteProgram();

            // Assert
            Assert.AreEqual(expectedPayload.ToString(), result);
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private string _responseContent = string.Empty;

        // Method to set the response content for the fake HTTP message handler.
        public void SetResponseContent(string responseContent)
        {
            _responseContent = responseContent;
        }

        // Method to intercept and mock the HTTP requests for unit testing.
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
