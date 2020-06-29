using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CoachBot.LegacyImporter
{
    public static class IpTools
    {
        public static string GetCountryFromIpData(string address)
        {
            string url = $"https://api.ipdata.co/{address}?api-key=d3afba425661d876906802dadaf5a6b8dffc9d1586abf33a3597ea66";
            var request = WebRequest.Create(url);
            try
            {
                using (WebResponse wrs = request.GetResponse())
                using (Stream stream = wrs.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var obj = JObject.Parse(json);
                    string countryCode = (string)obj["country_code"];

                    return countryCode;
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
