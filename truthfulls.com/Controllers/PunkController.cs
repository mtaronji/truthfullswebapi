using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Punk;
using System.Net;

namespace truthfulls.com.Controllers
{
    //evaluate punk syntax that comes from the front end

    [ApiController]
    public class PunkController :ControllerBase
    {

        public PunkController()
        {         

        }

        [HttpPost]
        [Route("interpret")]
        [Produces("application/json")]
        public async Task<IActionResult> Interpret([FromBody] JsonDocument data)
        {
          
            PunkReturnResult result;
            var JSONparser = new Punk.JSONDocumentParser();
            var interpreter = new Punk.Interpreter();
            JSONparser.Parse(data);
            var syntax = WebUtility.UrlDecode(JSONparser.GetSyntax());
            if (syntax == string.Empty) { return BadRequest(new { error = "syntax is missing" }); }

            
            var filevectors = JSONparser.GetFileVectors();
            if (filevectors.Count > 0)
            {
                result = await interpreter.InterpretAsync(syntax, filevectors);
            }
            else
            {
                result = await interpreter.InterpretAsync(syntax);
            }
            var evaluations = result.GetEvaluationResults();
            if (evaluations == null)
            {
                return BadRequest(result.GetErrorMessage());
            }
            else
            {
                return Ok(evaluations);
            }

        }
    }

}
