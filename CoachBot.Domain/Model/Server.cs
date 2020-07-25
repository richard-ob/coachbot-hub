using CoachBot.Database;
using CoachBot.Domain.Model;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    public class Server : IUserUpdateableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        [JsonIgnore]
        public string RconPassword { get; set; }

        public bool HasRconPassword => !string.IsNullOrEmpty(RconPassword);

        public int RegionId { get; set; }

        public Region Region { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? DeactivatedDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? UpdatedById { get; set; }

        public Player UpdatedBy { get; set; }
    }
}