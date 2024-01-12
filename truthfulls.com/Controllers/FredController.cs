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
        //private MarketContext _market;
        private FREDContext _fredcontext;
        public FredController(FREDContext fredcontext)
        {
            this._fredcontext = fredcontext;
        }

        [Route("[controller]/getseries")]
        [HttpGet]
        public async Task<IActionResult> GetSeries()
        {
            var seriess = await this._fredcontext.TryGetSeriessAsync();
            if (seriess == null) { return BadRequest(); }
            else { return Ok(seriess); }
        }

        [HttpGet]
        [Route("[controller]/seriesobservations/{seriesid}")]
        public async Task<IActionResult> GetSeriesObservations(string? seriesid = null)
        {
            if (seriesid == null) { return BadRequest(); }
            var observations = await this._fredcontext.TryGetSeriesObservationsAsync(seriesid);

            if (observations == null) { return BadRequest(); }
            else { return Ok(observations); }
            
        }

    }
}
