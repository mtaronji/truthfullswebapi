using Microsoft.AspNetCore.Http;
using truthfulls.com.Controllers;
using truthfulls.com.Services;

namespace TruthfullsTests
{
    public class StockAPITests
    {
        private readonly string ConnectionString = "Data Source=unittest.db;";
        private StockDataRetrievalService _stockdataservice;
        private StockController _stockcontroller;
        [SetUp]
        public void Setup()
        {
            this._stockdataservice = new StockDataRetrievalService(this.ConnectionString);
            this._stockcontroller = new StockController(this._stockdataservice);
            this._stockcontroller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Test]
        public async Task RetreivingPricesOfNonExistentTickerShouldReturn404ResponseAndANullObject()
        {
            var result = await this._stockcontroller.TryGetDailyPricesAsync("0=SPYBOOGIE&datebegin=2022-09-07&dateend=2023-09-07");
            Assert.That(this._stockcontroller.Response.StatusCode == 404);
            Assert.That(result.Value, Is.Null);
        }
        [Test]
        public async Task RetreivingPricesOfRealTickerWithCorrectRangeShouldReturn200ResponseAndANonNullObject()
        {
            var result = await this._stockcontroller.TryGetDailyPricesAsync("0=SPY&datebegin=2022-09-07&dateend=2023-09-07");
            Assert.That(this._stockcontroller.Response.StatusCode == 200);
            Assert.That(result.Value, Is.Not.Null);
        }

        [Test]
        public async Task RetreivingPricesOfRealTickerWithIncorrectRangeShouldReturn404ResponseAndANullObject()
        {
            var result = await this._stockcontroller.TryGetDailyPricesAsync("0=SPY&datebegin=1500-09-07&dateend=1600-09-07");
            Assert.That(this._stockcontroller.Response.StatusCode == 404);
            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public async Task RetreivingTickersShouldBeNonNullAndReturn200()
        {
            var result = await this._stockcontroller.GetAllTickers();
            Assert.That(this._stockcontroller.Response.StatusCode == 200);
            Assert.That(result.Value, Is.Not.Null);
        }
    } 
}
