using System;
using System.Net;

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