namespace DTML.EduBot
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using DTML.EduBot.Common;
    using DTML.EduBot.Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using DTML.EduBot.UserData;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly RootDialog _rootDialog;

        public MessagesController(RootDialog rootDialog)
        {
            _rootDialog = rootDialog;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
                if (activity.Type == ActivityTypes.Message)
                {
                    await Conversation.SendAsync(activity, () =>
                       new ExceptionHandlerDialog<object>(
                          _rootDialog,
                          displayException: true));
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
            if (message == null) return null;

            try
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
                            Activity typing = message.CreateReply();
                            typing.Type = ActivityTypes.Typing;
                            await connector.Conversations.ReplyToActivityAsync(typing);

                            Activity reply = message.CreateReply(BotPersonality.BotSelfIntroductionStart);
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
            }
            catch
            { }

            return null;
        }
    }
}