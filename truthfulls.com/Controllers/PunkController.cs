using Microsoft.AspNetCore.Mvc;
using truthfulls.com.Services;
using System.Text.Json;
using System.Globalization;
using System.Net;

namespace truthfulls.com.Controllers
{
    //evaluate punk syntax that comes from the front end
    [ApiController]
    public class PunkController :ControllerBase
    {
        private PunkInterpreter _interpreter;


        public PunkController(PunkInterpreter Interpreter)
        {         
            this._interpreter = Interpreter;
        }

        [HttpPost]
        [Route("interpret")]
        [Produces("application/json")]
        public async Task<IActionResult> Interpret([FromBody] JsonDocument data)
        {
            PunkReturnResult result;
            PunkPostParser parser = new PunkPostParser(data);
            var syntax = WebUtility.UrlDecode(parser.GetSyntax());
            if(syntax == string.Empty) { return BadRequest(new {error = "syntax is missing"}); }
            var filevectors = parser.GetFileVectors();
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

    public class PunkPostParser
    {
        string syntax = string.Empty;

        List<FileVector> FileVectors = new();
        public PunkPostParser(JsonDocument docu)
        {         
            var documentEnumerator = docu.RootElement.EnumerateArray();
            if (documentEnumerator.MoveNext())  //move to first object
            {
                var e = documentEnumerator.Current.EnumerateObject();
                if (e.MoveNext()) { this.syntax = e.Current.Value.ToString(); }
            }

            if (documentEnumerator.MoveNext()) //move to second object which should be csv file arr
            {
                var e = documentEnumerator.Current.EnumerateObject();
                if (e.MoveNext()) //if csvfiles exist
                {                                      
                    foreach (var file in e.Current.Value.EnumerateArray())
                    {                    
                        bool FirstRow = true;
                        List<string> csvHeaders = new List<string>();
                        List<Type> FieldTypes = new List<Type>();
                        List<List<double>> Columns = new();
                        
                        foreach (var rowdata in file.EnumerateArray())
                        {                         
                            if (FirstRow)
                            {
                                FirstRow = false;
                                int count = 0;
                                foreach (var item in rowdata.EnumerateObject())
                                {
                                    var fieldtype = GetFieldType(item.Value.ToString());
                                    if (fieldtype != null && 
                                            (fieldtype == typeof(double) || fieldtype == typeof(long)) )
                                    { 
                                        FieldTypes.Add(fieldtype);
                                        csvHeaders.Add(item.Name.ToString());
                                        Columns.Add(new List<double>());
                                        count++;
                                    }
                                }                          
                            }
                            else
                            {                              
                                int count = 0;
                                foreach (var item in rowdata.EnumerateObject())
                                {
                                    if (csvHeaders.Contains(item.Name.ToString()))
                                    {
                                        Columns[count].Add(double.Parse(item.Value.ToString()));
                                        count++;
                                    }                                                            
                                }                               
                            }
                        }
                        this.FileVectors.Add(new FileVector() { Vectors = Columns });                 
                    }
                }
            }
            EvenFileVectors();
        }
        public void EvenFileVectors()
        {
            //force vectors to be the same size for matrix purposes
            int min = this.FileVectors[0].Vectors[0].Count;
            foreach (var v in this.FileVectors)
            {
                min = v.Vectors[0].Count < min ? min = v.Vectors[0].Count : min;
            }
            for (int i = 0; i < this.FileVectors.Count; i++)
            {
                if (this.FileVectors[i].Vectors[0].Count > min)
                {
                    for (int j = 0; j < this.FileVectors[i].Vectors.Count; j++)
                    {
                        this.FileVectors[i].Vectors[j] = this.FileVectors[i].Vectors[j].Take(min).ToList();
                    }
                }
            }
        }
        public string GetSyntax()
        {
            return syntax;
        }
        public List<FileVector> GetFileVectors()
        {
            return this.FileVectors;
        }

        public dynamic ParseField(string field, Type type)
        {
            if(type == typeof(long))
            {
                return long.Parse(field);
            }
            else if(type == typeof(double))
            {
                return double.Parse(field);
            }
            else if(type == typeof(DateOnly))
            {
                return DateOnly.Parse(field);
            }
            else if(type == typeof(DateTime))
            {
                return DateTime.Parse(field);
            }
            else
            {
                throw new NotImplementedException($"Unable to parse data type with value {field} --- please see suported csv field types");
            }
        }
        public Type? GetFieldType(string field)
        {
            double trydouble;
            long trylong;
            if(double.TryParse(field, out trydouble)) { return  typeof(double); }
            else if(long.TryParse(field, out trylong)) {return typeof(long); }
            else
            {
                //try parse dates
                CultureInfo enUS = new CultureInfo("en-US");
                DateTime dateTimeValue;
                DateOnly dateOnlyvalue;

                if(DateOnly.TryParseExact(field, "yyyy-MM-dd",out dateOnlyvalue))
                {
                    return typeof(DateOnly);
                }
                else if (DateTime.TryParse(field, out dateTimeValue))
                {
                    return typeof(DateTime);
                }
                else
                {
                    return null;
                }
            }
            throw new NotImplementedException($"Unable to parse data type with value {field} --- please see suported csv field types");
        }
        
    }

    public class FileVector
    {
        public List<List<double>> Vectors = new();      
    }
}
