using truthfulls.com.Services;
using Punk;
using Punk.TypeNodes;


namespace TruthfullsTests
{
    internal class PunkInterpreterTests
    {
        PunkInterpreter Interpreter { get; set; }
        public PunkInterpreterTests() 
        {
            this.Interpreter = new PunkInterpreter();

        }
        [Test]
        public async Task DataSyntaxWorks()
        {
            var expression = @"[0...100000]{double, double : return x0*2.0;}{double, double : return x0/3.0;}{double, double: return Pow(x0,(1.0/3.0));}";
            var result = await this.Interpreter.InterpretAsync(expression);
            Assert.That(result.ResultType == PunkResultType.Success);
        }
        [Test]
        public async Task DataSyntaxEvaluatesCorrectly()
        {
            var expression = @"[0...100000]{double, double : return x0*2.0;}{double, double : return x0/3.0;}{double, double: return Pow(x0,(1.0/3.0));}";
            var result = await this.Interpreter.InterpretAsync(expression);
            if(result== null) { return; }

        }

        [Test]
        public async Task QuerySyntaxEvaluatesCorrectly()
        {
            var expression = @"##stocks{stocks.Prices.Where(x => x.Ticker == ""QQQ"" && x.Close > 300.0).Select(x => x)} | ->=";
            var result = await this.Interpreter.InterpretAsync(expression);
            if (result == null) { return; }

         

           
        }
    }
}



//select * from Prices p
// where p.Ticker = "QQQ"