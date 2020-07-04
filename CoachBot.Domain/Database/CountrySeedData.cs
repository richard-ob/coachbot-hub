using CoachBot.Domain.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CoachBot.Domain.Database
{
    public static class CountrySeedData
    {
        public static List<Country> GetCountries()
        {
            int currentId = 1;
            List<Country> countries = new List<Country>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo country = new RegionInfo(culture.LCID);
                if (!countries.Any(c => c.Code == country.Name))
                {
                    var dbCountry = new Country()
                    {
                        Id = currentId,
                        Name = country.EnglishName,
                        Code = country.Name
                    };
                    countries.Add(dbCountry);
                    currentId++;
                }
            }

            return countries.OrderBy(p => p.Name).ToList();
        }
    }
}