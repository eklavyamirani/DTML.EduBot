﻿namespace DTML.EduBot
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
    using DTML.EduBot.Helpers;

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
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    var message = activity as IMessageActivity;
                    await Conversation.SendAsync(activity, () => _rootDialog);
                }
                else
                {
                    await HandleSystemMessageAsync(activity);
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //e.Data.Add("id", activity.From.Id);
                //throw e;
            }
            return null;
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
                await ConversationHelper.LoadConversation(message);
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