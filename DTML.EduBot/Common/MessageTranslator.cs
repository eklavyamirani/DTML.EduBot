namespace DTML.EduBot.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml.Linq;

    public class MessageTranslator
    {
        /// <summary>
        /// Get authentication token for translator API
        /// </summary>
        /// <param name="key">OcpApimSubscriptionKey</param>
        /// <returns>token if the request is successful null otherwise</returns>
        private static async Task<string> GetAuthenticationTokenAsync(string key)
        {
            using (var client = new HttpClient())
            {
                string token = null;
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                var response = await client.PostAsync(System.Configuration.ConfigurationManager.AppSettings["tokenEndpoint"], null);
                if (response.IsSuccessStatusCode)
                {
                    token = await response.Content.ReadAsStringAsync();
                }
                return token;
            }
        }

        /// <summary>
        /// Identify language for input text
        /// </summary>
        /// <param name="inputText">text whose language should be detected</param>
        /// <returns></returns>
        public static async Task<string> IdentifyLangAsync(string inputText)
        {
            string inputTextLang = "en";

            if (String.IsNullOrWhiteSpace(inputText))
            {
                return inputTextLang;
            }

            var accessToken = await MessageTranslator.GetAuthenticationTokenAsync(ConfigurationManager.AppSettings["TranslatorApiKey"]);
            if (accessToken == null)
            {
                return inputTextLang;
            }

            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&contentType=text/plain";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(ConfigurationManager.AppSettings["detectLanguageEndpoint"] + query);
                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        var translatedText = XElement.Parse(result).Value;
                        inputTextLang = translatedText;
                    }
                }
                return inputTextLang;
            }
        }

        /// <summary>
        /// Translate text to english and send forward
        /// </summary>
        /// <param name="inputText">input text to be converted</param>
        /// <param name="inputLang">Language to which the inputText should be translated to </param>
        /// <returns>translated string</returns>
        public static async Task<string> TranslateTextAsync(string inputText, string inputLang = "en")
        {
            if (String.IsNullOrWhiteSpace(inputText))
            {
                return inputText;
            }

            var accessToken = await MessageTranslator.GetAuthenticationTokenAsync(ConfigurationManager.AppSettings["TranslatorApiKey"]);
            if(accessToken==null)
            {
                return inputText;
            }

            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={inputLang}&contentType=text/plain";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(System.Configuration.ConfigurationManager.AppSettings["translatorEndpoint"] + query);
                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        var translatedText = XElement.Parse(result).Value;
                        return translatedText;
                    }
                }
                return inputText;
            }
        }

    }
}