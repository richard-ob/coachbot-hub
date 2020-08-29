using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoachBot.Model;
using Newtonsoft.Json;

namespace CoachBot.Model
{
    public class Region
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RegionId { get; set; }

        public string RegionName { get; set; }

        public string RegionCode { get; set; }

        [JsonIgnore]
        public string CreateTeamToken { get; set; }

        public Domain.Model.MatchFormat MatchFormat
        {
            get
            {
                switch (RegionCode)
                {
                    case "EU":
                        return Domain.Model.MatchFormat.EightVsEight;
                    case "SA":
                        return Domain.Model.MatchFormat.SixVsSix;
                    case "NA":
                        return Domain.Model.MatchFormat.SixVsSix;
                    default:
                        return Domain.Model.MatchFormat.EightVsEight;
                }
            }
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}