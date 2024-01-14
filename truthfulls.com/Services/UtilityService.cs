using Azure;
using Microsoft.AspNetCore.Http;
using truthfulls.com.Models;
using truthfulls.com.StockModels;

namespace truthfulls.com.Services
{
    //utility methords to parse query strings and return the information in a model
    public class UtilityService
    {
        public UtilityService()
        {
        }

        public QueryStringTickersDateRange? TryParseTickersDateRange(string? querystring)
        {
            var q = querystring;
            var dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(q);
            string datebegin; string dateend;
            string[] Selectedtickers = { };


            try
            {
                datebegin = dict["datebegin"].ToString(); dict.Remove("datebegin");
                dateend = dict["dateend"].ToString(); dict.Remove("dateend");
                Selectedtickers = dict.Values.Select(x => x.ToString()).ToArray();
            }
            catch (KeyNotFoundException e)
            {
                //log exception here
                return null;
            }

            return new QueryStringTickersDateRange() { Tickers = Selectedtickers, datebegin = datebegin, dateend = dateend };
        }

        public List<string>? TryParseTickers(string? querystring)
        {
            var q = querystring;
            var dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(q);
            List<string> Selectedtickers = new();
            Dictionary<string, List<string>>? AllCodes = new();

            try
            {
                Selectedtickers = dict.Values.Select(x => x.ToString()).ToList<string>();
            }
            catch (KeyNotFoundException e)
            {
                //log exception here
                return null;
            }

            return Selectedtickers;
        }




    }
}
