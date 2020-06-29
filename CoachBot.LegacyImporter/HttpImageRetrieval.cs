using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace CoachBot.LegacyImporter
{
    public static class HttpImageRetrieval
    {
        public static string GetImageAsBase64(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);
                var base64EncodedImage = Convert.ToBase64String(data);
                return "data:image/png;base64," + base64EncodedImage;
            }
        }
    }
}
