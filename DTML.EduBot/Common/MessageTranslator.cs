namespace DTML.EduBot.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml.Linq;
    using Google.Cloud.Translation.V2;
    using Google.Apis.Auth.OAuth2;

    public class MessageTranslator
    {
        public static readonly string DEFAULT_LANGUAGE = "en";
        private static TranslationClient client = TranslationClient.Create();

        /// <summary>
        /// Identify language for input text
        /// </summary>
        /// <param name="inputText">text whose language should be detected</param>
        /// <returns></returns>
        public static async Task<string> IdentifyLangAsync(string inputText)
        {
            string inputTextLang = DEFAULT_LANGUAGE;

            if (String.IsNullOrWhiteSpace(inputText))
            {
                return inputTextLang;
            }

            var response = await client.DetectLanguageAsync(inputText);
            return response.Language;
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

            var message = await client.TranslateTextAsync(inputText, inputLang);
            return message.TranslatedText;
        }

        public static async Task<ICollection<string>> TranslatedChoices(IEnumerable<string> choices, string languageToTranslate)
        {
            ICollection<string> translatedChoices = new Collection<string>();

            foreach (string choice in choices)
            {
                string translatedChoice = await MessageTranslator.TranslateTextAsync(choice, languageToTranslate);
                translatedChoices.Add(translatedChoice);
            }

            return translatedChoices;
        }
    }
}