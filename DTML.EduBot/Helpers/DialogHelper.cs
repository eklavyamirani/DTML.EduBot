using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace DTML.EduBot.Helpers
{
    public static class DialogHelper
    {
        public static async Task SendMessage(IDialogContext context, string query, string message)
        {
            string language = await DialogHelper.GetLanguage(query);

            if (language.ToLower().Equals("es"))
            {
                string translated = await DialogHelper.GetTranslation(message);
                await context.PostAsync(translated);
            }
            else
            {
                await context.PostAsync(message);
            }
        }

        public static async Task<string> GetLanguage(string message)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(message);

            var uri = "https://dtml-staging.azurewebsites.net/api/translationservice/getlanguage?text=" + queryString;

            HttpResponseMessage response;

            // Request body
            response = await client.GetAsync(uri);

            return response.Content.ReadAsStringAsync().Result;
        }

        public static async Task<string> GetTranslation(string message)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(message);

            var uri = "https://dtml-staging.azurewebsites.net/api/translationservice/gettranslation?text=" + queryString;

            HttpResponseMessage response;

            // Request body
            response = await client.GetAsync(uri);

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}