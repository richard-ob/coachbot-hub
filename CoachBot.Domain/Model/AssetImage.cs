using CoachBot.Model;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class AssetImage
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public string Base64EncodedImage { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}