using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Net.Http;
using System.Web;
using DTML.EduBot.Helpers;

namespace DTML.EduBot.Dialogs
{
    public partial class WhatIsDialog : LuisDialog<object>
    {
        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string message = "Hello, its nice to meet you!";
            string query = result.Query.ToString();
            await DialogHelper.SendMessage(context, query, message);
        }
    }
}