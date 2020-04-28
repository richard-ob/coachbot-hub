using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using CoachBot.Extensions;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public int CreateAssetImage(AssetImage assetImage)
        {
            return _assetImageService.CreateAssetImage(assetImage.Base64EncodedImage, assetImage.FileName, HttpContext.User.GetDiscordUserId());
        }

        [HttpGet("{id}")]
        public AssetImage GetAssetImage(int id)
        {
            return _assetImageService.GetAssetImage(id);
        }
    }
}
