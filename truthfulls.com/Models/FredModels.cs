using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace truthfulls.com.FREDModels
{
    [Table("Series", Schema = "Fred")]
    public class Series
    {
        [Key]
        public required string SeriesID { get; set; }

        public required string Title {get;set;}
        public required string Frequency {get;set;}
        public required string Units {get; set;}
        public required string SeasonalAdj {get; set;}
        public required int Popularity {get; set;}
        public required string Notes {get; set;}
    }
    [Table("Observations", Schema = "Fred")]
    public class Observations
    {
        [ForeignKey("SeriesID")]
        public required Series Series { get; set; }
        public required string SeriesID;
        public required DateTime Date { get; set; }
        public required float Observation { get; set; }
    }
}
