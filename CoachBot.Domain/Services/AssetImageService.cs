using CoachBot.Database;
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

        public int CreateAssetImage(string base64encodedImage, string fileName, ulong discordUserId)
        {
            var playerId = _coachBotContext.Players.Where(p => p.DiscordUserId == discordUserId).Select(s => s.Id).Single();
            var currentDailyAssetCount = _coachBotContext.AssetImages.Count(a => a.PlayerId == playerId && a.CreatedDate > DateTime.Now.AddDays(-1));
            var fileSize = (Math.Floor((double)base64encodedImage.Length / 3) + 1) * 4 + 1;

            if (currentDailyAssetCount > 25)
            {
                throw new Exception("User has uploaded 25 images today already");
            }

            if (fileSize > 50000)
            {
                throw new Exception("File exceeds 500KB in size");
            }
            
            if (!Regex.IsMatch(base64encodedImage, "^data:image/(?:png)(?:;charset=utf-8)?;base64,(?:[A-Za-z0-9]|[+/])+={0,2}"))
            {
                throw new Exception("File is not a valid PNG image");
            }

            var assetImage = new AssetImage()
            {
                Base64EncodedImage = base64encodedImage,
                PlayerId = playerId,
                FileName = fileName
            };
            _coachBotContext.AssetImages.Add(assetImage);
            _coachBotContext.SaveChanges();

            return assetImage.Id;
        }

    }
}
