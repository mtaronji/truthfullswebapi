using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using truthfulls.com.Models;


namespace truthfulls.com.Controllers
{

    [ApiController]
    public class FredController : ControllerBase
    {
        public FredController()
        {
    
        }

        [Route("[controller]/getseries")]
        [HttpGet]
        public async Task<IActionResult> GetSeries()
        {
            //List<Series>? series = await this._fredRetrievalService.TryGetSeries();
            //if(series == null) { return BadRequest(); }
            //else{return Ok(series);}
            return Ok();
        }

        [HttpGet]
        [Route("[controller]/seriesobservations/{seriesid}")]
        public async Task<IActionResult> GetSeriesObservations(string? seriesid = null)
        {
            //if (seriesid == null) { return BadRequest(); }
            //var observations = await this._fredRetrievalService.TryGetSeriesObservationsAsync(seriesid);

            //if (observations == null) { return BadRequest(); }
            //else { return Ok(observations); }

            return Ok();
            
        }

    }
}
