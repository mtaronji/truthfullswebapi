using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace truthfulls.com.Models
{
    public class Option
    {

        public string Code { get; set; } = null!;
        public string? CompanyName { get; set; }
        public string? Country { get; set; }
    }

    public class OptionPrice
    {
        public string optioncode { get; set; } = null!;
        public string maturitydate { get; set; } = null!;
        public string date { get; set; } = null!;
        public decimal open { get; set; }
        public decimal? close { get; set; }
        public decimal? adjclose { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public Int64 volume { get; set; }
        public decimal vwap { get; set; }
        public Int64 duration { get; set; }
    }

}
