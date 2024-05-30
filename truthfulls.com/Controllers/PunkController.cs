using Microsoft.AspNetCore.Mvc;
using truthfulls.com.Services;
using System.Text.Json;
using System.Net;
using System.Threading.Tasks;


namespace truthfulls.com.Controllers
{
    //evaluate punk syntax that comes from the front end

    [ApiController]
    public class PunkController :ControllerBase
    {
        private PunkInterpreter _interpreter;
        private PunkPostParser _postParser;

        public PunkController(PunkInterpreter Interpreter, PunkPostParser postparser)
        {         
            this._interpreter = Interpreter;
            this._postParser = postparser;
        }

        [HttpPost]
        [Route("interpret")]
        [Produces("application/json")]
        public async Task<IActionResult> Interpret([FromBody] JsonDocument data)
        {
            PunkReturnResult result;
            this._postParser.Parse(data);
            var syntax = WebUtility.UrlDecode(_postParser.GetSyntax());
            if(syntax == string.Empty) { return BadRequest(new {error = "syntax is missing"}); }
            var filevectors = _postParser.GetFileVectors();
            if (filevectors.Count > 0)
            {
                result = await _interpreter.InterpretAsync(syntax, filevectors);
            }
            else
            {
                result = await _interpreter.InterpretAsync(syntax);
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
