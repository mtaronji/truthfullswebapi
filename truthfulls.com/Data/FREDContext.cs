using Microsoft.EntityFrameworkCore;
using truthfulls.com.StockModels;
using System.Data;
using truthfulls.com.FREDModels;


namespace truthfulls.com.Data
{
    public class FREDContext : DbContext
    {

        public FREDContext(DbContextOptions<FREDContext> options) : base(options)
        {

        }

        DbSet<FinancialDate> FinancialDates { get; set; }

        DbSet<FREDModels.Series> Series { get; set; }
        DbSet<FREDModels.Observations> Observations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FREDModels.Observations>().HasNoKey();
            modelBuilder.Entity<FREDModels.Observations>().HasIndex(o => new { o.SeriesID, o.Date }).IsUnique();

        }

        public async Task<List<FREDModels.Observations>?> TryGetSeriesObservationsAsync(string seriesid)
        {
            var observations = await (from obs in this.Observations where obs.SeriesID == seriesid orderby obs.Date select obs).ToListAsync<FREDModels.Observations>();
            return observations;
        }

        public async Task<List<Series>?> TryGetSeriessAsync()
        {
            var seriess = await (from s in this.Series select s).ToListAsync<Series>();
            return seriess;
        }
    }
}