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
        //[Test]
        //public async Task DataSyntaxWorks()
        //{
      
        //}
        //[Test]
        //public async Task DataSyntaxEvaluatesCorrectly()
        //{


        //}

        //[Test]
        //public async Task QuerySyntaxEvaluatesCorrectly()
        //{

        
           
        //}
    }
}



//select * from Prices p
// where p.Ticker = "QQQ"