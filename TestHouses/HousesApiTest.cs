using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TestHouses.Helpers;
using TestHouses.Models;

namespace TestHouses
{
    [TestFixture]
    public class HousesApiTest
    {
        private const string BaseUrl = "https://www.anapioficeandfire.com/api/houses";

        [Test]
        [Description("Check test houses response filtered by region and haswords")]
        [TestCase("Dorne", true)]
        public void TestRegionAndHaswordsFilter(string region, bool haswords)
        {
            // arrange
            var filterUrl = $"{BaseUrl}?region={region}&haswords={haswords.ToString().ToLower()}";

            // act
            var allHouses = ApiClient.GetObject<House[]>(BaseUrl);
            allHouses.Should().NotBeNullOrEmpty();
       
            var filteredHouses = ApiClient.GetObject<House[]>(filterUrl);
            filteredHouses.Should().NotBeNullOrEmpty();

            // assert
            // Check all filtered houses region
            filteredHouses.Should().AllSatisfy(
                _ => string.Equals(_.region, region),
                "Some houses received using filter have unexpected region.");

            // Check all filtered houses words
            filteredHouses.Should().AllSatisfy(
                _ => haswords.Equals(!string.IsNullOrEmpty(_.words)),
                "Some houses received using filter have unexpected words property.");

            // Check number of houses received using filter is equal to number of such houses in all houses
            allHouses.Where(
                _ => string.Equals(_.region, region)
                && haswords == !string.IsNullOrWhiteSpace(_.words))
                .Count()
                .Should().Be(filteredHouses.Count(),
                "Initial collection of houses contains different number of houses than received using the filter.");
        }

        [Test]
        [Description("Negative test - URL does not exist")]
        public void TestInvalidUrl()
        {
            const string NotExistingUrl = "http://notexistingurl.notexistingurl.notexistingurl";

            // arrange
            Action getHousesAction = () => ApiClient.GetObject<House[]>(NotExistingUrl);

            // act & assert
            getHousesAction.Should().Throw<System.Net.WebException>("Test method should throw System.Net.WebException.");
        }
    }
}
