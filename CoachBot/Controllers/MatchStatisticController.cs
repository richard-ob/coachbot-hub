using CoachBot.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MatchStatisticController : Controller
    {
        public MatchStatisticController()
        {
        }

        [HttpPost]
        public void Submit(MatchStatisticsDto matchStatisticsDto)
        {            
            var bodyStream = new StreamReader(HttpContext.Request.Body);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            int tmpint = 2;
        }
    }
}
