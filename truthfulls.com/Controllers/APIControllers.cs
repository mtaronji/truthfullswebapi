using Microsoft.AspNetCore.Mvc;
using truthfulls.com.Services;
using truthfulls.com.Models;
using System.Diagnostics;

namespace truthfulls.com.Controllers
{

    //return various stock data
    [ApiController]
    public class StockController : ControllerBase
    { 

        StockDataRetrievalService dataRetrievalService;
        public StockController(StockDataRetrievalService dataRetrievalService)
        {
            this.dataRetrievalService = dataRetrievalService;
        }


        [HttpGet]
        [Route("[controller]/{querystring}/getdailyprices")]
        [Produces("application/json")]
        public async Task<ActionResult<Dictionary<string, List<PriceVM>>?>> TryGetDailyPricesAsync(string querystring)
        {
            TimePeriod period = TimePeriod.Daily;
            var q = querystring;
            var dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(q);
            string datebegin; string dateend;
            string[] Selectedtickers = { };
            Dictionary<string, List<PriceVM>>? allprices = new();

            try
            {
                datebegin = dict["datebegin"].ToString(); dict.Remove("datebegin");
                dateend = dict["dateend"].ToString(); dict.Remove("dateend");
                Selectedtickers = dict.Values.Select(x => x.ToString()).ToArray();
            }
            catch (KeyNotFoundException e)
            {
                //log exception here
                Console.WriteLine(e.Message);
                allprices = null; Response.StatusCode = 404;
                return allprices;

            }

            foreach(var ticker in Selectedtickers)
            {
                var prices = await this.dataRetrievalService.TryGetPriceDataAsync(ticker, datebegin, dateend, period);
                if (prices == null) { Response.StatusCode = 404; allprices = null; return allprices; }
                allprices[ticker] = Price.ToPriceVM(prices);
            }        

            return allprices;
        }

        [HttpGet]
        [Route("[controller]/{querystring}/getweeklyprices")]
        [Produces("application/json")]
        public async Task<ActionResult<Dictionary<string, List<PriceVM>>?>> TryGetWeeklyPricesAsync(string querystring)
        {
            //API returns the weekstart date as the Date value in the price model.
            TimePeriod period = TimePeriod.Weekly;
            var q = querystring;
            var dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(q);
            var datebegin = dict["datebegin"].ToString(); dict.Remove("datebegin");
            var dateend = dict["dateend"].ToString(); dict.Remove("dateend");
            var Selectedtickers = dict.Values.Select(x => x.ToString()).ToArray();

            Dictionary<string, List<PriceVM>>? allprices = new();
            foreach (var ticker in Selectedtickers)
            {
                var prices = await this.dataRetrievalService.TryGetPriceDataAsync(ticker, datebegin, dateend, period);
                if (prices == null) { Response.StatusCode = 404; allprices = null; return allprices; }
                allprices[ticker] = Price.ToPriceVM(prices);
            }

            return allprices;
        }


        [HttpGet]
        [Route("/[controller]/{ticker:alpha}/{datebegin}/{dateend}/getdailygains")]
        public async Task<IActionResult> GetDailyGains(string ticker, string datebegin, string dateend)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("/[controller]/{ticker:alpha}/{datebegin}/{dateend}/getweeklygains")]
        public async Task<IActionResult> GetWeeklyGains(string ticker, string datebegin, string dateend) 
        {
            throw new NotImplementedException();
        }

        //return the simple moving average of the duration
        [HttpGet]
        [Route("/[controller]/{name:alpha}/sma/{duration}")]
        public async Task<IActionResult> GetSMA(string name, int duration)
        {
            throw new NotImplementedException();
        }


        //return the exponential moving average of the date specified
        [HttpGet]
        [Route("/[controller]/{name:alpha}/ema/{duration}")]
        public async Task<IActionResult> GetEMA(string name, int duration)
        {
            throw new NotImplementedException();
        }


        [HttpGet]
        [Route("/[controller]/{querystring}")]
        public async Task<IActionResult> GetDailyPrices(string querystring)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("/[controller]/getalltickers")]
        [Produces("application/json")]
        public async Task<ActionResult<List<string>?>> GetAllTickers()
        {
            var tickers = await dataRetrievalService.GetTickerDataAsync();
            if(tickers == null){ Response.StatusCode = 404; return tickers; }

            Response.StatusCode = 200;
            return tickers;

        }


    }
}
