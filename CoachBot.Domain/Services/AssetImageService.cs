using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Shared.Helpers;
using CoachBot.Shared.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class AssetImageService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly PlayerService _playerService;
        private readonly AzureAssetsConfig _azureAssetsConfig;

        public AssetImageService(CoachBotContext coachBotContext, PlayerService playerService)
        {
            _coachBotContext = coachBotContext;
            _playerService = playerService;
            _azureAssetsConfig = ConfigHelper.GetConfig().AzureAssetsConfig;
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
            var currentDailyAssetCount = _coachBotContext.AssetImages.Count(a => a.CreatedById == player.Id && a.CreatedDate > DateTime.UtcNow.AddDays(-1));
            var fileSize = (Math.Floor((double)base64encodedImage.Length / 3) + 1) * 4 + 1;

            if (currentDailyAssetCount > 10 && !_playerService.IsOwner(steamUserId))
            {
                throw new Exception("User has uploaded 10 images today already");
            }

            var assetImage = new AssetImage()
            {
                Base64EncodedImage = base64encodedImage,
                CreatedById = player.Id,
                FileName = fileName
            };
            _coachBotContext.AssetImages.Add(assetImage);
            _coachBotContext.SaveChanges();

            foreach (var assetImageSize in AssetImageSizes.AllSizes)
            {
                UploadImageToAzure(assetImage, assetImageSize.Name, assetImageSize.Width);
            }

            assetImage.Url = GenerateImageUri($"{assetImage.Id}_{AssetImageSizes.ASSET_IMAGE_REPLACEMENT_TOKEN}.png").ToString();
            _coachBotContext.SaveChanges();

            return assetImage.Id;
        }

        public void GenerateAllAssetImageUrls()
        {
            foreach(var assetImage in _coachBotContext.AssetImages)
            {
                foreach(var assetImageSize in AssetImageSizes.AllSizes)
                {
                    UploadImageToAzure(assetImage, assetImageSize.Name, assetImageSize.Width);
                }

                assetImage.Url = GenerateImageUri($"{assetImage.Id}_{AssetImageSizes.ASSET_IMAGE_REPLACEMENT_TOKEN}.png").ToString();
                _coachBotContext.SaveChanges();
            }
        }

        private void UploadImageToAzure(AssetImage assetImage, string sizeName, int? width)
        {
            byte[] bytes = Convert.FromBase64String(assetImage.Base64EncodedImage.Replace("data:image/png;base64,", ""));
            using (var outputStream = new MemoryStream())
            using (var image = Image.Load(new MemoryStream(bytes)))
            {
                if (width != null)
                {
                    int newHeight = Convert.ToInt32(Math.Floor(image.Height * CalculateNumberChange(image.Width, (int)width)));
                    image.Mutate(x => x.Resize((int)width, newHeight));
                }

                var storageCredentials = new StorageSharedKeyCredential(_azureAssetsConfig.AccountName, _azureAssetsConfig.Key);
                var blobUri = GenerateImageUri($"{assetImage.Id}_{sizeName}.png");
                var blobClient = new BlobClient(blobUri, storageCredentials);
                image.SaveAsPng(outputStream);
                outputStream.Position = 0;

                blobClient.UploadAsync(outputStream, overwrite: true).Wait();
            }
        }

        private double CalculateNumberChange(int originalNumber, int newNumber)
        {
            var change = originalNumber - newNumber;

            return (double)newNumber / originalNumber;
        }

        private Uri GenerateImageUri(string fileName)
        {
            return new Uri("https://" + _azureAssetsConfig.AccountName + ".blob.core.windows.net/" + _azureAssetsConfig.ContainerName + "/" + fileName);
        }
    }
}