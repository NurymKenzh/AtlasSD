using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasSD.Data;
using AtlasSD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace AtlasSD.Controllers
{
    public class RegionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public RegionsController(ApplicationDbContext context, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Regions
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder, string Code, string NameEN, string NameKK, string NameRU, int? Year, int? Page, decimal? PopulationMin, decimal? PopulationMax, decimal? AreaMin, decimal? AreaMax)
        {
            var regions = _context.Region
                .Where(r => true);

            ViewBag.CodeFilter = Code;
            ViewBag.NameENFilter = NameEN;
            ViewBag.NameKKFilter = NameKK;
            ViewBag.NameRUFilter = NameRU;
            ViewBag.YearFilter = Year;
            ViewBag.PopulationMinFilter = PopulationMin;
            ViewBag.PopulationMaxFilter = PopulationMax;
            ViewBag.AreaMinFilter = AreaMin;
            ViewBag.AreaMaxFilter = AreaMax;

            ViewBag.CodeSort = SortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.PopulationSort = SortOrder == "Population" ? "PopulationDesc" : "Population";
            ViewBag.AreaSort = SortOrder == "Area" ? "AreaDesc" : "Area";

            if (!string.IsNullOrEmpty(Code))
            {
                regions = regions.Where(r => r.Code.ToLower().Contains(Code.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                regions = regions.Where(r => r.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                regions = regions.Where(r => r.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                regions = regions.Where(r => r.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (Year != null)
            {
                regions = regions.Where(r => r.Year == Year);
            }
            if (PopulationMin != null)
            {
                regions = regions.Where(r => r.Population >= PopulationMin);
            }
            if (PopulationMax != null)
            {
                regions = regions.Where(r => r.Population <= PopulationMax);
            }
            if (AreaMin != null)
            {
                regions = regions.Where(r => r.Area >= AreaMin);
            }
            if (AreaMax != null)
            {
                regions = regions.Where(r => r.Area <= AreaMax);
            }

            switch (SortOrder)
            {
                case "Code":
                    regions = regions.OrderBy(r => r.Code);
                    break;
                case "CodeDesc":
                    regions = regions.OrderByDescending(r => r.Code);
                    break;
                case "NameEN":
                    regions = regions.OrderBy(r => r.NameEN);
                    break;
                case "NameENDesc":
                    regions = regions.OrderByDescending(r => r.NameEN);
                    break;
                case "NameKK":
                    regions = regions.OrderBy(r => r.NameKK);
                    break;
                case "NameKKDesc":
                    regions = regions.OrderByDescending(r => r.NameKK);
                    break;
                case "NameRU":
                    regions = regions.OrderBy(r => r.NameRU);
                    break;
                case "NameRUDesc":
                    regions = regions.OrderByDescending(r => r.NameRU);
                    break;
                case "Year":
                    regions = regions.OrderBy(r => r.Year);
                    break;
                case "YearDesc":
                    regions = regions.OrderByDescending(r => r.Year);
                    break;
                case "Population":
                    regions = regions.OrderBy(r => r.Population);
                    break;
                case "PopulationDesc":
                    regions = regions.OrderByDescending(r => r.Population);
                    break;
                case "Area":
                    regions = regions.OrderBy(r => r.Area);
                    break;
                case "AreaDesc":
                    regions = regions.OrderByDescending(r => r.Area);
                    break;
                default:
                    regions = regions.OrderBy(r => r.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(regions.Count(), Page);

            var viewModel = new RegionIndexPageViewModel
            {
                Items = regions.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };
            
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            return View(viewModel);
        }

        // GET: Regions/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Region
                .SingleOrDefaultAsync(m => m.Id == id);
            if (region == null)
            {
                return NotFound();
            }

            return View(region);
        }
        
        // GET: Regions/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Region.SingleOrDefaultAsync(m => m.Id == id);
            if (region == null)
            {
                return NotFound();
            }
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = region.Year == i });
            return View(region);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,NameEN,NameKK,NameRU,Year,Area,Population,Coordinates")] Region region, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (id != region.Id)
            {
                return NotFound();
            }
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = region.Year == i });
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(region);
                    await _context.SaveChangesAsync();
                    KazakhstanRegionsEditByYear(region.Year);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegionExists(region.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (backlink.ToLower().Contains("region"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            return View(region);
        }

        // GET: Regions/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Region
                .SingleOrDefaultAsync(m => m.Id == id);
            if (region == null)
            {
                return NotFound();
            }

            return View(region);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id, string backlink)
        {
            ViewBag.BackLink = backlink;
            var region = await _context.Region.SingleOrDefaultAsync(m => m.Id == id);
            int year = region.Year;
            _context.Region.Remove(region);
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(backlink))
            {
                if (backlink.ToLower().Contains("region"))
                {
                    return Redirect(backlink);
                }
            }
            KazakhstanRegionsEditByYear(year);
            return RedirectToAction("Index");
        }

        // GET
        [Authorize(Roles = "Administrator, Moderator")]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public ActionResult Upload(IFormFile File,
            int Year,
            string Code,
            string NameEN,
            string NameKK,
            string NameRU,
            string Area,
            string Population,
            string JsonText, string backlink)
        {
            ViewBag.BackLink = backlink;
            try
            {
                // просмотр файла
                if (File != null)
                {
                    var jsontext = string.Empty;
                    using (var reader = new StreamReader(File.OpenReadStream()))
                    {
                        jsontext = reader.ReadToEnd();
                    }
                    dynamic map = JsonConvert.DeserializeObject<dynamic>(jsontext);
                    List<SelectListItem> properties = new List<SelectListItem>();
                    foreach (Newtonsoft.Json.Linq.JProperty property in map.features[0].properties)
                    {
                        properties.Add(new SelectListItem
                        {
                            Text = property.Name,
                            Value = property.Name
                        });
                    }
                    ViewBag.Properties = properties;
                    ViewBag.JsonText = jsontext;
                    int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
                    ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                    return View();
                }
                // загрузка данных из файла
                else
                {
                    if (string.IsNullOrEmpty(JsonText))
                    {
                        return View();
                    }
                    dynamic map = JsonConvert.DeserializeObject<dynamic>(JsonText);
                    List<Region> regions = new List<Region>();
                    for (int i = 0; i < map.features.Count; i++)
                    {
                        Region region = new Region();
                        region.Year = Year;
                        region.Area = 0;
                        region.Population = 0;
                        foreach (Newtonsoft.Json.Linq.JProperty property in map.features[i].properties)
                        {
                            if (property.Name == Code)
                            {
                                region.Code = property.Value.ToString();
                            }
                            if (property.Name == NameEN)
                            {
                                region.NameEN = property.Value.ToString();
                            }
                            if (property.Name == NameKK)
                            {
                                region.NameKK = property.Value.ToString();
                            }
                            if (property.Name == NameRU)
                            {
                                region.NameRU = property.Value.ToString();
                            }
                            if (property.Name == Area)
                            {
                                try
                                {
                                    region.Area = Convert.ToDecimal(property.Value.ToString());
                                }
                                catch
                                {
                                }
                            }
                            if (property.Name == Population)
                            {
                                try
                                {
                                    region.Population = Convert.ToInt32(property.Value.ToString());
                                }
                                catch
                                {
                                }
                            }
                        }
                        foreach (Newtonsoft.Json.Linq.JProperty geometry in map.features[i].geometry)
                        {
                            if (geometry.Name == "coordinates")
                            {
                                region.Coordinates = geometry.Value.ToString();
                            }
                        }
                        regions.Add(region);
                    }
                    _context.AddRange(regions);
                    _context.SaveChanges();
                    KazakhstanRegionsCreateByYear(regions.FirstOrDefault().Year);
                    return View();
                }
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Copy()
        {
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.YearFrom = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString()});
            ViewBag.YearTo = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return View();
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Copy(int YearFrom, int YearTo, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (_context.Region.Count(r => r.Year == YearFrom) == 0)
            {
                ViewBag.Result = _sharedLocalizer["NoDataToCopy"];
            }
            else if(_context.Region.Count(r => r.Year == YearTo) > 0)
            {
                ViewBag.Result = _sharedLocalizer["DataAlreadyExists"];
            }
            else
            {
                foreach(var region in _context.Region.Where(r => r.Year == YearFrom))
                {
                    _context.Region.Add(new Region()
                    {
                        Year = YearTo,
                        Area = region.Area,
                        Code = region.Code,
                        Coordinates = region.Coordinates,
                        NameEN = region.NameEN,
                        NameKK = region.NameKK,
                        NameRU = region.NameRU,
                        Population = region.Population
                    });
                }
                _context.SaveChanges();
                KazakhstanRegionsEditByYear(YearTo);
                if (backlink.ToLower().Contains("region"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.YearFrom = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = i == YearFrom });
            ViewBag.YearTo = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = i == YearTo });
            return View();
        }

        private bool RegionExists(int id)
        {
            return _context.Region.Any(e => e.Id == id);
        }
        
        [HttpPost]
        public JsonResult GetRegionsCountByYear(int Year)
        {
            int regionsCount = _context.Region.Count(r => r.Year == Year);
            JsonResult result = new JsonResult(regionsCount);
            return result;
        }

        public void KazakhstanRegionsCreateByYear(int Year)
        {
            int kzcount = _context.Region.AsNoTracking().Count(r => r.Year == Year && string.IsNullOrEmpty(r.Coordinates) && r.Code == Startup.Configuration["KazakhstanCode"]),
                regionscount = _context.Region.AsNoTracking().Count(r => r.Year == Year && !string.IsNullOrEmpty(r.Coordinates) && r.Code != Startup.Configuration["KazakhstanCode"]);
            if (kzcount == 0 && regionscount > 0)
            {
                Models.Region kz = new Models.Region()
                {
                    Area = _context.Region.AsNoTracking().Where(r => r.Year == Year && !string.IsNullOrEmpty(r.Coordinates) && r.Code != Startup.Configuration["KazakhstanCode"]).Sum(r => r.Area),
                    Code = Startup.Configuration["KazakhstanCode"],
                    NameEN = _sharedLocalizer.WithCulture(new CultureInfo("en"))["Kazakhstan"],
                    NameKK = _sharedLocalizer.WithCulture(new CultureInfo("kk"))["Kazakhstan"],
                    NameRU = _sharedLocalizer.WithCulture(new CultureInfo("ru"))["Kazakhstan"],
                    Population = _context.Region.AsNoTracking().Where(r => r.Year == Year && !string.IsNullOrEmpty(r.Coordinates) && r.Code != Startup.Configuration["KazakhstanCode"]).Sum(r => r.Population),
                    Year = Year
                };
                _context.Add(kz);
            }
            _context.SaveChanges();
        }

        public void KazakhstanRegionsEditByYear(int Year)
        {
            Models.Region kz = _context.Region.AsNoTracking().FirstOrDefault(r => r.Year == Year && string.IsNullOrEmpty(r.Coordinates) && r.Code == Startup.Configuration["KazakhstanCode"]);
            if (kz != null)
            {
                kz.Area = _context.Region.AsNoTracking().Where(r => r.Year == Year && !string.IsNullOrEmpty(r.Coordinates) && r.Code != Startup.Configuration["KazakhstanCode"]).Sum(r => r.Area);
                kz.Population = _context.Region.AsNoTracking().Where(r => r.Year == Year && !string.IsNullOrEmpty(r.Coordinates) && r.Code != Startup.Configuration["KazakhstanCode"]).Sum(r => r.Population);
                _context.Update(kz);
            }
            _context.SaveChanges();
        }

        public void KazakhstanValuesCreateByYearIndicator(int Year, int IndicatorId)
        {
            KazakhstanRegionsCreateByYear(Year);
            Indicator indicator = _context.Indicator.AsNoTracking().FirstOrDefault(i => i.Id == IndicatorId);
            List<IndicatorValue> indicatorValues = _context.IndicatorValue.AsNoTracking().Include(i => i.Region).Where(i => i.Region.Year == Year && i.IndicatorId == indicator.Id).ToList();
            int indicatorValueKZCount = _context.IndicatorValue
                .Include(i => i.Region)
                .Count(i => i.Region.Year == Year && i.IndicatorId == indicator.Id && string.IsNullOrEmpty(i.Region.Coordinates) && i.Region.Code == Startup.Configuration["KazakhstanCode"]);
            if (indicatorValueKZCount == 0 && indicatorValues.Count() > 0)
            {
                IndicatorValue indicatorValueKZ = new IndicatorValue()
                {
                    IndicatorId = indicator.Id,
                    RegionId = _context.Region.AsNoTracking().FirstOrDefault(r => r.Year == Year && string.IsNullOrEmpty(r.Coordinates) && r.Code == Startup.Configuration["KazakhstanCode"]).Id,
                    SourceId = indicatorValues.FirstOrDefault().SourceId,
                    Year = indicatorValues.FirstOrDefault().Region.Year,
                    Value = indicatorValues.Average(i => i.Value)
                };
                _context.Add(indicatorValueKZ);
            }
            _context.SaveChanges();
        }

        public void KazakhstanValuesEditByYearIndicator(int Year, int IndicatorId)
        {
            Indicator indicator = _context.Indicator.AsNoTracking().FirstOrDefault(i => i.Id == IndicatorId);
            List<IndicatorValue> indicatorValues = _context.IndicatorValue.AsNoTracking().Include(i => i.Region).Where(i => i.Region.Year == Year && i.IndicatorId == indicator.Id).ToList();
            IndicatorValue indicatorValueKZ = _context.IndicatorValue
                .Include(i => i.Region)
                .FirstOrDefault(i => i.Region.Year == Year && i.IndicatorId == indicator.Id && string.IsNullOrEmpty(i.Region.Coordinates) && i.Region.Code == Startup.Configuration["KazakhstanCode"]);
            if (indicatorValueKZ != null && indicatorValues.Count() > 0)
            {
                indicatorValueKZ.SourceId = indicatorValues.FirstOrDefault().SourceId;
                indicatorValueKZ.Value = indicatorValues.Average(i => i.Value);
                _context.Update(indicatorValueKZ);
            }
            _context.SaveChanges();
        }

        public void KazakhstanRegionsRename()
        {
            var regions = _context.Region.Where(r => string.IsNullOrEmpty(r.Coordinates) && r.Code == Startup.Configuration["KazakhstanCode"]);
            foreach(var region in regions)
            {
                region.NameEN = _sharedLocalizer.WithCulture(new CultureInfo("en"))["Kazakhstan"];
                region.NameKK = _sharedLocalizer.WithCulture(new CultureInfo("kk"))["Kazakhstan"];
                region.NameRU = _sharedLocalizer.WithCulture(new CultureInfo("ru"))["Kazakhstan"];
                _context.Update(region);
            }
            _context.SaveChanges();
        }
    }
}
