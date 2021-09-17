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

        // Method returns url with filters added from given string array
        public static string CreateFilterUrl(string baseUrl, string[] filterArray)
        {
            // If filter array is empty, return empty string
            if (!filterArray.Any())
            {
                return baseUrl;
            }

            StringBuilder filterBuilder = new StringBuilder();

            for (int i = 0; i < filterArray.Length; i++)
            {
                filterBuilder.Append(filterArray[i]);

                // If not last element, add '&' operator
                if (i < (filterArray.Length - 1))
                {
                    filterBuilder.Append("&");
                }
            }

            return string.Format("{0}?{1}", baseUrl, filterBuilder.ToString());
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
