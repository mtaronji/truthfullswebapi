using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace truthfulls.com.StockModels
{

    [Table("Stock", Schema = "Stock")]
    public class Stock
    {
        [Key]
        [Column(TypeName ="nvarchar(20)")]
        public string Ticker { get; set; } = null!;

        public string? CompanyName { get; set; }

        public string? Country { get; set; }

    }

    [Table("Sector", Schema = "Stock")]
    public class Sector
    {
        [ForeignKey("Ticker")]
        public Stock Stock { get; set; } = null!;
        public string Ticker { get; set; } = null!;

        public string SectorName { get; set; } = null!;


        public string? Description { get; set; }

    }

    [Table("Price", Schema ="Stock")]
    public class Price
    {
        [ForeignKey("Ticker")]
        public Stock stock { get; set; } = null!;

        public string Ticker { get; set; } = null!;


        [ForeignKey("Date")]
        public FinancialDate FinancialDate { get; set; } = null!;
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "money")]
        public decimal Open { get; set; }

        [Column(TypeName = "money")]
        public decimal Close { get; set; }

        [Column(TypeName = "money")]
        public decimal AdjClose { get; set; }

        [Column(TypeName = "money")]
        public decimal Low { get; set; }

        [Column(TypeName = "money")]
        public decimal High { get; set; }

        [Column(TypeName = "bigint")]
        public Int64 Volume { get; set; }

        public static List<PriceVM> ToPriceVM(List<Price>? prices)
        {
            if(prices == null) { throw new NullReferenceException(); }
            var pricesVM = new List<PriceVM>();
            foreach (var price in prices)
            {
                var pricevm = new PriceVM()
                {
                    date = price.Date.ToString("yyyy-MM-dd"),
                    close = price.Close,
                    adjclose = price.AdjClose,
                    high = price.High,
                    low = price.Low,
                    open = price.Open,
                    volume = price.Volume
                };
                pricesVM.Add(pricevm);
            }
            return pricesVM;
        }
    }

    [Table("FinancialDate", Schema = "Stock")]
    public class FinancialDate
    {
        [Key]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [Required]
        public DateTime Date { get; set; }

    }

    public class PriceVM
    {
        public string date { get; set;}

        public decimal open { get; set; }
        public decimal close { get; set; }
        public decimal adjclose { get; set; }
        public decimal high { get; set; }

        public decimal low { get; set; }

        public Int64 volume { get; set; }      
    }

}
