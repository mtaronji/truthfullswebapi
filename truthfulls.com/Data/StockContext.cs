using Microsoft.EntityFrameworkCore;
using truthfulls.com.Models;
using truthfulls.com.StockModels;
using System.Data;

namespace truthfulls.com.Data
{

    //to be used with sql server

    public class StockContext : DbContext
    {

        public StockContext(DbContextOptions<StockContext> options) : base(options)
        {

        }

        DbSet<FinancialDate> FinancialDates { get; set; }
        DbSet<StockModels.Stock> Stocks { get; set; }
        DbSet<StockModels.Price> StockPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StockModels.Price>().HasIndex(c => new { c.Ticker, c.Date }).IsUnique();
            modelBuilder.Entity<StockModels.Price>().HasNoKey();

        }

        public async Task<Dictionary<string, List<StockModels.PriceVM>>?> TryGetDailyStockPricesAsync(QueryStringTickersDateRange queryobject)
        {
            //set automatically back 
            var datebegin = DateTime.Parse(queryobject.datebegin);
            var dateend = DateTime.Parse(queryobject.dateend);
      
            Dictionary<string, List<PriceVM>>? allprices = new();

            foreach (var ticker in queryobject.Tickers)
            {
                var prices = await 
                              (from price in this.StockPrices
                              where price.Ticker == ticker && price.Date > datebegin && price.Date < dateend
                              select price
                              )
                             .ToListAsync<Price>();
                      
                allprices[ticker] = Price.ToPriceVM(prices);
            }
            return allprices;
        }

        public async Task<List<string>?> TryGetTickers()
        {
            return await this.Stocks.Select(s => s.Ticker).ToListAsync<string>();
        }
     
    }
}


