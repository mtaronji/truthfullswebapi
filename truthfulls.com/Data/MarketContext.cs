using Microsoft.EntityFrameworkCore;
using truthfulls.com.Models;
using truthfulls.com.StockModels;
using System.Data;
using truthfulls.com.OptionModels;
using Option.Extensions;
using Microsoft.AspNetCore.Mvc;


namespace truthfulls.com.Data
{
    public class MarketContext : DbContext
    {
        
        public MarketContext(DbContextOptions<MarketContext> options):base(options)
        {
          
        }
        
        
        DbSet<FinancialDate> FinancialDates { get; set; }
        DbSet<StockModels.Stock> Stocks { get; set; }
        DbSet<OptionModels.OptionCode> OptionCodes {get;set;}
        DbSet<OptionModels.SequenceComplete> SequenceCompletes { get; set; }

        DbSet<OptionModels.Price> OptionPrices { get; set; }
        DbSet<StockModels.Price> StockPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OptionModels.Price>().HasIndex(c => new { c.Code, c.Duration }).IsUnique();
            modelBuilder.Entity<OptionModels.Price>().HasNoKey();
       
            modelBuilder.Entity<StockModels.Price>().HasIndex(c => new { c.Ticker, c.Date }).IsUnique();
            modelBuilder.Entity<StockModels.Price>().HasNoKey();

            modelBuilder.Entity<SequenceComplete>().Property(p => p.IsComplete).HasDefaultValue(false);
            modelBuilder.Entity<SequenceComplete>().HasKey(s => s.Code);
   
        }

        public async Task<Dictionary<string, List<StockModels.PriceVM>>?> TryGetDailyStockPricesAsync(QueryStringTickersDateRange queryobject)
        {
            Dictionary<string, List<PriceVM>>? allprices = new();

            foreach (var ticker in queryobject.Tickers) 
            {
                var prices = await this.StockPrices.FromSqlRaw($"select * from Stock.GetStockPrices ('{ticker}','{queryobject.datebegin}','{queryobject.dateend}') order by [date]").ToListAsync<StockModels.Price>();
                allprices[ticker] = truthfulls.com.StockModels.Price.ToPriceVM(prices);
            } 

            return allprices;
        }

        public async Task<List<string>?> TryGetTickers()
        {
            return await this.Stocks.Select(s => s.Ticker).ToListAsync<string>();
        }
        public async Task<Dictionary<string, List<OptionModels.OptionPriceVM>>?> TryGetDailyOptionPrices(List<string> Codes)
        {
            Dictionary<string, List<OptionModels.OptionPriceVM>> optionprices = new();

            foreach (var code in Codes)
            {             
                var temp = await this.OptionPrices.FromSqlRaw($"select * from stockoption.price where code = '{code}' order by duration").ToListAsync<OptionModels.Price>();
                optionprices[code] = temp.ToVM();
            }
            return optionprices;
        }
         
        

        public async Task<Dictionary<string,List<string>>?> TryGetOptionCodesAsync(List<string> tickers)
        {
            //raw query from a defined sql server inline table function
            Dictionary<string, List<string>> allcodes = new();
            foreach(var ticker in tickers)
            {
                var codes = await this.OptionCodes.FromSqlRaw($"select * from StockOption.GetCodes('{ticker}')" ).Select(o => o.Code).ToListAsync<string>();
                allcodes[ticker] = codes;
            }
              
            return allcodes;
        }


    }
}
