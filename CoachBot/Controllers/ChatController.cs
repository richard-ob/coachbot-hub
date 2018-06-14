using CoachBot.Model;
using CoachBot.Services.Matchmaker;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Controllers
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public string Get()
        {
            return "yep";
        }

        [HttpPost]
        public void Say([FromBody]ChatMessage message)
        {
            _chatService.Say(message.Message);
        }
    }

}
