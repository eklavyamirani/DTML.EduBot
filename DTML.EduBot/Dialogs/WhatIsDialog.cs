namespace DTML.EduBot.Dialogs
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis.Models;
    using DTML.EduBot.Constants;
    using DTML.EduBot.Common;
    using DTML.EduBot.Extensions;
    using System;
    using AdaptiveCards;
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;
    using System.Threading;

    public partial class ChitChatDialog : QnaLuisDialog<object>
    {

        [LuisIntent("WhatIs")]
        public async Task HandleWhatIsIntent(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.Name, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync($"My name is {BotPersonality.BotName}");
            }
            else
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.encyclopediaOrganization, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync($"I think it is a company...");
            }
            else
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.Color, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync($"My favorite color is blue. Blue is the color of light between violet and green on the visible spectrum... It is chosen by almost half of both men and women as their favourite colour.");
            }
            else
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.Time, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync($"Right now is {DateTime.Now.ToShortTimeString()} at Seattle, WA");
            }
            else
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.Date, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                var date = DateTime.Now.ToLongDateString();
                await context.PostTranslatedAsync($"Oh yeah, let me check my digital calendar. Today is... {date}");
            }
            else
            if (result.Entities.Any(e => e.Type.Equals(BotEntities.PlaceLive, System.StringComparison.CurrentCultureIgnoreCase)))
            {
                await context.PostTranslatedAsync($"Well, I live on DTML.org website. Please visit me sometime.");
            }
            else
            {
                await context.PostTranslatedAsync(BotPersonality.GetRandomGenericResponse());
            }
        }

        private async Task<bool> PostAdaptiveCard(IDialogContext context, string url, string title, string imageURL, string text)
        {
            List<ActionBase> answerOptions = new List<ActionBase>();
            ActionBase action = new OpenUrlAction() { Title = title, Url = url };
            answerOptions.Add(action);

            AdaptiveCard adaptiveCard = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {                   
                    new TextBlock()
                    {
                        Text = text,
                        Wrap = true
                    },
                    new Image()
                    {
                        Size = ImageSize.Large,
                        Url  = imageURL,
                        HorizontalAlignment= HorizontalAlignment.Center
                    },
                },
                Actions = answerOptions
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply, CancellationToken.None);
            return true;
        }
    }
}