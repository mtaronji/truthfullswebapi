using Microsoft.AspNetCore.Mvc;
using truthfulls.com.StockModels;
using truthfulls.com.Data;
using truthfulls.com.Services;

namespace truthfulls.com.Controllers
{

    //return various stock data
    [ApiController]
    public class StockController : ControllerBase
    {         
        //private MarketContext _marketContext;
        private UtilityService _utility;
        public StockController(UtilityService Utility)
        {
            this._utility = Utility;
          
        }

  

        [HttpGet]
        [Route("[controller]/getweeklyprices/{querystring}")]
        [Produces("application/json")]
        public async Task<ActionResult<Dictionary<string, List<PriceVM>>?>> TryGetWeeklyPricesAsync(string querystring)
        {
            return Ok();
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
     

    }
}
