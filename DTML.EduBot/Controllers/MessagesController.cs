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
    using Autofac;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.IdentityModel.Protocols;
    using System.Configuration;
    using DTML.EduBot.Common.Interfaces;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly RootDialog _rootDialog;
        private readonly ILogger _logger;

        public MessagesController(RootDialog rootDialog, ILogger logger)
        {
            _rootDialog = rootDialog;
            _logger = logger;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
                if (activity.Type == ActivityTypes.Message)
                {
                    using (ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl)))
                    {
                        Activity typing = activity.CreateReply();
                        typing.Type = ActivityTypes.Typing;
                        await connector.Conversations.ReplyToActivityAsync(typing);
                    }

                await Conversation.SendAsync(activity, () =>
                               new ExceptionHandlerDialog<object>(
                                  _rootDialog,
                                  _logger));
                }
                else
                {
                    await HandleSystemMessageAsync(activity);
                }            

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
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