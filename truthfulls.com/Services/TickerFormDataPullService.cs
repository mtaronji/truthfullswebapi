using Microsoft.Data.Sqlite;
using truthfulls.com.Data;
using Microsoft.Data.SqlClient;
using truthfulls.com.Models;
using truthfulls.com.Models.PlotlyModels;
using Microsoft.Extensions.Primitives;
using System.Runtime.Intrinsics.X86;
using Microsoft.Extensions.Caching.Memory;

namespace truthfulls.com.Services
{
    public class TickerFormDataPullService
    {
        //private readonly SqlConnectionStringBuilder builder;
 
        //public TickerFormDataPullService(IConfiguration config) 
        //{
        //    builder = new();
        //    this.builder.ConnectionString = config.GetConnectionString("default");
        //}

        ////for testing
        //public TickerFormDataPullService(string connectionString)
        //{
        //    builder = new();
        //    this.builder.ConnectionString = connectionString;
        //    //this._cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());
        //}
        //public async Task<PlotlyMultipleTickerTracesModel?> GetMultiTracePlotlyModelAsync(string[] tickers, string datebegin, string dateend)
        //{
        //    PlotlyMultipleTickerTracesModel? plotmodel = new PlotlyMultipleTickerTracesModel();

        //    //loop through all the tickers we are requesting and load up the plotly model
        //    //some graphing plots are not allowed more
        //    tickers = tickers.Select(x => x.ToUpper().Trim()).Distinct().ToArray();



        //    for (int i = 0; i < tickers.Length; i++)
        //    {
        //        var sql = SQLiteQueries.DailyStockData2d(tickers[i].ToUpper(), datebegin, dateend);
        //        try
        //        {
        //            using (var connection = new SqliteConnection(this.builder.ConnectionString))
        //            {
        //                connection.Open();
        //                var command = connection.CreateCommand();
        //                command.CommandText = sql;
        //                var x = new List<string>();
        //                var y = new List<decimal>();
        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        if (reader.IsDBNull(0) || reader.IsDBNull(1)) { return null; }
        //                        x.Add(reader.GetDateTime(0).ToString("yyyy-MM-dd"));
        //                        y.Add(reader.GetDecimal(1));                              
        //                    }

        //                    reader.Close();
        //                    if(x.Count == 0 || y.Count == 0) { return  null; }
        //                }

        //                plotmodel.AddTrace(x.ToArray(), y.ToArray(), tickers[i]);
        //                plotmodel.AddYAxis(i + 1);  //add 1 to correct array index mismatch

        //            }
        //        }
        //        catch (SqlException ex) { Console.Write(ex.Message); }

        //    }

        //    //configure the model        

        //    if (plotmodel.traces.Count == 0) { return null; }
        //    else { return plotmodel; }
        //}

        //public async Task<List<string>> GetTickersAsync()
        //{
        //    List<string> tickers = new List<string>();
        //    var sql = SQLiteQueries.Tickers();
        //    try
        //    {
        //        using (var connection = new SqliteConnection(this.builder.ConnectionString))
        //        {
        //            connection.Open();
        //            var command = connection.CreateCommand();
        //            command.CommandText = sql;
        //            using (var reader = await command.ExecuteReaderAsync())
        //            {

        //                while (reader.Read())
        //                {   
        //                    tickers.Add(reader.GetString(0));

        //                }
        //                reader.Close();
        //            }

        //        }
        //    }
        //    catch (SqlException ex) { Console.Write(ex.Message); }
        //    return tickers;
        //}

        //public async Task<decimal> GetEMAAsync(string ticker, int duration)
        //{
        //    ticker = ticker.ToUpper().Trim();
        //    var ema = new decimal();
        //    string queryString = SQLiteQueries.EMA(ticker, duration);
        //    try
        //    {
        //        using (var connection = new SqliteConnection(this.builder.ConnectionString))
        //        {
        //            connection.Open();
        //            var command = connection.CreateCommand();
        //            command.CommandText = queryString;

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                while (reader.Read())
        //                {
        //                    ema = reader.IsDBNull(0)? 0.0M: reader.GetDecimal(0);

        //                }
        //                reader.Close();
        //            }
        //        }
        //    }
        //    catch (SqlException ex) { Console.WriteLine(ex.Message); ema = 0.0M; }
        //    return ema;
        //}

        //public async Task<decimal> GetSMAAsync(string ticker, int duration)
        //{
        //    ticker = ticker.ToUpper().Trim();
        //    var sma = new decimal();
        //    string queryString = SQLiteQueries.SMA(ticker, duration);

        //    try
        //    {
        //        using (var connection = new SqliteConnection(this.builder.ConnectionString))
        //        {
        //            connection.Open();
        //            var command = connection.CreateCommand();
        //            command.CommandText = queryString;

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {

        //                while (reader.Read())
        //                {
        //                    sma = reader.IsDBNull(0) ? 0.0M : reader.GetDecimal(0);

        //                }
        //                reader.Close();
        //            }
        //        }

        //    }
        //    catch (SqlException ex) { Console.WriteLine(ex.Message); sma = 0.0M; }
        //    return sma;
        //}

    }

        //public async Task<JsonResult> GetWeeklyPrices()
        //{

        //}

        //public async Task<JsonResult> GetWeeklyPrices()
        //{

        //}
    
}
