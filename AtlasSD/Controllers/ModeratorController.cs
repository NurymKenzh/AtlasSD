using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AtlasSD.Controllers
{
    public class ModeratorController : Controller
    {
        [Authorize(Roles = "Moderator")]
        public IActionResult Index()
        {
            return View();
        }
    }
}