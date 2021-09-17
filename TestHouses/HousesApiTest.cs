using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHouses
{
    [TestClass]
    public class HousesApiTest
    {
        const string BaseUrl = "https://www.anapioficeandfire.com/api/houses";

        [TestCategory("FilterTest")]
        [DataRow("Dorne" , true)]
        [DataTestMethod]
        public void TestRegionAndHaswordsFilter(string region, bool haswords)
        {
            // Get all houses
            ApiObjects.House[] allHouses = Helpers.ApiRequester.GetJsonObject<ApiObjects.House[]>(BaseUrl);
            Assert.IsNotNull(allHouses);
            Assert.IsTrue(allHouses.Any(), "Received empty list of houses.");

            string filterUrl = Helpers.ApiRequester.CreateFilterUrl(
                BaseUrl, new[] { string.Format("region={0}", region), string.Format("haswords={0}", haswords.ToString().ToLower())});

            // Get houses using filters
            ApiObjects.House[] filteredHouses = Helpers.ApiRequester.GetJsonObject<ApiObjects.House[]>(filterUrl);
            Assert.IsNotNull(filteredHouses);

            // Check all filtered houses have expected region=Dorne и haswords=true
            Assert.IsTrue(filteredHouses.All(_ => string.Equals(_.region, region)), "Some houses received using filter have invalid region.");
            Assert.IsTrue(filteredHouses.All(_ => !string.IsNullOrWhiteSpace(_.words)), "Some houses received using filter have no words.");

            // Check number of houses received using filter is equal to number of such houses in all houses
            ApiObjects.House[] selectedFromAllHouses = allHouses.Where(_ => string.Equals(_.region, region) && haswords == !string.IsNullOrWhiteSpace(_.words)).ToArray();
            Assert.IsTrue(selectedFromAllHouses.Count() == filteredHouses.Count(), "Initial collection of houses contains less houses with parameters given in the filter.");
        }

        [TestCategory("NegativeTest")]
        [ExpectedException(typeof(System.Net.WebException), "Test method should throw System.Net.WebException.")]
        [DataRow("http://notexistingurl.notexistingdomain.notexistingzone")]
        [DataTestMethod]
        public void TestInvalidUrl(string url)
        {
            // This method should throw WebException
            Helpers.ApiRequester.GetJsonObject<ApiObjects.House[]>(url);
        }
    }
}
