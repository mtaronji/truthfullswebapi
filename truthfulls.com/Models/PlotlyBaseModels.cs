using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing.Text;

namespace truthfulls.com.Models.PlotlyModels
{
    public abstract class PlotlyBaseGraphModel
    {
        protected string[] datacolors = { "rgba(255, 100, 102, 1)", "rgb(128, 0, 128)", "rgb(219, 64, 82)", "rgb(55, 128, 191)", "rgb(55, 0, 191)", "rgb(0, 128, 191)", "rgb(200, 75, 75)", "rgb(55, 200, 75)", "rgb(200, 25, 0)", "rgb(55, 50, 240)" };
        public PlotlyBaseGraphModel()
        {
            this.layout = new Layout();
            this.config = new Config();
        }
        public Layout layout { get; set; }
        public Config config { get; set; }

    }

    public class Layout
    {
        public Layout()
        {
            this.xaxis = new LayoutAxis("X");
            this.showlegend = true;
            this.autosize = true;
        }
        public void AddYAxis(int yaxis)
        {
            Debug.Assert(yaxis > 0);
            Debug.Assert(yaxis < 11);

            switch(yaxis)
            {
                case 1: this.yaxis = new LayoutAxis(yaxis);  break;
                case 2: this.yaxis2 = new LayoutAxis(yaxis); break;
                case 3: this.yaxis3 = new LayoutAxis(yaxis); this.xaxis.domain = new float[2] { 0.15f, 0.85f }; break;
                case 4: this.yaxis4 = new LayoutAxis(yaxis); break;
                case 5: this.yaxis5 = new LayoutAxis(yaxis); break;
                case 6: this.yaxis6 = new LayoutAxis(yaxis); break;

                case 8: this.yaxis8 = new LayoutAxis(yaxis); break;
                case 9: this.yaxis9 = new LayoutAxis(yaxis); break;
                case 10: this.yaxis10 = new LayoutAxis(yaxis); break;
            }
        }

        //take a color string array of length traces and change all the plot colors that colo
        

        public LayoutAxis? xaxis { get; set; }
        public LayoutAxis? yaxis { get; set; }
        public LayoutAxis? yaxis2 { get; set; }
        public LayoutAxis? yaxis3 { get; set; }
        public LayoutAxis? yaxis4 { get; set; }
        public LayoutAxis? yaxis5 { get; set; }
        public LayoutAxis? yaxis6 { get; set; }
        public LayoutAxis? yaxis7 { get; set; }
        public LayoutAxis? yaxis8 { get; set; }
        public LayoutAxis? yaxis9 { get; set; }
        public LayoutAxis? yaxis10 { get; set; }


        public bool autosize { get; set; }
        public bool showlegend { get; set; }
    }
    
    public class LayoutAxis
    {
        protected string[] datacolors = { "rgba(255, 100, 102, 1)", "rgb(128, 0, 128)", "rgb(219, 64, 82)", "rgb(55, 128, 191)", "rgb(55, 0, 191)", "rgb(0, 128, 191)", "rgb(200, 75, 75)", "rgb(55, 200, 75)", "rgb(200, 25, 0)", "rgb(55, 50, 240)" };
        private float positionMultiplier = 0.10f;
        //layout for xaxis
        public LayoutAxis(string xaxis)
        {
            title = xaxis;
            type = "_";
            showgrid = true;
            this.rangeslider = new RangeSlider();
            this.domain = new float[2] { 0.0f, 1.0f };
        }
        //layout for y axis
        public LayoutAxis(int yaxis)
        {
            this.tickfont = new TickFont();  this.tickfont.color = datacolors[yaxis];
            this.titlefont = new TitleFont(); this.titlefont.color = datacolors[yaxis];
            title = "yaxis";
            type = "_";
            showgrid = true;
            this.rangeslider = new RangeSlider();

            if (yaxis == 1)
            {
                this.overlaying = null;
            }
            else if (yaxis == 2)
            {
                this.title = "yaxis2";
                this.anchor = "x";
                this.side = "right";
                this.overlaying = "y";
            }
            else if (yaxis == 3)
            {
                this.title = "yaxis3";
                this.overlaying = "y";
                this.anchor = "free";
                this.side = "left";
                this.position = 0.0f;
            }

            else if (yaxis == 4)
            {
                this.title = "yaxis4";
                this.overlaying = "y";
                this.anchor = "free";
                this.side = "right";
                this.position = 1.0f;
            }
        }
        public string? title { get; set; }
        public bool showgrid { get; set; }
        public string? overlaying { get; set; }
        public string? anchor { get; set; }
        public float? position { get; set; }
        public string? side { get; set; }
        public RangeSlider rangeslider { get; set; }
        public TickFont? tickfont { get; set; }
        public TitleFont? titlefont { get; set; }
        public float[]? domain { get; set; }
        public string type { get; set; }
    }
    public class Config
    {
        public Config()
        {
            responsive = true;
            displayModeBar = true;
        }
        public bool responsive { get; set; }
        public bool displayModeBar { get; set; }
    }
    public class TraceBase
    {
        public TraceBase()
        {
            this.mode = "lines";
            this.name = "";
            this.type = "scatter";
        }
        public string mode { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public Line? line { get; set; }
        public Marker? marker { get; set; }
    }

    public class Title
    {
        public Title(string name)
        {
            font = new Font();
            text = "";
            this.text = name;

        }
        public Font font { get; set; }
        public string text { get; set; }
    }

    public class Font
    {
        public Font()
        {
            family = "Courier New, monospace";
            color = "";
            size = 16;
        }
        public string color { get; set; }
        public string family { get; set; }
        public uint size { get; set; }
    }
    public class RangeSlider
    {
        public RangeSlider()
        {
            visible = false;
        }
        public bool visible { get; set; }
    }

    public class TitleFont
    {
        public string? color { get; set; }
    }
    public class TickFont
    {
        public string? color { get; set; }
    }

    public class Line
    {
        public Line(string color, int width)
        {
            this.color = color;
            this.width = width;
        }
        public string color { get; set; }
        public int width { get; set; }
    }
    public class Marker
    {
        public Marker(string color, int size)
        {
            this.color= color;
            this.size = size;
        }
        public string color { get; set; }
        public int size { get; set; }
    }

}
