using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xNet;

namespace DnevnikRuAPI
{
    public class DnevnikRuAPI
    {
        public static string Access_Token = "";

        public string Authorize(string login, string password)
        {
            var content = "";
            using (var request = new HttpRequest())
            {
                var stringContent = new StringContent(
                    "{  \"agreeTerms\": true,  \"clientId\": \"1d7bd105-4cd1-4f6c-9ecc-394e400b53bd\", " +
                    $" \"clientSecret\": \"5dcb5237-b5d3-406b-8fee-4441c3a66c99\",  \"password\": \"{password}\",  " +
                    "\"scope\": \"Schools,Relatives,EduGroups,Lessons,marks,EduWorks,EducationalInfo,Avatar\", " +
                    $" \"username\": \"{login}\"}}",
                    Encoding.UTF8);

                stringContent.ContentType = "application/json";

                var response = request.Post("https://api.dnevnik.ru/mobile/v5/authorizations/bycredentials",
                    stringContent);

                content = response.ToString();
            }

            var at = JObject.Parse(content)["credentials"]["accessToken"].ToString();

            Access_Token = at;
            return at;
        }

        public static class Lessons
        {
            public static Lesson GetLesson(long id)
            {
                var content = "";
                using (var request = new HttpRequest())
                {
                    request.AddHeader("Access-Token", Access_Token);

                    var response =
                        request.Get(
                            $"https://api.dnevnik.ru/v2.0/lessons/{id}");

                    content = response.ToString();
                }

                return JsonConvert.DeserializeObject<Lesson>(content);
            }

            public class Lesson
            {
                public long id { get; set; }
                public string id_str { get; set; }
                public string title { get; set; }
                public DateTime date { get; set; }
                public int number { get; set; }
                public Subject subject { get; set; }
                public long group { get; set; }
                public string status { get; set; }
                public object resultPlaceId { get; set; }
                public List<Work> works { get; set; }
                public List<long> teachers { get; set; }
                public List<string> teachers_str { get; set; }

                public class Subject
                {
                    public long id { get; set; }
                    public string id_str { get; set; }
                    public string name { get; set; }
                    public string knowledgeArea { get; set; }
                    public object fgosSubjectId { get; set; }
                }

                public class Work
                {
                    public object id { get; set; }
                    public string id_str { get; set; }
                    public string type { get; set; }
                    public object workType { get; set; }
                    public string markType { get; set; }
                    public int markCount { get; set; }
                    public object lesson { get; set; }
                    public string lesson_str { get; set; }
                    public bool displayInJournal { get; set; }
                    public string status { get; set; }
                    public object eduGroup { get; set; }
                    public string eduGroup_str { get; set; }
                    public List<object> tasks { get; set; }
                    public string text { get; set; }
                    public int periodNumber { get; set; }
                    public string periodType { get; set; }
                    public object subjectId { get; set; }
                    public bool isImportant { get; set; }
                    public DateTime targetDate { get; set; }
                    public object sentDate { get; set; }
                    public long createdBy { get; set; }
                    public object files { get; set; }
                    public object oneDriveLinks { get; set; }
                }
            }
        }

        public static class Marks
        {
            public static List<Mark> GetMarkByLesson(long lessonId)
            {
                var content = "";
                using (var request = new HttpRequest())
                {
                    request.AddHeader("Access-Token", Access_Token);

                    var response =
                        request.Get(
                            $"https://api.dnevnik.ru/v2.0/lessons/{lessonId}/marks");

                    content = response.ToString();
                }

                return JsonConvert.DeserializeObject<List<Mark>>(content);
            }

            public class Mark
            {
                public long id { get; set; }
                public string id_str { get; set; }
                public string type { get; set; }
                public string value { get; set; }
                public string textValue { get; set; }
                public long person { get; set; }
                public string person_str { get; set; }
                public long work { get; set; }
                public string work_str { get; set; }
                public long lesson { get; set; }
                public string lesson_str { get; set; }
                public int number { get; set; }
                public DateTime date { get; set; }
                public long workType { get; set; }
                public string mood { get; set; }
                public bool use_avg_calc { get; set; }
            }
        }

        public static class Homeworks
        {
            public static List<Homework> GetHomeworksByDate(string startDate, string endDate,
                long schoolId = 1000000061442)
            {
                var homeworks = new List<Homework>();

                var content = "";
                using (var request = new HttpRequest())
                {
                    request.AddHeader("Access-Token", Access_Token);

                    var response =
                        request.Get(
                            $"https://api.dnevnik.ru/v2.0/users/me/school/{schoolId}/homeworks?startDate={startDate}&endDate={endDate}");

                    content = response.ToString();
                }

                foreach (var VARIABLE in JObject.Parse(content)["works"])
                    homeworks.Add(JsonConvert.DeserializeObject<Homework>(VARIABLE.ToString()));

                return homeworks;
            }

            public class Homework
            {
                public long id { get; set; }
                public string id_str { get; set; }
                public string type { get; set; }
                public long workType { get; set; }
                public string markType { get; set; }
                public int markCount { get; set; }
                public long lesson { get; set; }
                public string lesson_str { get; set; }
                public bool displayInJournal { get; set; }
                public string status { get; set; }
                public long eduGroup { get; set; }
                public string eduGroup_str { get; set; }
                public string text { get; set; }
                public int periodNumber { get; set; }
                public string periodType { get; set; }
                public long subjectId { get; set; }
                public bool isImportant { get; set; }
                public DateTime targetDate { get; set; }
                public object sentDate { get; set; }
                public long createdBy { get; set; }
                public List<object> files { get; set; }
                public List<object> oneDriveLinks { get; set; }
                public string lessonName => Lessons.GetLesson(lesson).subject.name;
            }
        }
    }
}