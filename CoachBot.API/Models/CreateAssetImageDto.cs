using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Models
{
    public class CreateAssetImageDto
    {
        public string Base64EncodedImage { get; set; }

        public string FileName { get; set; }
    }
}
