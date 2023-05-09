using System.Net;
using AssemblyLineManager.AGV;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

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
            _fakeHttpMessageHandler.SetResponseContent("{\"state\": 1}"); // Set the AGV state to Idle

            // Act
            _agvClient.LoadProgram(expectedProgramName);

            // Assert
            if (_fakeHttpMessageHandler.LastRequest?.Content == null)
            {
                Assert.Fail("Request content is null.");
            }
            else
            {
                var actualPayloadJson = await _fakeHttpMessageHandler.LastRequest.Content.ReadAsStringAsync();
                var actualPayload = JObject.Parse(actualPayloadJson);
                Assert.IsTrue(JToken.DeepEquals(expectedPayload, actualPayload));
            }
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
            _fakeHttpMessageHandler.SetResponseContent("{\"state\": 1}"); // Set the AGV state to Idle

            // Act
            _agvClient.ExecuteProgram();

            // Assert
            if (_fakeHttpMessageHandler.LastRequest?.Content == null)
            {
                Assert.Fail("Request content is null.");
            }
            else
            {
                var actualPayload = JObject.Parse(await _fakeHttpMessageHandler.LastRequest.Content.ReadAsStringAsync());
                Assert.AreEqual(expectedPayload.ToString(Formatting.None), actualPayload.ToString(Formatting.None));
            }
        }

        // Test case to verify if LoadProgram() returns an error when provided with an empty program name.
        [Test]
        public void LoadProgram_EmptyProgramName_ThrowsException()
        {
            // Arrange
            string expectedProgramName = "";

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => _agvClient.LoadProgram(expectedProgramName));

            if (exception != null)
                Assert.AreEqual("Invalid program name", exception.Message);
        }


        // Test case to verify if ExecuteProgram() returns an error when the AGV is in the wrong state.
        [Test]
        public async Task ExecuteProgram_AGVInWrongState_ReturnsError()
        {
            // Arrange
            _fakeHttpMessageHandler.SetResponseContent("{\"state\": " + (int)AGVClient.AGVState.Charging + "}"); // Set the AGV state to Charging

            // Act
            await _agvClient.GetStatus(); // Update the AGVClient's internal state to Charging

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => _agvClient.ExecuteProgram());
        }

        [Test]
        public async Task CheckIsIdle_ReturnsTrue_WhenStateIsIdle()
        {
            _fakeHttpMessageHandler.SetResponseContent("{\"state\": " + (int)AGVClient.AGVState.Idle + "}"); // Set the AGV state to Idle
            await _agvClient.GetStatus();
            Assert.IsTrue(_agvClient.CheckIsIdle());
        }

        [Test]
        public async Task CheckIsIdle_ReturnsFalse_WhenStateIsNotIdle()
        {
            _fakeHttpMessageHandler.SetResponseContent("{\"state\": " + (int)AGVClient.AGVState.Executing + "}"); // Set the AGV state to Executing
            await _agvClient.GetStatus();
            Assert.IsFalse(_agvClient.CheckIsIdle());
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private string _responseContent = string.Empty;
        private HttpStatusCode _statusCode = HttpStatusCode.OK;
        public HttpRequestMessage? LastRequest { get; private set; }

        // Method to set the response content for the fake HTTP message handler.
        public void SetResponseContent(string responseContent)
        {
            _responseContent = responseContent;
        }

        // Method to set the response content and status code for the fake HTTP message handler.
        public void SetResponse(HttpStatusCode statusCode, string responseContent)
        {
            _responseContent = responseContent;
            _statusCode = statusCode;
        }

        // Method to intercept and mock the HTTP requests for unit testing.
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent)
            };
            return Task.FromResult(response);
        }
    }
}
