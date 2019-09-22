using CoachBot.Database;
using CoachBot.Model;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Repositories
{
    public class RegionRepository
    {
        private readonly CoachBotContext _coachBotContext;

        public RegionRepository(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<Region> GetAll()
        {
            var regions = _coachBotContext.Regions.ToList();
            regions.ForEach(r => r.ServerCount = _coachBotContext.Servers.Count(s => s.RegionId == r.RegionId));

            return regions;
        }

        public Region Get(int id)
        {
            return _coachBotContext.Regions.FirstOrDefault(s => s.RegionId == id);
        }

        public void Add(Region region)
        {
            _coachBotContext.Regions.Add(region);
            _coachBotContext.SaveChanges();
        }

        public void Update(Region region)
        {
            _coachBotContext.Regions.Update(region);
            _coachBotContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var region = _coachBotContext.Regions.First(s => s.RegionId == id);
            _coachBotContext.Regions.Remove(region);
            _coachBotContext.SaveChanges();
        }
    }
}
