using truthfulls.com.Services;
using truthfulls.com.Controllers;
using Microsoft.AspNetCore.Http;

using truthfulls.com.Models;


namespace TruthfullsTests
{
    public class StockDataRetrievalTests
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

        //Stock Data Retrieval Tests
        [Test]
        public async Task RetreivingTickersShouldReturnNonNull()
        {
            var tickers = await this._stockdataservice.GetTickerDataAsync();
            Assert.That(tickers != null);
        }
        [Test]
        public async Task RetreivingExistingStockDataShouldReturnNotNull()
        {
            List<Price>? prices = await this._stockdataservice.TryGetPriceDataAsync("AMD", "2010-01-01", "2020-10-10",TimePeriod.Daily);
            Assert.That(prices != null);
        }
        [Test]
        public async Task RetreivingNonExistingStockShouldReturnNull()
        {
            List<Price>? prices = await this._stockdataservice.TryGetPriceDataAsync("SPYBOOGIE", "2010-01-01", "2020-10-10", TimePeriod.Daily);
            Assert.That(prices == null);
        }
        [Test]
        public async Task RetreivingStockDatesOutOfRangeShouldReturnNull()
        {
            List<Price>? prices = await this._stockdataservice.TryGetPriceDataAsync("AMD", "1000-01-01", "1500-03-14", TimePeriod.Daily);
            Assert.That(prices == null);
        }
       
    }

}