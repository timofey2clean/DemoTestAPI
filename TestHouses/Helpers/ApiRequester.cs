using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TestHouses.Helpers
{
    class ApiRequester
    {
        public static T GetJsonObject<T>(string url)
        {
            const string method = "GET";

            string responseBody = SendWebRequest(url, method);

            if (!IsValidJson<T>(responseBody))
            {
                throw new Exception("Received Json is invalid.");
            }

            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        private static string SendWebRequest(string url, string method)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(string.Format("Web request failed. Response code: {0}", (int)response.StatusCode));
            }

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        private static bool IsValidJson<T>(string jsonBody)
        {
            jsonBody = jsonBody.Trim();

            try
            {
                if (jsonBody.StartsWith("{") && jsonBody.EndsWith("}"))
                {
                    JToken.Parse(jsonBody);
                }
                else if (jsonBody.StartsWith("[") && jsonBody.EndsWith("]"))
                {
                    JArray.Parse(jsonBody);
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
