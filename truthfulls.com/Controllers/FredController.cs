using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using truthfulls.com.Data;
using truthfulls.com.Models;


namespace truthfulls.com.Controllers
{

    [ApiController]
    public class FredController : ControllerBase
    {
        private MarketContext _market;
        public FredController(MarketContext market)
        {
            this._market = market;
        }

        [Route("[controller]/getseries")]
        [HttpGet]
        public async Task<IActionResult> GetSeries()
        {
            var seriess = await this._market.TryGetSeriessAsync();
            if (seriess == null) { return BadRequest(); }
            else { return Ok(seriess); }
        }

        [HttpGet]
        [Route("[controller]/seriesobservations/{seriesid}")]
        public async Task<IActionResult> GetSeriesObservations(string? seriesid = null)
        {
            if (seriesid == null) { return BadRequest(); }
            var observations = await this._market.TryGetSeriesObservationsAsync(seriesid);

            if (observations == null) { return BadRequest(); }
            else { return Ok(observations); }
            
        }

    }
}
