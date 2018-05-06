namespace DTML.EduBot.Dialogs
{
    using AdaptiveCards;
    using DTML.EduBot.Common.Interfaces;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Serializable]
    public partial class AuthenticateDialog : IDialog<Object>
    {
        ILogger _logger;

        public AuthenticateDialog(ILogger logger)
        {
            _logger = logger;
        }


        public async Task StartAsync(IDialogContext context)
        {
            AdaptiveCard adaptiveCard = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                     new TextBlock()
                    {
                        Text = "Please login or signup",
                        Weight=  TextWeight.Bolder
                    },

                    new TextBlock()
                    {
                        Wrap = true,
                        MaxLines = 5,
                        Text = "A user account is required to chat with the bot. Creating an account is quick and easy. Once you have created an account, you can sign in at any time.",
                    }
                },

                Actions = new List<ActionBase>()
                {
                    new OpenUrlAction()
                    {
                        Url = "https://dtml.org/Account/Login?ReturnUrl=https://dtml.org/Home/Edubot",
                        Title  = "Login"
                    },
                    new OpenUrlAction()
                    {
                        Url = "https://dtml.org/Registration/Student",
                        Title  = "Signup"
                    },
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply);
            return;
        }
    }
}