using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHouses.Helpers
{
    class UrlHelper
    {
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
    }
}
