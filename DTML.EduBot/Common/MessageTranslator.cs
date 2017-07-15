using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace DTML.EduBot.Common
{
    public static class MessageTranslator
    {
        /// <summary>
        /// Microsoft translator API key
        /// </summary>
        private static string TranslatorApiKey = "60d3de534d1a42ab824521d18b98fe0a ";

        /// <summary>
        /// Language to which we want the input to be converted to
        /// </summary>
        private static string inputTextLang;

        /// <summary>
        /// Translate text to english and send forward
        /// </summary>
        /// <param name="inputText">input text to be converted</param>
        /// <param name="inputLang">Language to which the inputText should be translated to </param>
        /// <returns>translated string</returns>
        public static async Task<string> TranslateTextAsync(string inputText,  string inputLang = null) 
        {
            var accessToken = await MessageTranslator.GetAuthenticationTokenAsync(TranslatorApiKey);
            if (inputLang == null)
            {
                inputLang = inputTextLang;
            }

            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={inputLang}&contentType=text/plain";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(System.Configuration.ConfigurationManager.AppSettings["translatorEndpoint"] + query);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                var translatedText = XElement.Parse(result).Value;
                return translatedText;
            }
        }

        /// <summary>
        /// Identify language for input text
        /// </summary>
        /// <param name="inputText">text whose language should be detected</param>
        /// <returns></returns>
        public static async Task IdentifyLangAsync(string inputText)
        {
            var accessToken = await MessageTranslator.GetAuthenticationTokenAsync(TranslatorApiKey);
            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&contentType=text/plain";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(System.Configuration.ConfigurationManager.AppSettings["detectLanguageEndpoint"] + query);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return;

                var translatedText = XElement.Parse(result).Value;
                inputTextLang = translatedText;
            }

        }

        /// <summary>
        /// Get authentication token for translator API
        /// </summary>
        /// <param name="key">OcpApimSubscriptionKey</param>
        /// <returns></returns>
        public static async Task<string> GetAuthenticationTokenAsync(string key)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                var response = await client.PostAsync(System.Configuration.ConfigurationManager.AppSettings["tokenEndpoint"], null);
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
        }
    }
}