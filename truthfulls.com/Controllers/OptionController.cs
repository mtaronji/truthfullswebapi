using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using truthfulls.com.Data;
using truthfulls.com.Models;
using truthfulls.com.Services;


namespace truthfulls.com.Controllers
{

    [ApiController]
    public class OptionController : ControllerBase
    {
        private OptionContext _optioncontext;
        private UtilityService _utility;
        public OptionController(OptionContext optioncontext, UtilityService Utility) 
        {

            this._optioncontext = optioncontext;
            this._utility = Utility;
        }

        [HttpGet]
        [Route("[controller]/getoptioncodes/{querystring}")]
        [Produces("application/json")]
        public async Task<ActionResult<Dictionary<string, List<string>>?>> TryGetOptionCodes(string? querystring = null)
        {
            var queryobject = this._utility.TryParseTickers(querystring); if (queryobject == null) { return BadRequest(); }
            var allcodes = await this._optioncontext.TryGetOptionCodesAsync(queryobject);
            
            return Ok(allcodes);
        }

        [HttpGet]
        [Route("[controller]/getdailyoptionprices/{querystring}")]
        [Produces("application/json")]
        public async Task<ActionResult<Dictionary<string, List<OptionModels.Price>>?>> TryGetDailyOptionPrices(string? querystring = null)
        {
            var tickers = this._utility.TryParseTickers(querystring); if (tickers == null) { return BadRequest(); }
            var prices = await  this._optioncontext.TryGetDailyOptionPrices(tickers);

            return Ok(prices);
          
        }
    }
}
