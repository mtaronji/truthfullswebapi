using Punk;
using Punk.TypeNodes;
using Punk.UnaryOperators;
using MathNet.Numerics.LinearAlgebra;
using truthfulls.com.Controllers;
using Punk.Types;


namespace truthfulls.com.Services
{
    //for interpretation of punk syntax
    public class PunkInterpreter
    {
        private Lexer _lexer;
        private Parser _parser;

        public PunkInterpreter()
        {
            this._lexer = new Lexer();
            this._parser = new Parser();
        }

        public async Task<PunkReturnResult> InterpretAsync(string syntax)
        {
            PunkReturnResult result;
            try
            {
                List<TreeNode> EvalationResults = new List<TreeNode>();
                List<string> PrintedTrees = new List<string>();
                var tokens = this._lexer.Read(syntax);
                var ExpressionTrees = await this._parser.ParseAsync(tokens);
                foreach (var tree in ExpressionTrees)
                {
                    EvalationResults.Add(tree.Eval());
                    PrintedTrees.Add(tree.Print());
                }
                result = new PunkReturnResult(EvalationResults, PrintedTrees);
                return result;
            }
            catch (Exception ex)
            {
                result = new PunkReturnResult(ex);
                return result;
            }
        }
        public async Task<PunkReturnResult> InterpretAsync(string syntax, List<FileVector> filevectors)
        {
     
            int count = 0;
            foreach(var v in filevectors)
            {
                var matrix = Matrix<double>.Build.DenseOfColumns(v.Vectors);
                MatrixType m = new MatrixType(matrix);
                var matrixnode = new MatrixNode(m);
                this._parser.Identifiers.Add($"F{count}", new IdentifierNode(matrixnode));
                count++;
            }
            PunkReturnResult result;
            try
            {
                List<TreeNode> EvalationResults = new List<TreeNode>();
                List<string> PrintedTrees = new List<string>();
                var tokens = this._lexer.Read(syntax);
                
                var ExpressionTrees = await this._parser.ParseAsync(tokens);
                foreach (var tree in ExpressionTrees)
                {
                    EvalationResults.Add(tree.Eval());
                    PrintedTrees.Add(tree.Print());
                }
                result = new PunkReturnResult(EvalationResults, PrintedTrees);
                return result;
            }
            catch (Exception ex)
            {
                result = new PunkReturnResult(ex);
                return result;
            }
        }
    }
    public class PunkReturnResult
    {
        
        string? ExceptionMessage;
        public PunkResultType ResultType { get; private set; }
        private IEnumerable<TreeNode>? EvalationResults;
        private List<string> PrintedTrees;
        public PunkReturnResult(Exception ex)
        {
            this.ResultType = PunkResultType.Fail; this.ExceptionMessage = ex.Message;
            this.EvalationResults = null;
            this.PrintedTrees = new List<string>();
        }
        public PunkReturnResult(IEnumerable<TreeNode> EvaulationResults, List<string> PrintedTrees)
        {
            this.EvalationResults = EvaulationResults;
            this.ResultType = PunkResultType.Success;
            this.PrintedTrees = PrintedTrees;
        }

        public List<object>? GetEvaluationResults()
        {
            List<object> results =  new List<object>();
            if (this.EvalationResults != null && this.PrintedTrees != null)
            {
                var treedata = this.EvalationResults.Zip(this.PrintedTrees, (eval, print) => new { eval = eval, print = print });
                foreach (var node in treedata)
                {
                    if(node.eval is IResultTreeNode) 
                    {
                        var resultnode = (IResultTreeNode)node.eval;
                        var result = resultnode.GetResult();
                        if (result != null)
                        {
                            if(resultnode is IdentifierNode)
                            {
                                var idnode = (IdentifierNode)resultnode;
                                if (idnode.Value == null) { return null; }
                                else { resultnode = (IResultTreeNode)idnode.Value; }
                                    
                            }
                            else if (resultnode is QueryNode)
                            {
                                if (result != null) { results.Add( new { type = "query", results = result, print = node.print}); }
                            }
                            else if (resultnode is DataNode)
                            {
                                if (result != null) { results.Add(new {type = "data", results = result, print = node.print }); }
                            }

                            else if (resultnode is IdentifierNode)
                            {
                                if (result != null) { results.Add(new { type = "data", results = result, print = node.print }); }
                            }
                            else if (resultnode is NumberNode)
                            {
                                var numbernode = (NumberNode)resultnode;                             
                                if (result != null) { results.Add(new { type = "number", results = result, print = $"{node.print} = {numbernode.Value.Value}" }); }
                            }
                            else if(resultnode is MatrixNode)
                            {
                                var matrixnode = (MatrixNode)resultnode;
                                if (result != null) { results.Add(new { type = "matrix", results = result, print = $"Matrix({matrixnode.matrix.Value.RowCount.ToString()} x {matrixnode.matrix.Value.ColumnCount.ToString()})" }); }
                            }
                            else
                            {

                            }

                        }                   
                    }
                }
            }
            else
            {
                return null;
            }
            return results;             
        }
        public string? GetErrorMessage()
        {
            return this.ExceptionMessage;
        }
    }

    public enum PunkResultType
    {
        Success,
        Fail
    }
}
