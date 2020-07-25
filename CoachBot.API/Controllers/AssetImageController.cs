using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using CoachBot.Models;
using CoachBot.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static CoachBot.Attributes.HubRoleAuthorizeAttribute;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/asset-images")]
    [ApiController]
    public class AssetImageController : Controller
    {
        private readonly AssetImageService _assetImageService;

        public AssetImageController(AssetImageService assetImageService)
        {
            _assetImageService = assetImageService;
        }

        [HubRolePermission(HubRole = PlayerHubRole.Player)]
        [HttpPost]
        public IActionResult CreateAssetImage(CreateAssetImageDto assetImageDto)
        {
            var fileSize = (double)assetImageDto.Base64EncodedImage.Length * 0.72; // Rough estimation of equivalent size

            if (fileSize > 250000)
            {
                return BadRequest("File exceeds 250KB in size");
            }

            if (!Regex.IsMatch(assetImageDto.Base64EncodedImage, "^data:image/(?:png)(?:;charset=utf-8)?;base64,(?:[A-Za-z0-9]|[+/])+={0,2}"))
            {
                return BadRequest("File is not a valid PNG image");
            }

            return Ok(_assetImageService.CreateAssetImage(assetImageDto.Base64EncodedImage, assetImageDto.FileName, HttpContext.User.GetSteamId()));
        }

        [HttpGet("{id}")]
        public AssetImage GetAssetImage(int id)
        {
            return _assetImageService.GetAssetImage(id);
        }

        [HttpPost("get-batch")]
        public List<AssetImage> GetAssetImages(List<int> assetImageIds)
        {
            return _assetImageService.GetAssetImages(assetImageIds);
        }
    }
}