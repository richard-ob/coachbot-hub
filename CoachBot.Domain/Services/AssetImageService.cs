using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CoachBot.Domain.Services
{
    public class AssetImageService
    {
        private readonly CoachBotContext _coachBotContext;

        public AssetImageService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public AssetImage GetAssetImage(int id)
        {
            return _coachBotContext.AssetImages.Single(a => a.Id == id);
        }

        public List<AssetImage> GetAssetImages(List<int> ids)
        {
            return _coachBotContext.AssetImages.Where(a => ids.Any(i => i == a.Id)).ToList();
        }

        public int CreateAssetImage(string base64encodedImage, string fileName, ulong steamUserId)
        {
            var player = _coachBotContext.GetPlayerBySteamId(steamUserId);
            var currentDailyAssetCount = _coachBotContext.AssetImages.Count(a => a.PlayerId == player.Id && a.CreatedDate > DateTime.Now.AddDays(-1));
            var fileSize = (Math.Floor((double)base64encodedImage.Length / 3) + 1) * 4 + 1;

            if (currentDailyAssetCount > 25)
            {
                throw new Exception("User has uploaded 25 images today already");
            }

            var assetImage = new AssetImage()
            {
                Base64EncodedImage = base64encodedImage,
                PlayerId = player.Id,
                FileName = fileName
            };
            _coachBotContext.AssetImages.Add(assetImage);
            _coachBotContext.SaveChanges();

            return assetImage.Id;
        }

    }
}
