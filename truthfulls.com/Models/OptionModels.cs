using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using truthfulls.com;
using truthfulls.com.OptionModels;


namespace truthfulls.com.OptionModels
{
    [Table("OptionCodes",Schema ="StockOption")]
    public class OptionCode
    {
        [Key]
        [Required]
        [Column(Order = 0, TypeName = "nvarchar(25)")]
        public required string Code { get; set; }
        
    }

    [Table("SequenceComplete",Schema = "StockOption")]
    public class SequenceComplete
    {
        [Key]
        [ForeignKey("Code")]
        public required OptionCode OptionCode { get; set; }
        public required string Code { get; set; }

        public required bool IsComplete { get; set; }
    }

    [Table("Price",Schema ="StockOption")]
    public class Price
    {
        [ForeignKey("Code")]
        [Column(Order =0,TypeName ="nvarchar(25)")]
        public required OptionCode OptionCode{ get; set; }
        public required string Code { get; set; }

        [Column(Order = 1, TypeName = "smallint")]
        public int Duration { get; set; }

        [ForeignKey("MaturityDate")]
        public required StockModels.FinancialDate FinancialDate { get; set; }
        [Column(Order = 2, TypeName = "Date")]
        public DateTime? MaturityDate { get; set; } //nullable so it doens't cascade delete with the other date foreign key (date above will cascade delete)


        [Column(Order =3, TypeName ="money")]
        public decimal Open { get; set; }

        [Column(Order = 4, TypeName = "money")]
        public decimal? Close { get; set; }

        [Column(Order = 5, TypeName = "money")]
        public decimal? Adjclose { get; set; }

        [Column(Order = 6, TypeName = "money")]
        public decimal High { get; set; }


        [Column(Order = 7, TypeName = "money")]
        public decimal Low { get; set; }

        [Column(Order = 8, TypeName = "bigint")]
        public Int64 Volume { get; set; }

        [Column(Order = 9, TypeName = "money")]
        public decimal VWAP { get; set; }

    }

    public class OptionPriceVM
    {
        public int Duration { get; set; }

        public DateTime? Maturitydate { get; set; } //nullable so it doens't cascade delete with the other date foreign key (date above will cascade delete)

        public decimal Open { get; set; }

        public decimal? Adjclose { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public Int64 Volume { get; set; }

    }

}

namespace Option.Extensions
{
    public static class ToListExtension
    {
        public static List<OptionPriceVM> ToVM(this List<Price> prices)
        {
            var vms = (from p in prices
                       select new OptionPriceVM
                       {
                           Duration = p.Duration,
                           Maturitydate =  p.MaturityDate,
                           Open = p.Open,
                           Adjclose =  p.Adjclose,
                           High =  p.High,
                           Low =  p.Low,
                           Volume =   p.Volume
                       }).ToList<OptionPriceVM>();
            return vms;
            
        }
    }
}