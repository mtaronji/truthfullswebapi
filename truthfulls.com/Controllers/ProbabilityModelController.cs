using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace truthfulls.com.Controllers
{
    //[Authorize(Roles ="Subscriber")]
    [ApiController]
    [Route("[controller]")]
    public class ProbabilityModelController : ControllerBase
    {

        public ProbabilityModelController()
        {

        }

        [HttpGet]
        [Route("Poisson")]
        [Produces("application/json")]
        public async Task<IActionResult> GetDataSet(string? querystring = null)
        {
            //the user should specify exactly what data they want to import
            //querystring should specify 1 dataset if you aren't subscribed

            //if (querystring == null) { return BadRequest(new {error = "Query String is Null"}); }

            ////if optioncode is 
            //var optioncode = "someoptioncode";
            //var optionprices = await this._optionservice.TryQueryOptionDailyPricesAsync(optioncode);

            return Ok();
        }

        [HttpGet]
        [Route("Poisson")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateModel(string? querystring = null)
        {
            //the user should specify exactly what data they want to import
            //querystring should specify 1 dataset if you aren't subscribed

            //if (querystring == null) { return BadRequest(new { error = "Query String is Null" }); }

            ////if optioncode is 
            //var optioncode = "someoptioncode";
            //var optionprices = await this._optionservice.TryQueryOptionDailyPricesAsync(optioncode);

            return Ok();
        }
    }
}
