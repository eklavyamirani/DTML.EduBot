using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace Edubot.WebTests
{
    [TestClass]
    public class EdubotTests
    {
        private HttpClient _client;
        private string _token;
        private const string botUri = "https://directline.botframework.com/v3/directline/conversations";
        private const string BotChatSecretKey = "botchat-secret";

        [TestInitialize]
        public void TestSetup()
        {
            this._client = new HttpClient();
            this._token = ConfigurationManager.AppSettings[BotChatSecretKey];
        }

        [TestMethod]
        public async Task TheBotIsAlive()
        {
            var tokenHeader = $"Bearer {_token}";
            var uri = new Uri(botUri);
            _client.DefaultRequestHeaders.Add("Authorization", tokenHeader);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await _client.PostAsync(uri, null);
            response.EnsureSuccessStatusCode();

            var serializedResponse = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(serializedResponse);
            Assert.IsFalse(string.IsNullOrEmpty(data.Value<string>("conversationId")));
        }
    }
}
