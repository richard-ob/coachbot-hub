using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class AssetImage
    {
        [Key]
        public int Id { get; set; }

        public string Base64EncodedImage { get; set; }

        public string FileName { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
