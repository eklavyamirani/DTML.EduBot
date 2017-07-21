namespace DTML.EduBot
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Autofac;
    using DTML.EduBot.Common;
    using DTML.EduBot.Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                using (var scope = WebApiApplication.FindContainer().BeginLifetimeScope())
                {
                    await Conversation.SendAsync(activity, () => scope.Resolve<RootDialog>());
                }
            }
            else
            {
                await HandleSystemMessageAsync(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message)
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

                // Ensure the auto messages don't fire in group conversations and only when bot gets added to the conversation.
                var isGroupConversation = message.Conversation.IsGroup.HasValue && message.Conversation.IsGroup.Value;
                if (!isGroupConversation && message.MembersAdded.Any(member => member.Name == message.Recipient.Name))
                {
                    using (ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl)))
                    {
                        // TODO: start the root activity here.
                        Activity reply = message.CreateReply($"Hi I am {BotPersonality.BotName}.");
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                }
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