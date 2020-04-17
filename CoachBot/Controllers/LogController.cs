using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace CoachBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class LogController : Controller
    {

        public LogController() { }

        [HttpGet]
        public string Get()
        {
            var fileName = Directory.GetFiles(Environment.CurrentDirectory, "log-*.txt", SearchOption.TopDirectoryOnly).ToList().OrderByDescending(t => t).First();
            string log = "";
            using (FileStream fs = new FileStream(fileName,
                                     FileMode.Open,
                                     FileAccess.Read,
                                     FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (sr.Peek() >= 0)
                    {
                        log = sr.ReadLine() + Environment.NewLine + log;                        
                    }
                }
            }
            return log;
        }
    }
}
