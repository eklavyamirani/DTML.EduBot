using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DTML.EduBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// Microsoft translator API key
        /// </summary>
        private string TranslatorApiKey = "60d3de534d1a42ab824521d18b98fe0a ";

        /// <summary>
        /// Language to which we want the input to be converted to
        /// </summary>
        private const string targetLang = "en";

        private string translatorEndpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var userQuery = activity.Text;
                var authenticationToken = await GetAuthenticationToken(TranslatorApiKey);
                var translatedText = await TranslateText(userQuery, authenticationToken);
                if (translatedText != null && authenticationToken != null)
                {
                    activity.Text = translatedText;
                }
                
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        /// <summary>
        /// Translate text to english and send forward
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="accessToken">access token for translator API</param>
        /// <returns></returns>
        private async Task<string> TranslateText(string inputText, string accessToken)
        {
            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={targetLang}&contentType=text/plain";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(this.translatorEndpoint + query);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                var translatedText = XElement.Parse(result).Value;
                return translatedText;
            }
        }

        /// <summary>
        /// Get authentication token for translator API
        /// </summary>
        /// <param name="key">OcpApimSubscriptionKey</param>
        /// <returns></returns>
        static async Task<string> GetAuthenticationToken(string key)
        {
            

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                var response = await client.PostAsync(endpoint, null);
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}