﻿using CoachBot.Database;
using CoachBot.Model;
using CoachBot.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CoachBot.Domain.Services
{
    public class ServerService
    {
        private readonly CoachBotContext _coachBotContext;

        public ServerService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public List<Server> GetServers(bool activeOnly = true)
        {
            return _coachBotContext.Servers
                .Include(s => s.Region)
                .Include(s => s.Country)
                .Where(s => s.IsActive == true)
                .OrderBy(s => s.RegionId)
                .ThenBy(s => s.Name)
                .ToList();
        }

        public List<Server> GetDeactivatedServers()
        {
            return _coachBotContext.Servers
                .Include(s => s.Region)
                .Include(s => s.Country)
                .Where(s => s.IsActive == false)
                .OrderBy(s => s.RegionId)
                .ThenBy(s => s.Name)
                .ToList();
        }

        public List<Server> GetServersByRegion(int regionId)
        {
            return _coachBotContext.Servers
                .Include(s => s.Country)
                .Include(s => s.Region)
                .Where(s => s.RegionId == regionId)
                .ToList();
        }

        public Server GetServer(int id)
        {
            return _coachBotContext.Servers
                .Include(s => s.Country)
                .Include(r => r.Region)
                .FirstOrDefault(s => s.Id == id);
        }

        public void AddServer(Server server)
        {
            if (!ServerAddressHelper.IsValidAddress(server.Address))
            {
                throw new Exception("The server address provided is not a valid IPv4 IP or hostname, and port");
            }

            _coachBotContext.Servers.Add(server);
            _coachBotContext.SaveChanges();
        }

        public void UpdateServer(Server server)
        {
            var currentServer = _coachBotContext.Servers.Single(s => s.Id == server.Id);
            currentServer.Name = server.Name;
            currentServer.RegionId = server.RegionId;
            _coachBotContext.SaveChanges();
        }

        public void UpdateServerRconPassword(int id, string rconPassword)
        {
            var currentServer = _coachBotContext.Servers.Single(s => s.Id == id);
            currentServer.RconPassword = rconPassword;
            _coachBotContext.SaveChanges();
        }

        public void ReactivateServer(int serverId)
        {
            var server = _coachBotContext.Servers.Single(s => s.Id == serverId);
            server.IsActive = true;
            server.DeactivatedDate = null;
            _coachBotContext.SaveChanges();
        }

        public void RemoveServer(int id)
        {
            var server = _coachBotContext.Servers.Single(s => s.Id == id);
            server.IsActive = false;
            server.DeactivatedDate = DateTime.UtcNow;
            _coachBotContext.SaveChanges();
        }

        public bool CheckServerHasNoTournamentGamesScheduled(int serverId)
        {
            const int TOURNAMENT_GAME_BUFFER = 90;
            return _coachBotContext.Matches.Any(m => m.ServerId == serverId && m.KickOff != null && m.KickOff > DateTime.UtcNow && m.KickOff.Value.AddMinutes(-TOURNAMENT_GAME_BUFFER) < DateTime.UtcNow);
        }
    }
}