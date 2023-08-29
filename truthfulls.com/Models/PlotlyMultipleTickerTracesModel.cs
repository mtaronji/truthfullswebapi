using Microsoft.AspNetCore.Server.IIS.Core;
using System.Drawing;

namespace truthfulls.com.Models.PlotlyModels
{
    public sealed class PlotlyMultipleTickerTracesModel : PlotlyBaseGraphModel
    {
        
        public PlotlyMultipleTickerTracesModel() :base()
        {            
            this.traces = new List<Trace>();
            this.layout.xaxis = new LayoutAxis("Dates");         
        }

        public void AddTrace(string[] x, decimal[] y, string name)
        {
            if(traces == null){throw new ArgumentNullException(nameof(traces));}

            var trace = new Trace();
            trace.name = name; trace.x = x; trace.y = y; trace.name = name;
            this.traces.Add(trace);

            this.traces[traces.Count - 1].line = new Line(datacolors[traces.Count], 1);
            this.traces[traces.Count - 1].yaxis = traces.Count > 1 ? $"y{traces.Count}" : null;
        }

        public void AddYAxis(int yaxis)
        {
            this.layout.AddYAxis(yaxis);

        }
        public List<Trace> traces { get; set; }
        
    }

    public sealed class Trace : TraceBase
    {
        public Trace()
        {

        }
        public string[]? x { get; set; }
        public decimal[]? y { get; set; }
        public string? yaxis { get; set; }
    }

}
