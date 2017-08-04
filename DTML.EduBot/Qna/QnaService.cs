namespace DTML.EduBot.Qna
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DTML.EduBot.Extensions;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    [Serializable]
    public class QnaService
    {
        private readonly IQnaModel model;
        private readonly Uri serviceUri;

        public QnaService(IQnaModel model)
        {
            Set.FieldNotNull(model, nameof(model), out this.model);
            this.serviceUri = BuildUri();
        }

        private Uri BuildUri()
        {
            return model.BuildUri();
        }

        public async Task<IQnaResult> QueryAsync(string question, CancellationToken token)
        {
            var request = new QnaRequest(question);
            string jsonResponse;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", model.SubscriptionKey);
                var content = new StringContent(JsonConvert.SerializeObject(new QnaRequest(question)), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(serviceUri.AbsoluteUri, content, token))
                {
                    response.EnsureSuccessStatusCode();
                    jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            try
            {
                var results = JsonConvert.DeserializeObject<QnaResultCollection>(jsonResponse).Answers;
                var topScore = results.Max(result => result.Score);
                return results.FirstOrDefault(result => result.Score == topScore);
            }
            catch (Exception)
            {
                // TODO: log exception
                return new QnaResult
                {
                    Answer = string.Empty,
                    Score = -1,
                    Questions = new List<string>
                    {
                        question
                    }
                };
            }
        }
    }
}