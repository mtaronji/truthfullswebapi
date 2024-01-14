namespace truthfulls.com.Models
{
    public class QueryStringTickersDateRange
    {
        public required string[] Tickers { get; set; }
        public required string datebegin { get; set; }
        public required string dateend { get; set;}
    }
}
