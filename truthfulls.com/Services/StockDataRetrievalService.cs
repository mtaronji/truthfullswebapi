using truthfulls.com.Models;
using truthfulls.com.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Runtime.Caching;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace truthfulls.com.Services
{
    public class StockDataRetrievalService
    {
        private readonly SqlConnectionStringBuilder Builder;
        private List<string> CachedTickers;
        private string AllTickerNamesCache;
        private IMemoryCache _Cache;

        public StockDataRetrievalService(IConfiguration config, IMemoryCache cache) 
        {
            this._Cache = cache;
            this.CachedTickers = new List<string>() { };
            this.AllTickerNamesCache = "Tickers";
            this.Builder = new();
            var connectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_stock");
            this.Builder.ConnectionString = connectionString;
        }

        //for testing
        public StockDataRetrievalService(string ConnectionString)
        {
            this._Cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());
            this.CachedTickers = new List<string>() { "SPY", "NVDA", "TSLA", "QQQ", "GOOGL", "AMZN" };
            this.AllTickerNamesCache = "Tickers";
            this.Builder = new();
            this.Builder.ConnectionString = ConnectionString;
        }

        public async Task<List<Price>?> TryGetPriceDataAsync(string ticker, string datebegin, string dateend, TimePeriod period)
        {
            List<Price>? Prices;

            //check cache first
            //if not in cache query and add the data to the cache
            if(CachedTickers.Contains(ticker)){

                if(!this._Cache.TryGetValue(ticker, out Prices))
                {
                    Prices = await TryQueryPriceDataAsync(ticker, datebegin, dateend, period);

                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddDays(1)
                    };
                    //setting cache entries
                    this._Cache.Set(ticker, Prices, cacheExpiryOptions);
                }
            }
            else
            {
                Prices = await TryQueryPriceDataAsync(ticker, datebegin, dateend, period);
            }
            return Prices;          
        }

        public async Task<List<string>?> GetTickerDataAsync()
        {
            //check cache first
            List<string>? Tickers;


            if (!this._Cache.TryGetValue(this.AllTickerNamesCache, out Tickers))
            {
                //if not in cache query  and add it
                Tickers = await TryGetTickerDataAsync();

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddDays(1)
                };
                //setting cache entries
                this._Cache.Set(this.AllTickerNamesCache, Tickers, cacheExpiryOptions);
            }

            return Tickers;
        }
        private async Task<List<Price>?> TryQueryPriceDataAsync(string ticker, string datebegin, string dateend, TimePeriod period)
        {
            string sql = "";

            switch (period)
            {
                case TimePeriod.Daily:
                    sql = SQLiteQueries.DailyPriceData(ticker,datebegin,dateend); break;
                case TimePeriod.Weekly:
                    sql = SQLiteQueries.WeeklyPriceData(ticker, datebegin, dateend); break;
            }

            Debug.Assert(sql != "");


            var Prices = new List<Price>();
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
                            
                            //return null if any column returns null or missing data
                            if(period == TimePeriod.Daily)
                            {
                                if (await reader.IsDBNullAsync("Date")) { return null; }
                            }
                            else if(period == TimePeriod.Weekly)
                            {
                                if (await reader.IsDBNullAsync("weekstart")) { return null; }
                                if (await reader.IsDBNullAsync("weekend")) { return null; }
                            }

                            if (await reader.IsDBNullAsync("Open")) { return null; }
                            if (await reader.IsDBNullAsync("Close")) { return null; }
                            if (await reader.IsDBNullAsync("AdjClose")) { return null; }
                            if (await reader.IsDBNullAsync("Low")) { return null; }
                            if (await reader.IsDBNullAsync("High")) { return null; }
                            if (await reader.IsDBNullAsync("Volume")) { return null; }

                            var price = new Price(); 
                            if(period == TimePeriod.Daily) { price.Date = reader.GetDateTime("Date"); }
                            if(period == TimePeriod.Weekly) { price.Date = reader.GetDateTime("weekstart"); }

                            price.Open = reader.GetDecimal("Open");
                            price.Close = reader.GetDecimal("Close");
                            price.AdjClose = reader.GetDecimal("AdjClose");
                            price.Low = reader.GetDecimal("Low");
                            price.High = reader.GetDecimal("High");
                            price.Volume = reader.GetInt64("Volume");
                            
                            Prices.Add(price);
                        }                      
                    }
                }
                if (Prices.Count == 0) { Prices = null; }
                return Prices;
            }
            catch (SqliteException ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        private async Task<List<string>?> TryGetTickerDataAsync()
        {
            var sql = SQLiteQueries.Tickers();
            var Tickers = new List<string>();
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
                            //return null if any column returns null or missing data
                            if (await reader.IsDBNullAsync(0)) { return null; }

                            var ticker = reader.GetString(0);
                            Tickers.Add(ticker);
                        }
                    }
                }
                return Tickers;
            }
            catch (SqliteException ex)
            {
                return null;
            }
        }
    }
}
