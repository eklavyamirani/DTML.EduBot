using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Autofac;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DTML.EduBot.Models;
using System.Threading.Tasks;
using DTML.EduBot.Common;
using System;

namespace DTML.EduBot.Helpers
{
    public static class ConversationHelper
    {
        public static async Task LogMessage(IMessageActivity botMessage, IMessageActivity userMessage, bool isBot = false)
        {
            try
            {
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, userMessage))
                {
                    var botDataStore = scope.Resolve<IBotDataStore<BotData>>();
                    
                    var key = new AddressKey()
                    {
                        BotId = isBot ? userMessage.Recipient.Id : userMessage.From.Id,
                        ChannelId = userMessage.ChannelId,
                        UserId = isBot ? userMessage.From.Id : userMessage.Recipient.Id,
                        ConversationId = userMessage.Conversation.Id,
                        ServiceUrl = userMessage.ServiceUrl
                    };

                    var botUserData = await botDataStore.LoadAsync(key, BotStoreType.BotUserData, CancellationToken.None);

                    List<Activity> messages = null;

                    if (botUserData.Data != null)
                    {
                        messages = botUserData.GetProperty<List<Activity>>("BotMessages");
                    }

                    if (messages == null)
                    {
                        messages = new List<Activity>();
                    }

                    if (!string.IsNullOrEmpty(userMessage.Text))
                    {
                        messages.Add(userMessage as Activity);
                    }

                    if (!string.IsNullOrEmpty(botMessage.Text))
                    {
                        messages.Add(botMessage as Activity);
                    }
                    
                    botUserData.SetProperty("BotMessages", messages);

                    await botDataStore.SaveAsync(key, BotStoreType.BotUserData, botUserData, CancellationToken.None);
                    await botDataStore.FlushAsync(key, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await Task.CompletedTask;
        }

        public static async Task LoadConversation(Activity userMessage)
        {
            try
            {
                var isGroupConversation = userMessage.Conversation.IsGroup.HasValue && userMessage.Conversation.IsGroup.Value;
                if (!isGroupConversation && userMessage.MembersAdded.Any(member => member.Name == userMessage.Recipient.Name))
                {
                    using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, userMessage))
                    {
                        var client = scope.Resolve<IConnectorClient>();
                        var botDataStore = scope.Resolve<IBotDataStore<BotData>>();
                        var botMessage = userMessage.CreateReply();

                        var key = new AddressKey()
                        {
                            BotId = userMessage.Recipient.Id,
                            ChannelId = userMessage.ChannelId,
                            UserId = userMessage.From.Id,
                            ConversationId = userMessage.Conversation.Id,
                            ServiceUrl = userMessage.ServiceUrl
                        };

                        var botUserData = await botDataStore.LoadAsync(key, BotStoreType.BotUserData, CancellationToken.None);

                        List<Activity> messages = new List<Activity>();

                        if (botUserData.Data != null)
                        {
                            messages = botUserData.GetProperty<List<Activity>>("BotMessages");
                        }

                        foreach (var botMsg in messages)
                        {
                            client.Conversations.SendToConversation(botMsg, userMessage.Conversation.Id);
                        }

                        botMessage.Text = BotPersonality.BotSelfIntroduction;
                        await LogMessage(botMessage, userMessage, true);
                        client.Conversations.SendToConversation(botMessage, userMessage.Conversation.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await Task.CompletedTask;
        }
    }
}