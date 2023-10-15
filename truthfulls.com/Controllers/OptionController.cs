using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truthfulls.com.Models;
using truthfulls.com.Services;

namespace truthfulls.com.Controllers
{
    [ApiController]
    public class OptionController : ControllerBase
    {
        OptionRetrievalService _optionRetrievalService;
        public OptionController(OptionRetrievalService optionRetrievalService) 
        {

            this._optionRetrievalService = optionRetrievalService;
        }

        [HttpGet]
        [Route("[controller]/{querystring}/getoptioncodes")]
        [Produces("application/json")]
        public async Task<ActionResult<Dictionary<string, List<string>>?>> TryGetOptionCodes(string querystring)
        {
            var q = querystring;
            var dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(q);
            string[] Selectedtickers = { };
            Dictionary<string, List<string>>? AllCodes = new();

            try
            {
                Selectedtickers = dict.Values.Select(x => x.ToString()).ToArray();
            }
            catch (KeyNotFoundException e)
            {
                //log exception here
                Console.WriteLine(e.Message);
                AllCodes = null; Response.StatusCode = 404;
                return AllCodes;

            }

            foreach (var ticker in Selectedtickers)
            {
                var codes = await this._optionRetrievalService.TryQueryOptionCodesAsync(ticker);
                if (codes == null) { Response.StatusCode = 404; AllCodes = null; return AllCodes; }
                AllCodes[ticker] = codes;
            }

            return AllCodes;
        }

        [HttpGet]
        [Route("[controller]/{querystring}/getdailyoptionprices")]
        [Produces("application/json")]
        public async Task<ActionResult<Dictionary<string, List<OptionPrice>>?>> TryGetDailyOptionPrices(string querystring)
        {
            var q = querystring;
            var dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(q);
            string[] SelectedOptionCodes = { };
            Dictionary<string, List<OptionPrice>>? AllPrices = new();

            try
            {
                SelectedOptionCodes = dict.Values.Select(x => x.ToString()).ToArray();
            }
            catch (KeyNotFoundException e)
            {
                //log exception here
                Console.WriteLine(e.Message);
                AllPrices = null; Response.StatusCode = 404;
                return AllPrices;

            }

            foreach (var code in SelectedOptionCodes)
            {
                var prices = await this._optionRetrievalService.TryQueryOptionDailyPricesAsync(code);
                if (prices == null) { Response.StatusCode = 404; AllPrices = null; return AllPrices; }
                AllPrices[code] = prices;
            }

            return AllPrices;
        }
    }
}
