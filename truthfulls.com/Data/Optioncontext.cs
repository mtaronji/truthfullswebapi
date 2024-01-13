using Microsoft.EntityFrameworkCore;
using truthfulls.com.Models;
using truthfulls.com.StockModels;
using System.Data;
using truthfulls.com.OptionModels;
using Option.Extensions;
using Microsoft.AspNetCore.Mvc;
using truthfulls.com.FREDModels;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Microsoft.Extensions.Azure;


namespace truthfulls.com.Data
{

    //to be used with sql server

    public class OptionContext : DbContext
    {

        public OptionContext(DbContextOptions<OptionContext> options) : base(options)
        {

        }
        

        DbSet<FinancialDate> FinancialDates { get; set; }
        DbSet<OptionModels.OptionCode> OptionCodes { get; set; }
        DbSet<OptionModels.SequenceComplete> SequenceCompletes { get; set; }
        DbSet<OptionModels.Price> OptionPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OptionModels.Price>().HasIndex(c => new { c.Code, c.Duration }).IsUnique();
            modelBuilder.Entity<OptionModels.Price>().HasNoKey();

            modelBuilder.Entity<SequenceComplete>().Property(p => p.IsComplete).HasDefaultValue(false);
            modelBuilder.Entity<SequenceComplete>().HasKey(s => s.Code);

        }

        public async Task<Dictionary<string, List<OptionModels.OptionPriceVM>>?> TryGetDailyOptionPrices(List<string> Codes)
        {
            Dictionary<string, List<OptionModels.OptionPriceVM>> optionprices = new();

            foreach (var code in Codes)
            {
                var temp = await (
                           from price in OptionPrices
                           where price.Code == code
                           orderby price.Duration
                           select price)
                           .ToListAsync<OptionModels.Price>();

                optionprices[code] = temp.ToVM();
            }
            return optionprices;
        }

        public async Task<Dictionary<string, List<string>>?> TryGetOptionCodesAsync(List<string> tickers)
        {
                 
            //get option codes starting with the tickers
            //then from that list from a regex to further filter
            //this second filter is for matches for tickers such as 'A' matching a symbol 'A' as well as 'AMD' 'AAPLE' ...etc

            Dictionary<string, List<string>> allcodes = new();
            foreach (var ticker in tickers)
            {            
                var codes = await (from c in this.OptionCodes where c.Code.Contains(ticker) select c.Code).ToListAsync<string>();

                string pattern = $@"^{ticker}\w+";
                Regex searchTerm = new Regex(pattern);
                codes = (from c in codes where searchTerm.IsMatch(c) select c).ToList<string>();
                allcodes[ticker] = codes;
            }

            return allcodes;
        }

    }
}
