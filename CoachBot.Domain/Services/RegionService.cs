﻿using CoachBot.Database;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class RegionService
    {
        private readonly CoachBotContext _coachBotContext;

        public RegionService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<RegionDto> GetRegions()
        {
            return _coachBotContext.Regions.Select(r =>
               new RegionDto()
               {
                   RegionId = r.RegionId,
                   RegionName = r.RegionName,
                   RegionCode = r.RegionCode,
                   ServerCount = _coachBotContext.Servers.Count(s => s.RegionId == r.RegionId),
                   MatchCount = _coachBotContext.Matches.Count(m => m.Server.RegionId == r.RegionId),
                   TeamCount = _coachBotContext.Teams.Count(t => t.RegionId == r.RegionId),
                   MatchFormat = r.MatchFormat
               }
           ).ToList();
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

        public void RegenerateAuthorizationToken(int regionId)
        {
            var region = _coachBotContext.Regions.Single(r => r.RegionId == regionId);

            var token = Guid.NewGuid().ToString();
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(token);

            region.AuthorizationToken = Convert.ToBase64String(plainTextBytes);
            _coachBotContext.SaveChanges();
        }
    }
}