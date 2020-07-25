using CoachBot.Database;
using CoachBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class AssetImage: IEntity
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        public string Base64EncodedImage { get; set; }

        public string FileName { get; set; }

        [JsonIgnore]
        public string Url { get; set; }

        public string OriginalUrl => Url?.Replace(AssetImageSizes.ASSET_IMAGE_REPLACEMENT_TOKEN, AssetImageSizes.ASSET_IMAGE_SIZE_ORIGINAL.Name);
        public string LargeUrl => Url?.Replace(AssetImageSizes.ASSET_IMAGE_REPLACEMENT_TOKEN, AssetImageSizes.ASSET_IMAGE_SIZE_LARGE.Name);
        public string MediumUrl => Url?.Replace(AssetImageSizes.ASSET_IMAGE_REPLACEMENT_TOKEN, AssetImageSizes.ASSET_IMAGE_SIZE_MEDIUM.Name);
        public string SmallUrl => Url?.Replace(AssetImageSizes.ASSET_IMAGE_REPLACEMENT_TOKEN, AssetImageSizes.ASSET_IMAGE_SIZE_SMALL.Name);
        public string ExtraSmallUrl => Url?.Replace(AssetImageSizes.ASSET_IMAGE_REPLACEMENT_TOKEN, AssetImageSizes.ASSET_IMAGE_SIZE_EXTRASMALL.Name);

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public Player CreatedBy { get; set; }

    }

    public static class AssetImageSizes
    {
        public const string ASSET_IMAGE_REPLACEMENT_TOKEN = "[[SIZE]]";
        public static AssetImageSize ASSET_IMAGE_SIZE_ORIGINAL => new AssetImageSize() { Name = "orig", Width = null };
        public static AssetImageSize ASSET_IMAGE_SIZE_LARGE => new AssetImageSize() { Name = "lg", Width = 512 };
        public static AssetImageSize ASSET_IMAGE_SIZE_MEDIUM => new AssetImageSize() { Name = "md", Width = 256 };
        public static AssetImageSize ASSET_IMAGE_SIZE_SMALL => new AssetImageSize() { Name = "sm", Width = 128 };
        public static AssetImageSize ASSET_IMAGE_SIZE_EXTRASMALL => new AssetImageSize() { Name = "xs", Width = 32 };

        public static List<AssetImageSize> AllSizes => new List<AssetImageSize>()
        {
            ASSET_IMAGE_SIZE_ORIGINAL,
            ASSET_IMAGE_SIZE_LARGE,
            ASSET_IMAGE_SIZE_MEDIUM,
            ASSET_IMAGE_SIZE_SMALL,
            ASSET_IMAGE_SIZE_EXTRASMALL
        };
    }

    public struct AssetImageSize
    {
        public string Name { get; set; }

        public int? Width { get; set; }
    }
}