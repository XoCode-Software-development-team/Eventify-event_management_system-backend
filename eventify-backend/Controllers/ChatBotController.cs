using System;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly SessionsClient _sessionsClient;
        private readonly string _projectId;

        public ChatBotController()
        {
            // Path to the service account key file
            string credentialPath = "F:\\New folder\\Eventify-event_management_system-backend\\eventify-backend\\appsettings.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            _projectId = "eventifychatbot";  // Replace with your Dialogflow project ID
            _sessionsClient = SessionsClient.Create();
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            var response = await DetectIntentFromText(request.Message);
            return Ok(new ChatResponse { Reply = response });
        }

        private async Task<string> DetectIntentFromText(string text)
        {
            var sessionId = Guid.NewGuid().ToString(); // Unique session ID for each conversation
            var sessionName = new SessionName(_projectId, sessionId);
            var queryInput = new QueryInput
            {
                Text = new TextInput
                {
                    Text = text,
                    LanguageCode = "en"  // Replace with your desired language
                }
            };

            var response = await _sessionsClient.DetectIntentAsync(sessionName, queryInput);
            return response.QueryResult.FulfillmentText;
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }

    public class ChatResponse
    {
        public string Reply { get; set; }
    }
}
