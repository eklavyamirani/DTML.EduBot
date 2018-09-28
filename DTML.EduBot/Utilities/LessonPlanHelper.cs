using DTML.EduBot.LessonPlan;
using DTML.EduBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DTML.EduBot.Utilities
{
    public class LessonPlanHelper
    {
        public static async Task<T> GetLessonPlanAsync<T>(string path)
        {
            HttpClient client = new HttpClient();
            T lesson = default(T);
            HttpResponseMessage response = client.GetAsync(path).Result;

            if (response.IsSuccessStatusCode)
            {
                lesson = await response.Content.ReadAsAsync<T>();
            }
            return lesson;
        }
    }
}