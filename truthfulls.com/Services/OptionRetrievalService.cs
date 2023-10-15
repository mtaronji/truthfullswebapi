using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Diagnostics;
using truthfulls.com.Data;
using truthfulls.com.Models;

namespace truthfulls.com.Services
{
    public class OptionRetrievalService
    {
        private readonly SqlConnectionStringBuilder Builder;
        private List<string> CachedTickers;
        private string AllTickerNamesCache;
        private IMemoryCache _Cache;

        public OptionRetrievalService(IConfiguration config, IMemoryCache cache)
        {
            this._Cache = cache;
            this.CachedTickers = new List<string>() { };
            this.AllTickerNamesCache = "Tickers";
            this.Builder = new();
            var connectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_option");
            this.Builder.ConnectionString = connectionString;
        }

        //for testing
        public OptionRetrievalService(string ConnectionString)
        {
            this._Cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());
            this.CachedTickers = new List<string>() { "SPY", "NVDA", "TSLA", "QQQ", "GOOGL", "AMZN" };
            this.AllTickerNamesCache = "Tickers";
            this.Builder = new();
            this.Builder.ConnectionString = ConnectionString;
        }

        public async Task<List<string>?> TryQueryOptionCodesAsync(string ticker)
        {
            string sql = "";
            sql = SQLiteQueries.OptionCodes(ticker);


            Debug.Assert(sql != "");


            var codes = new List<string>();
            try
            {
                using (var connection = new SqliteConnection(this.Builder.ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = sql;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            if (await reader.IsDBNullAsync("Code")) { return null; }
                            var code = reader.GetString("Code");
                            codes.Add(code);
                        }
                    }
                }
                if (codes.Count == 0) { codes = null; }
                return codes;
            }
            catch (SqliteException ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        public async Task<List<OptionPrice>?> TryQueryOptionDailyPricesAsync(string ticker)
        {
            string sql = "";
            sql = SQLiteQueries.OptionPriceData(ticker);

            Debug.Assert(sql != "");

            var prices = new List<OptionPrice>();
            try
            {
                using (var connection = new SqliteConnection(this.Builder.ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = sql;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            if (await reader.IsDBNullAsync("OptionCode")) { return null; }
                            if (await reader.IsDBNullAsync("MaturityDate")) { return null; }
                            if (await reader.IsDBNullAsync("Date")) { return null; }
                            if (await reader.IsDBNullAsync("Open")) { return null; }
                            if (await reader.IsDBNullAsync("AdjClose")) { return null; }
                            if (await reader.IsDBNullAsync("High")) { return null; }
                            if (await reader.IsDBNullAsync("Low")) { return null; }
                            if (await reader.IsDBNullAsync("Volume")) { return null; }
                            if (await reader.IsDBNullAsync("VWAP")) { return null; }
                            if (await reader.IsDBNullAsync("Duration")) { return null; }

                            var price = new OptionPrice();

                            price.optioncode = reader.GetString("OptionCode");
                            price.maturitydate = reader.GetString("MaturityDate");
                            price.date = reader.GetString("Date");
                            price.open = reader.GetDecimal("Open");
                            price.adjclose = reader.GetDecimal("AdjClose");
                            price.high = reader.GetDecimal("High");
                            price.low = reader.GetDecimal("Low");
                            price.volume = reader.GetInt64("Volume");
                            price.vwap = reader.GetDecimal("VWAP");
                            price.duration = reader.GetInt64("Duration");

                            prices.Add(price);

                        }
                    }
                }
                if (prices.Count == 0) { prices = null; }
                return prices;
            }
            catch (SqliteException ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }
    }
}