using CoachBot.Domain.Repositories;
using CoachBot.Model;
using System.Collections.Generic;

namespace CoachBot.Domain.Services
{
    public class RegionService
    {
        private readonly RegionRepository _regionRepository;

        public RegionService(RegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public List<Region> GetRegions()
        {
            return _regionRepository.GetAll();
        }

        public Region GetRegion(int id)
        {
            return _regionRepository.Get(id);
        }

        public void AddRegion(Region region)
        {
            _regionRepository.Add(region);
        }

        public void UpdateRegion(Region region)
        {
            _regionRepository.Update(region);
        }

        public void RemoveRegion(int id)
        {
            _regionRepository.Delete(id);
        }
    }
}
