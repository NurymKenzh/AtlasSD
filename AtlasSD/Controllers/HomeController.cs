using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using AtlasSD.Models;
using System.Reflection;
using AtlasSD.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AtlasSD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(ApplicationDbContext context,
            IStringLocalizer<HomeController> localizer,
            IStringLocalizer<SharedResources> sharedLocalizer,
            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
            _hostingEnvironment = hostingEnvironment;
        }

        //[Route("api/[controller]")]
        public IActionResult Index()
        {
            ViewBag.Blocs = _context.Bloc.OrderBy(b => b.Name).ToList();
            ViewBag.Groups = _context.Group.OrderBy(g => g.Name).ToList();
            ViewBag.Indicators = _context.Indicator.Where(i=>!string.IsNullOrEmpty(i.Name)).OrderBy(i => i.Name).ToList();
            //ViewBag.Maps = _context.Map.Include(i => i.Indicator).Where(m => m.Indicator.Type == Startup.Configuration["Integral"]).OrderBy(m => m.Name).ToList();

            //Indicator indicatorTest = _context.Indicator.FirstOrDefault(i => i.NameEN == "Test indicator 0"); // 100 - 1000
            //Indicator indicatorTest1 = _context.Indicator.FirstOrDefault(i => i.NameEN == "Test indicator 1"); // 20 - 70
            //Indicator indicatorTest2 = _context.Indicator.FirstOrDefault(i => i.NameEN == "Test indicator 2"); // 60 - 100
            //Indicator indicatorTest3 = _context.Indicator.FirstOrDefault(i => i.NameEN == "Test indicator 3"); // 30 - 90
            //Source source = _context.Source.FirstOrDefault();
            //List<IndicatorValue> indicatorvalues = new List<IndicatorValue>();
            //Random random = new Random();
            //for (int year = Convert.ToInt32(Startup.Configuration["YearMin"]); year <= 2016; year++)
            //{
            //    foreach (var region in _context.Region.Where(r => r.Year == year))
            //    {
            //        indicatorvalues.Add(new IndicatorValue()
            //        {
            //            IndicatorId = indicatorTest.Id,
            //            RegionId = region.Id,
            //            Year = year,
            //            Value = random.Next(0, 1000),
            //            SourceId = source.Id
            //        });
            //        indicatorvalues.Add(new IndicatorValue()
            //        {
            //            IndicatorId = indicatorTest1.Id,
            //            RegionId = region.Id,
            //            Year = year,
            //            Value = random.Next(20, 70),
            //            SourceId = source.Id
            //        });
            //        indicatorvalues.Add(new IndicatorValue()
            //        {
            //            IndicatorId = indicatorTest2.Id,
            //            RegionId = region.Id,
            //            Year = year,
            //            Value = random.Next(60, 100),
            //            SourceId = source.Id
            //        });
            //        indicatorvalues.Add(new IndicatorValue()
            //        {
            //            IndicatorId = indicatorTest3.Id,
            //            RegionId = region.Id,
            //            Year = year,
            //            Value = random.Next(30, 90),
            //            SourceId = source.Id
            //        });
            //    }
            //}
            //foreach (var ivalue in indicatorvalues)
            //{
            //    _context.IndicatorValue.Add(ivalue);
            //}
            //_context.SaveChanges();

            return View();
        }

        //[Route("api/[controller]")]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetMaps(string id)
        {
            JsonResult result = new JsonResult(null);
            Random rnd = new Random();
            if(string.IsNullOrEmpty(id))
            {
                var maps = _context.Map
                    .ToArray()
                    .OrderBy(m => rnd.Next());
                result = new JsonResult(maps);
            }
            else
            {
                if (id.Split('-')[0] == "indicator")
                {
                    var maps = _context.Map
                        .Where(m => m.IndicatorId == Convert.ToInt32(id.Split('-')[1]))
                        .ToArray()
                        .OrderBy(m => m.Name);
                    result = new JsonResult(maps);
                }
                if (id.Split('-')[0] == "group")
                {
                    var maps = _context.Map
                        .Include(m => m.Indicator)
                        .Where(m => m.Indicator.GroupId == Convert.ToInt32(id.Split('-')[1]))
                        .ToArray()
                        .OrderBy(m => m.Name);
                    result = new JsonResult(maps);
                }
                if (id.Split('-')[0] == "bloc")
                {
                    var maps = _context.Map
                        .Include(m => m.Indicator)
                        .Include(m => m.Indicator.Group)
                        .Where(m => m.Indicator.Group.BlocId == Convert.ToInt32(id.Split('-')[1]))
                        .ToArray()
                        .OrderBy(m => m.Name);
                    result = new JsonResult(maps);
                }
            }
            return result;
        }

        [HttpPost]
        public JsonResult GetIntegralMaps()
        {
            JsonResult result = new JsonResult(null);
            var maps = _context.Map.Include(i => i.Indicator).Where(m => m.Indicator.Type == Startup.Configuration["Integral"]).OrderBy(m => m.Name).ToList();
            result = new JsonResult(maps);
            return result;
        }

        public IActionResult Help()
        {
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf.RequestCulture.Culture.Name;
            ViewBag.Help = new string[] { "" };
            if(culture == "en")
            {
                string path_filename = Path.Combine(_hostingEnvironment.ContentRootPath, "Help", Path.GetFileName("HelpEN.txt"));
                ViewBag.Help = System.IO.File.ReadAllText(path_filename).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (culture == "kk")
            {
                string path_filename = Path.Combine(_hostingEnvironment.ContentRootPath, "Help", Path.GetFileName("HelpKK.txt"));
                ViewBag.Help = System.IO.File.ReadAllText(path_filename).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (culture == "ru")
            {
                string path_filename = Path.Combine(_hostingEnvironment.ContentRootPath, "Help", Path.GetFileName("HelpRU.txt"));
                ViewBag.Help = System.IO.File.ReadAllText(path_filename).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Help(string HelpText)
        {
            var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf.RequestCulture.Culture.Name;
            if (culture == "en")
            {
                string path_filename = Path.Combine(_hostingEnvironment.ContentRootPath, "Help", Path.GetFileName("HelpEN.txt"));
                System.IO.File.WriteAllText(path_filename, HelpText);
            }
            if (culture == "kk")
            {
                string path_filename = Path.Combine(_hostingEnvironment.ContentRootPath, "Help", Path.GetFileName("HelpKK.txt"));
                System.IO.File.WriteAllText(path_filename, HelpText);
            }
            if (culture == "ru")
            {
                string path_filename = Path.Combine(_hostingEnvironment.ContentRootPath, "Help", Path.GetFileName("HelpRU.txt"));
                System.IO.File.WriteAllText(path_filename, HelpText);
            }

            ViewBag.Help = new string[] { "" };
            if (culture == "en")
            {
                string path_filename = Path.Combine(_hostingEnvironment.ContentRootPath, "Help", Path.GetFileName("HelpEN.txt"));
                ViewBag.Help = System.IO.File.ReadAllText(path_filename).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (culture == "kk")
            {
                string path_filename = Path.Combine(_hostingEnvironment.ContentRootPath, "Help", Path.GetFileName("HelpKK.txt"));
                ViewBag.Help = System.IO.File.ReadAllText(path_filename).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (culture == "ru")
            {
                string path_filename = Path.Combine(_hostingEnvironment.ContentRootPath, "Help", Path.GetFileName("HelpRU.txt"));
                ViewBag.Help = System.IO.File.ReadAllText(path_filename).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return View();
        }
    }
}
