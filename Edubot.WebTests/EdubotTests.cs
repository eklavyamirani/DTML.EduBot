using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net.WebSockets;
using System.Threading;
using System.Linq;
using System.Text;
using Microsoft.Bot.Connector.DirectLine;
using System.Collections.Generic;
using DTML.EduBot.Common;

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

        [TestMethod]
        public async Task BotSendsIntroductionOnStartup()
        {
            var fakeSender = new ChannelAccount
            {
                Id = "0000-0000-0000-0000",
                Name = "Unit Test"
            };

            var client = new DirectLineClient(_token);
            var updateActivity = new Activity
            {
                Type = ActivityTypes.ConversationUpdate
            };

            updateActivity.MembersAdded = new List<ChannelAccount> {
                fakeSender
            };

            updateActivity.From = fakeSender;

            var conversation = await client.Conversations.StartConversationAsync();
            var response = await client.Conversations.PostActivityAsync(conversation.ConversationId, updateActivity);
            var activitySet = await client.Conversations.GetActivitiesAsync(conversation.ConversationId, null);
            var receivedActivity = activitySet.Activities
                .FirstOrDefault((activity) => activity.Conversation.Id == conversation.ConversationId 
                                              && activity.From.Id != fakeSender.Id);

            Assert.IsNotNull(receivedActivity);
            Assert.AreEqual(BotPersonality.BotSelfIntroduction, receivedActivity.Text);
        }
    }
}
