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
using MoreLinq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;
using System.Drawing;
//using System.Web;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.DataValidation;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Authentication;
using System.Globalization;

namespace AtlasSD.Controllers
{
    public class IndicatorValuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public IndicatorValuesController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: IndicatorValues
        //[Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder, int? SourceId, string Type, int? BlocId, int? GroupId, int? IndicatorId, int? Year, string RegionCode, int? Page)
        {
            var indicatorvalues = _context.IndicatorValue
                .Include(i => i.Indicator)
                .Include(i => i.Indicator.Group)
                .Include(i => i.Indicator.Group.Bloc)
                .Include(i => i.Region)
                .Include(i => i.Source)
                .Where(i => true);

            ViewBag.SourceIdFilter = SourceId;
            ViewBag.TypeFilter = Type;
            ViewBag.BlocIdFilter = BlocId;
            ViewBag.GroupIdFilter = GroupId;
            ViewBag.IndicatorIdFilter = IndicatorId;
            ViewBag.YearFilter = Year;
            ViewBag.RegionCodeFilter = RegionCode;

            ViewBag.SourceSort = SortOrder == "Source" ? "SourceDesc" : "Source";
            ViewBag.TypeSort = SortOrder == "Type" ? "TypeDesc" : "Type";
            ViewBag.BlocSort = SortOrder == "Bloc" ? "BlocDesc" : "Bloc";
            ViewBag.GroupSort = SortOrder == "Group" ? "GroupDesc" : "Group";
            ViewBag.IndicatorSort = SortOrder == "Indicator" ? "IndicatorDesc" : "Indicator";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.RegionSort = SortOrder == "Region" ? "RegionDesc" : "Region";
            ViewBag.ValueSort = SortOrder == "Value" ? "ValueDesc" : "Value";

            if (SourceId != null)
            {
                indicatorvalues = indicatorvalues.Where(i => i.SourceId == SourceId);
            }
            if (!string.IsNullOrEmpty(Type))
            {
                indicatorvalues = indicatorvalues.Where(i => i.Indicator.Type == Type);
            }
            if (BlocId != null)
            {
                indicatorvalues = indicatorvalues.Where(i => i.Indicator.Group.BlocId == BlocId);
            }
            if (GroupId != null)
            {
                indicatorvalues = indicatorvalues.Where(i => i.Indicator.GroupId == GroupId);
            }
            if (IndicatorId != null)
            {
                indicatorvalues = indicatorvalues.Where(i => i.IndicatorId == IndicatorId);
            }
            if (Year != null)
            {
                indicatorvalues = indicatorvalues.Where(i => i.Year == Year);
            }
            if (!string.IsNullOrEmpty(RegionCode))
            {
                indicatorvalues = indicatorvalues.Where(i => i.Region.Code == RegionCode);
            }

            switch (SortOrder)
            {
                case "Source":
                    indicatorvalues = indicatorvalues.OrderBy(i => i.Source == null ? "" : i.Source.Name);
                    break;
                case "SourceDesc":
                    indicatorvalues = indicatorvalues.OrderByDescending(i => i.Source == null ? "" : i.Source.Name);
                    break;
                case "Type":
                    indicatorvalues = indicatorvalues.OrderBy(i => i.Indicator.Type);
                    break;
                case "TypeDesc":
                    indicatorvalues = indicatorvalues.OrderByDescending(i => i.Indicator.Type);
                    break;
                case "Bloc":
                    indicatorvalues = indicatorvalues.OrderBy(i => i.Indicator.Group == null ? "" : i.Indicator.Group.Bloc.Name);
                    break;
                case "BlocDesc":
                    indicatorvalues = indicatorvalues.OrderByDescending(i => i.Indicator.Group == null ? "" : i.Indicator.Group.Bloc.Name);
                    break;
                case "Group":
                    indicatorvalues = indicatorvalues.OrderBy(i => i.Indicator.Group == null ? "" : i.Indicator.Group.Name);
                    break;
                case "GroupDesc":
                    indicatorvalues = indicatorvalues.OrderByDescending(i => i.Indicator.Group == null ? "" : i.Indicator.Group.Name);
                    break;
                case "Indicator":
                    indicatorvalues = indicatorvalues.OrderBy(i => i.Indicator.Name);
                    break;
                case "IndicatorDesc":
                    indicatorvalues = indicatorvalues.OrderByDescending(i => i.Indicator.Name);
                    break;
                case "Year":
                    indicatorvalues = indicatorvalues.OrderBy(i => i.Year);
                    break;
                case "YearDesc":
                    indicatorvalues = indicatorvalues.OrderByDescending(i => i.Year);
                    break;
                case "Region":
                    indicatorvalues = indicatorvalues.OrderBy(i => i.Region.Name);
                    break;
                case "RegionDesc":
                    indicatorvalues = indicatorvalues.OrderByDescending(i => i.Region.Name);
                    break;
                case "Value":
                    indicatorvalues = indicatorvalues.OrderBy(i => i.Value);
                    break;
                case "ValueDesc":
                    indicatorvalues = indicatorvalues.OrderByDescending(i => i.Value);
                    break;
                default:
                    indicatorvalues = indicatorvalues.OrderBy(i => i.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(indicatorvalues.Count(), Page);

            var viewModel = new IndicatorValueIndexPageViewModel
            {
                Items = indicatorvalues.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            ViewBag.SourceId = new SelectList(_context.Source.OrderBy(s => s.Name), "Id", "Name");
            ViewBag.BlocId = new SelectList(_context.Bloc.OrderBy(b => b.Name), "Id", "Name");
            ViewBag.GroupId = new SelectList(_context.Group.OrderBy(g => g.Name), "Id", "Name");
            ViewBag.IndicatorId = new SelectList(_context.Indicator.OrderBy(g => g.Name), "Id", "Name");
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            ViewBag.RegionCode = new SelectList(_context.Region.OrderBy(r => r.Name).DistinctBy(r => r.Code), "Code", "NameCode");
            string[] indicatorTypes = new string[3] { Startup.Configuration["Regular"], Startup.Configuration["Calculated"], Startup.Configuration["Integral"] };
            ViewData["IndicatorTypes"] = new SelectList(indicatorTypes.Select((text, index) => new SelectListItem { Text = _sharedLocalizer[text], Value = text }), "Value", "Text").OrderBy(s => s.Text);

            return View(viewModel);
        }

        // GET: IndicatorValues/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indicatorValue = await _context.IndicatorValue
                .Include(i => i.Indicator)
                .Include(i => i.Indicator.Group)
                .Include(i => i.Indicator.Group.Bloc)
                .Include(i => i.Region)
                .Include(i => i.Source)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (indicatorValue == null)
            {
                return NotFound();
            }

            return View(indicatorValue);
        }

        // GET: IndicatorValues/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult CreateEditSelect()
        {
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).OrderBy(g => g.NameCode).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            ViewData["SourceId"] = new SelectList(_context.Source.OrderBy(s => s.Name), "Id", "Name");
            return View();
        }

        // POST: IndicatorValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> CreateEditSelect(IndicatorValueViewModel indicatorValueViewModel, string backlink)
        {
            ViewBag.BackLink = backlink;
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", indicatorValueViewModel.BlocId);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode", indicatorValueViewModel.GroupId);
            ViewData["SourceId"] = new SelectList(_context.Source.OrderBy(s => s.Name), "Id", "Name", indicatorValueViewModel.SourceId);
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).OrderBy(g => g.NameCode).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString(), Selected = i.Id == indicatorValueViewModel.IndicatorId });
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            if (ModelState.IsValid)
            {
                indicatorValueViewModel.Bloc = _context.Bloc.FirstOrDefault(b => b.Id == indicatorValueViewModel.BlocId);
                indicatorValueViewModel.Group = _context.Group.FirstOrDefault(b => b.Id == indicatorValueViewModel.GroupId);
                indicatorValueViewModel.Indicator = _context.Indicator.FirstOrDefault(i => i.Id == indicatorValueViewModel.IndicatorId);
                indicatorValueViewModel.Source = _context.Source.FirstOrDefault(s => s.Id == indicatorValueViewModel.SourceId);
                indicatorValueViewModel.IndicatorValues = _context.IndicatorValue
                    .Where(i => i.IndicatorId == indicatorValueViewModel.IndicatorId && i.Year == indicatorValueViewModel.Year)
                    .Include(i => i.Region)
                    .OrderBy(i => i.Region.Name)
                    .ToList();
                for (int i = 0; i < indicatorValueViewModel.IndicatorValues.Count; i++)
                {
                    indicatorValueViewModel.IndicatorValues[i].Region = _context.Region
                        .FirstOrDefault(r => r.Id == indicatorValueViewModel.IndicatorValues[i].RegionId);
                    indicatorValueViewModel.IndicatorValues[i].SourceId = indicatorValueViewModel.SourceId;
                }
                if (indicatorValueViewModel.IndicatorValues.Count == 0)
                {
                    foreach (Models.Region region in _context.Region.Where(r => r.Year == indicatorValueViewModel.Year).ToArray().OrderBy(r => r.Name))
                    {
                        indicatorValueViewModel.IndicatorValues.Add(new IndicatorValue()
                        {
                            IndicatorId = (int)indicatorValueViewModel.IndicatorId,
                            RegionId = region.Id,
                            Region = region,
                            Year = indicatorValueViewModel.Year,
                            SourceId = indicatorValueViewModel.SourceId
                        });
                    }
                }
                return View("CreateEdit", indicatorValueViewModel);
            }
            return View(indicatorValueViewModel);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult CreateEdit()
        {
            return View();
        }

        // POST: IndicatorValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> CreateEdit(IndicatorValueViewModel indicatorValueViewModel, string backlink)
        {
            ViewBag.BackLink = backlink;
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", indicatorValueViewModel.BlocId);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode", indicatorValueViewModel.GroupId);
            ViewData["SourceId"] = new SelectList(_context.Source.OrderBy(s => s.Name), "Id", "Name", indicatorValueViewModel.SourceId);
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString(), Selected = i.Id == indicatorValueViewModel.IndicatorId });
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            if (ModelState.IsValid && indicatorValueViewModel.IndicatorValues != null)
            {
                bool create = true;
                foreach (IndicatorValue indicatorValue in indicatorValueViewModel.IndicatorValues)
                {
                    if (_context.IndicatorValue.AsNoTracking().Any(i => i.Id == indicatorValue.Id))
                    {
                        _context.Update(indicatorValue);
                        create = false;
                    }
                    else
                    {
                        _context.IndicatorValue.Add(indicatorValue);
                    }
                    _context.SaveChanges();
                }
                if(create)
                {
                    int regionId = indicatorValueViewModel.IndicatorValues.FirstOrDefault().RegionId;
                    Models.Region region = _context.Region.AsNoTracking().FirstOrDefault(r => r.Id == regionId);
                    KazakhstanValuesCreateByYearIndicator(region.Year, indicatorValueViewModel.IndicatorValues.FirstOrDefault().IndicatorId);
                }
                else
                {
                    int regionId = indicatorValueViewModel.IndicatorValues.FirstOrDefault().RegionId;
                    Models.Region region = _context.Region.AsNoTracking().FirstOrDefault(r => r.Id == regionId);
                    KazakhstanValuesEditByYearIndicator(region.Year, indicatorValueViewModel.IndicatorValues.FirstOrDefault().IndicatorId);
                }
                new IndicatorsController(_context, _sharedLocalizer).CalculateIndicatorsValues((int)indicatorValueViewModel.IndicatorId);
                if (backlink.ToLower().Contains("indicatorvalue"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            else
            {
                indicatorValueViewModel.Bloc = _context.Bloc.FirstOrDefault(b => b.Id == indicatorValueViewModel.BlocId);
                indicatorValueViewModel.Group = _context.Group.FirstOrDefault(b => b.Id == indicatorValueViewModel.GroupId);
                indicatorValueViewModel.Indicator = _context.Indicator.FirstOrDefault(i => i.Id == indicatorValueViewModel.IndicatorId);
                indicatorValueViewModel.Source = _context.Source.FirstOrDefault(s => s.Id == indicatorValueViewModel.SourceId);
                if (indicatorValueViewModel.IndicatorValues == null)
                {
                    indicatorValueViewModel.IndicatorValues = new List<IndicatorValue>();
                }

                for (int i = 0; i < indicatorValueViewModel.IndicatorValues.Count; i++)
                {
                    indicatorValueViewModel.IndicatorValues[i].Region = _context.Region.FirstOrDefault(r => r.Id == indicatorValueViewModel.IndicatorValues[i].RegionId);
                }
                return View(indicatorValueViewModel);
            }


        }

        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult DownloadForm()
        {
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DownloadForm(IndicatorValueViewModel indicatorValueViewModel, string backlink)
        {
            ViewBag.BackLink = backlink;
            string error = null;
            try
            {
                if (ModelState.IsValid)
                {

                    string sContentRootPath = _hostingEnvironment.ContentRootPath;
                    sContentRootPath = Path.Combine(sContentRootPath, "Download");
                    sContentRootPath = Path.Combine(sContentRootPath, "IndicatorValuesForm");
                    DirectoryInfo di = new DirectoryInfo(sContentRootPath);
                    foreach (FileInfo filed in di.GetFiles())
                    {
                        try
                        {
                            filed.Delete();
                        }
                        catch
                        {
                        }
                    }
                    Indicator indicator = _context.Indicator.FirstOrDefault(i => i.Id == indicatorValueViewModel.IndicatorId);
                    Group group = _context.Group.FirstOrDefault(g => g.Id == indicatorValueViewModel.GroupId);
                    Bloc bloc = _context.Bloc.FirstOrDefault(b => b.Id == indicatorValueViewModel.BlocId);
                    string sFileName = $"{indicator.Name}.xlsx";
                    FileInfo file = new FileInfo(Path.Combine(sContentRootPath, sFileName));
                    if (file.Exists)
                    {
                        file.Delete();
                        file = new FileInfo(Path.Combine(sContentRootPath, sFileName));
                    }
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        for (int year = Convert.ToInt32(Startup.Configuration["YearMin"]); year <= DateTime.Now.Year; year++)
                        {
                            var regions = _context.Region.Where(r => r.Year == year && r.Code != Startup.Configuration["KazakhstanCode"]).OrderBy(r =>
                                r.NameRU.Contains("город Алматы") ? "яя" :
                                r.NameRU.Contains("город Астана") ? "яю" :
                                r.NameRU.Contains("Кызылординская область") ? "Костанайская область" :
                                r.NameRU.Contains("Костанайская область") ? "Кызылординская область" :
                                r.Name
                            ).ToList();
                            var indicatorValues = _context.IndicatorValue
                                .Where(i => i.Year == year && i.IndicatorId == indicatorValueViewModel.IndicatorId)
                                .Include(i => i.Source)
                                .ToList();
                            if (regions.Count() > 0)
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(year.ToString());
                                worksheet.Cells[1, 1].Value = _sharedLocalizer["Bloc"];
                                worksheet.Cells[1, 2].Value = bloc.Name;
                                worksheet.Cells[1, 3].Value = bloc.Id;
                                worksheet.Cells[1, 3].Style.Font.Color.SetColor(Color.White);
                                worksheet.Cells[2, 1].Value = _sharedLocalizer["Group"];
                                worksheet.Cells[2, 2].Value = group.Name;
                                worksheet.Cells[2, 3].Value = group.Id;
                                worksheet.Cells[2, 3].Style.Font.Color.SetColor(Color.White);
                                worksheet.Cells[3, 1].Value = _sharedLocalizer["Indicator"];
                                worksheet.Cells[3, 2].Value = indicator.Name;
                                worksheet.Cells[3, 3].Value = indicator.Id;
                                worksheet.Cells[3, 3].Style.Font.Color.SetColor(Color.White);
                                worksheet.Cells[4, 1].Value = _sharedLocalizer["Year"];
                                worksheet.Cells[4, 2].Value = year;

                                var sources = _context.Source.OrderBy(s => s.Name).ToList();
                                for (int i = 0; i < sources.Count; i++)
                                {
                                    worksheet.Cells[i + 1, 4].Value = sources[i].Name;
                                    worksheet.Cells[i + 1, 4].Style.Font.Color.SetColor(Color.White);
                                    worksheet.Cells[i + 1, 5].Value = sources[i].Id;
                                    worksheet.Cells[i + 1, 5].Style.Font.Color.SetColor(Color.White);
                                }
                                worksheet.Cells[5, 1].Value = _sharedLocalizer["Source"];
                                var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[5, 2].Address);
                                validation.ShowErrorMessage = true;
                                validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                                var formula = $"=$D$1:$D${sources.Count.ToString()}";
                                validation.Formula.ExcelFormula = formula;
                                IndicatorValue indicatorValueYear = indicatorValues.FirstOrDefault(i => i.Year == year);
                                worksheet.Cells[5, 2].Value = indicatorValueYear == null ? sources[0].Name : indicatorValueYear.Source.Name;
                                worksheet.Cells[5, 2].Style.Locked = false;
                                worksheet.Cells[5, 3].Formula = $"=INDEX(E1:E{sources.Count.ToString()},MATCH(B5,D1:D{sources.Count.ToString()}))";
                                worksheet.Cells[5, 3].Style.Font.Color.SetColor(Color.White);

                                worksheet.Cells[6, 1].Value = _sharedLocalizer["Code"];
                                worksheet.Cells[6, 1].Style.Font.Bold = true;
                                worksheet.Cells[6, 2].Value = _sharedLocalizer["Region"];
                                worksheet.Cells[6, 2].Style.Font.Bold = true;
                                worksheet.Cells[6, 3].Value = _sharedLocalizer["Value"];
                                worksheet.Cells[6, 3].Style.Font.Bold = true;
                                for (int r = 0; r < regions.Count(); r++)
                                {
                                    worksheet.Cells[r + 7, 1].Value = regions[r].Code;
                                    worksheet.Cells[r + 7, 2].Value = regions[r].Name;
                                    IndicatorValue indicatorValue = indicatorValues.FirstOrDefault(i => i.RegionId == regions[r].Id);
                                    if (indicatorValue != null)
                                    {
                                        worksheet.Cells[r + 7, 3].Value = indicatorValue.Value;
                                    }
                                    else
                                    {
                                        worksheet.Cells[r + 7, 3].Value = null;
                                    }
                                    worksheet.Cells[r + 7, 3].Style.Locked = false;
                                }
                                worksheet.Column(1).AutoFit();
                                worksheet.Column(2).AutoFit();
                                worksheet.Protection.IsProtected = true;
                            }
                        }
                        package.Save();
                    }
                    var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(sContentRootPath, file.Name));
                    return File(fileBytes, mimeType, sFileName);
                }
            }
            catch(Exception e)
            {
                error = e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                ViewBag.Error = error;
            }
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", indicatorValueViewModel.BlocId);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode", indicatorValueViewModel.GroupId);
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString(), Selected = i.Id == indicatorValueViewModel.IndicatorId });
            return View(indicatorValueViewModel);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UploadForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> UploadForm(IEnumerable<IFormFile> Files, string backlink)
        {
            ViewBag.BackLink = backlink;
            string error = null;
            try
            {
                string sContentRootPath = _hostingEnvironment.ContentRootPath;
                sContentRootPath = Path.Combine(sContentRootPath, "Upload");
                sContentRootPath = Path.Combine(sContentRootPath, "IndicatorValuesForm");
                DirectoryInfo di = new DirectoryInfo(sContentRootPath);
                foreach (FileInfo filed in di.GetFiles())
                {
                    try
                    {
                        filed.Delete();
                    }
                    catch
                    {
                    }
                }
                foreach (IFormFile file in Files)
                {
                    string path_filename = Path.Combine(sContentRootPath, Path.GetFileName(file.FileName));
                    using (var stream = new FileStream(Path.GetFullPath(path_filename), FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                List<IndicatorValue> indicatorValues = new List<IndicatorValue>();
                List<int> indicatorIds = new List<int>();
                foreach (IFormFile file in Files)
                {
                    string path_filename = Path.Combine(sContentRootPath, Path.GetFileName(file.FileName));
                    FileInfo fileinfo = new FileInfo(Path.Combine(sContentRootPath, Path.GetFileName(path_filename)));
                    int indicatorIdCommon = 0;
                    using (ExcelPackage package = new ExcelPackage(fileinfo))
                    {
                        foreach (var worksheet in package.Workbook.Worksheets)
                        {
                            int indicatorId = Convert.ToInt32(worksheet.Cells[3, 3].Value),
                                year = Convert.ToInt32(worksheet.Cells[4, 2].Value),
                                sourceId = Convert.ToInt32(worksheet.Cells[5, 3].Value);
                            indicatorIdCommon = indicatorId;
                            for (int i = 7; ; i++)
                            {
                                if (worksheet.Cells[i, 1].Value == null)
                                {
                                    break;
                                }
                                string regionCode = worksheet.Cells[i, 1].Value.ToString();
                                Models.Region region = _context.Region.FirstOrDefault(r => r.Code == regionCode && r.Year == year);
                                IndicatorValue indicatorValue = _context.IndicatorValue.FirstOrDefault(iv => iv.RegionId == region.Id && iv.IndicatorId == indicatorId);
                                if (indicatorValue != null)
                                {
                                    indicatorValue.Value = Convert.ToDecimal(worksheet.Cells[i, 3].Value);
                                    indicatorValue.SourceId = sourceId;
                                    indicatorValues.Add(indicatorValue);
                                }
                                else if (worksheet.Cells[i, 3].Value != null)
                                {
                                    indicatorValues.Add(new IndicatorValue()
                                    {
                                        SourceId = sourceId,
                                        IndicatorId = indicatorId,
                                        RegionId = region.Id,
                                        Value = Convert.ToDecimal(worksheet.Cells[i, 3].Value),
                                        Year = year
                                    });
                                }
                            }
                        }
                    }
                    indicatorIds.Add(indicatorIdCommon);
                }
                int add = 0,
                    change = 0;
                foreach (IndicatorValue indicatorValue in indicatorValues)
                {
                    if (indicatorValue.Id == 0)
                    {
                        _context.IndicatorValue.Add(indicatorValue);
                        add++;
                    }
                    else
                    {
                        _context.Update(indicatorValue);
                        change++;
                    }
                }
                if (add > 0)
                {
                    int regionId = indicatorValues.FirstOrDefault().RegionId;
                    Models.Region region = _context.Region.AsNoTracking().FirstOrDefault(r => r.Id == regionId);
                    KazakhstanValuesCreateByYearIndicator(region.Year, indicatorValues.FirstOrDefault().IndicatorId);
                }
                else
                {
                    int regionId = indicatorValues.FirstOrDefault().RegionId;
                    Models.Region region = _context.Region.AsNoTracking().FirstOrDefault(r => r.Id == regionId);
                    KazakhstanValuesEditByYearIndicator(region.Year, indicatorValues.FirstOrDefault().IndicatorId);
                }
                _context.SaveChanges();
                foreach (int indicatorId in indicatorIds)
                {
                    new IndicatorsController(_context, _sharedLocalizer).CalculateIndicatorsValues(indicatorId);
                }
                ViewBag.Report = $"{_sharedLocalizer["Added"]} {add.ToString()}. {_sharedLocalizer["Changed"]} {change.ToString()}.";
            }
            catch(Exception e)
            {
                error = e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                ViewBag.Error = error;
            }
            return View();
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public ActionResult Parse()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult> Parse(string Url,
            int? SourceId,
            int? IndicatorId,
            int? Year,
            string Region,
            string Value,
            string JsonText, string backlink)
        {
            ViewBag.BackLink = backlink;
            try
            {
                // view data
                if (!string.IsNullOrEmpty(Url))
                {
                    if (Url.Last() == '/')
                    {
                        Url = Url.Remove(Url.Length - 1, 1);
                    }
                    Url += "?source={\"size\":1000}";
                    var jsontext = string.Empty;
                    using (var handler = new HttpClientHandler())
                    {
                        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                        handler.SslProtocols = SslProtocols.Tls12;
                        handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
                        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                        using (var client = new HttpClient(handler))
                        {
                            var response = client.GetAsync(Url).GetAwaiter().GetResult();
                            response.EnsureSuccessStatusCode();
                            jsontext = await response.Content.ReadAsStringAsync();
                        }
                    }
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(jsontext);
                    List<SelectListItem> properties = new List<SelectListItem>();
                    foreach (Newtonsoft.Json.Linq.JObject jobject in data)
                    {
                        foreach (Newtonsoft.Json.Linq.JProperty property in jobject.Children())
                        {
                            properties.Add(new SelectListItem
                            {
                                Text = property.Name,
                                Value = property.Name
                            });
                        }
                        break;
                    }
                    ViewBag.JsonText = jsontext;
                    ViewBag.Properties = properties;
                    ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
                    ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
                    ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).OrderBy(g => g.NameCode).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
                    int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
                    ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                    ViewData["SourceId"] = new SelectList(_context.Source.OrderBy(s => s.Name), "Id", "Name");
                    return View();
                }
                // downloading data from the site
                else
                {
                    if (Year != null)
                    {
                        if (SourceId == null)
                        {
                            ViewBag.Error = _sharedLocalizer["NoSource"];
                            return View();
                        }
                        if (IndicatorId == null)
                        {
                            ViewBag.Error = _sharedLocalizer["NoIndicator"];
                            return View();
                        }
                    }
                    if (string.IsNullOrEmpty(JsonText))
                    {
                        return View();
                    }
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(JsonText);
                    List<IndicatorValue> newIndicatorValues = new List<IndicatorValue>();
                    List<string> Test = new List<string>();
                    foreach (var region in _context.Region.Where(r => r.Year == Year && r.Code != Startup.Configuration["KazakhstanCode"]))
                    {
                        IndicatorValue newIndicatorValue = new IndicatorValue();
                        string regionName = region.NameRU,
                            jsonName = "",
                            remove = " область",
                            jsonNameTest = "";
                        if(JsonText.Contains("қ"))
                        {
                            regionName = region.NameKK;
                            remove = " облысы";
                        }
                        double maxcoincidence = 0;
                        decimal? value = null;
                        if (regionName.IndexOf(remove) >= 0)
                        {
                            regionName = regionName.Remove(regionName.IndexOf(remove), remove.Length);
                        }
                        foreach (Newtonsoft.Json.Linq.JObject jobject in data)
                        {
                            foreach (Newtonsoft.Json.Linq.JProperty property in jobject.Children())
                            {
                                if (property.Name == Region)
                                {
                                    jsonName = property.Value.ToString();
                                    if (jsonName.IndexOf(remove) >= 0)
                                    {
                                        jsonName = jsonName.Remove(jsonName.IndexOf(remove), remove.Length);
                                    }
                                }
                                if (property.Name == Value)
                                {
                                    try
                                    {
                                        value = Convert.ToDecimal(property.Value.ToString(), CultureInfo.InvariantCulture);
                                    }
                                    catch
                                    {
                                        value = null;
                                    }
                                }
                            }
                            // find such a region
                            double coincidence = StringCompare(regionName, jsonName);
                            if (coincidence > maxcoincidence)
                            {
                                maxcoincidence = coincidence;
                                newIndicatorValue.IndicatorId = (int)IndicatorId;
                                newIndicatorValue.RegionId = region.Id;
                                newIndicatorValue.SourceId = SourceId;
                                newIndicatorValue.Year = (int)Year;
                                newIndicatorValue.Region = region;
                                newIndicatorValue.Value = value;
                                jsonNameTest = jsonName;
                            }
                        }
                        Test.Add($"{newIndicatorValue.Region.NameKK} - {jsonNameTest}");
                        newIndicatorValues.Add(newIndicatorValue);
                    }
                    int added = 0,
                        edited = 0;
                    foreach(var newIndicatorValue in newIndicatorValues)
                    {
                        IndicatorValue existIndicatorValue = _context.IndicatorValue.FirstOrDefault(i => i.IndicatorId == newIndicatorValue.IndicatorId
                            && i.RegionId == newIndicatorValue.RegionId
                            && i.Year == newIndicatorValue.Year);
                        if (existIndicatorValue != null)
                        {
                            newIndicatorValue.Id = existIndicatorValue.Id;
                            existIndicatorValue.Value = newIndicatorValue.Value;
                            _context.Update(existIndicatorValue);
                            edited++;
                        }
                        else
                        {
                            _context.Add(newIndicatorValue);
                            added++;
                        }
                    }
                    if (added > 0)
                    {
                        int regionId = newIndicatorValues.FirstOrDefault().RegionId;
                        Models.Region region = _context.Region.AsNoTracking().FirstOrDefault(r => r.Id == regionId);
                        KazakhstanValuesCreateByYearIndicator(region.Year, newIndicatorValues.FirstOrDefault().IndicatorId);
                    }
                    else
                    {
                        int regionId = newIndicatorValues.FirstOrDefault().RegionId;
                        Models.Region region = _context.Region.AsNoTracking().FirstOrDefault(r => r.Id == regionId);
                        KazakhstanValuesEditByYearIndicator(region.Year, newIndicatorValues.FirstOrDefault().IndicatorId);
                    }
                    _context.SaveChanges();
                    ViewBag.Report = $"{_sharedLocalizer["Added"]}: {added}, {_sharedLocalizer["Changed"]}: {edited}.";
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
        }

        // GET: IndicatorValues/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete()
        {
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewBag.IndicatorId = _context.Indicator.Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return View();
        }

        // POST: IndicatorValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(IndicatorValueViewModel indicatorValueViewModel, string backlink)
        {
            ViewBag.BackLink = backlink;
            _context.IndicatorValue.RemoveRange(_context.IndicatorValue.Where(i => i.IndicatorId == indicatorValueViewModel.IndicatorId && i.Year == indicatorValueViewModel.Year));
            await _context.SaveChangesAsync();

            new IndicatorsController(_context, _sharedLocalizer).CalculateIndicatorsValues((int)indicatorValueViewModel.IndicatorId);
            if (!string.IsNullOrEmpty(backlink))
            {
                if (backlink.ToLower().Contains("indicatorvalue"))
                {
                    return Redirect(backlink);
                }
            }

            return RedirectToAction("Index");
        }

        private bool IndicatorValueExists(int id)
        {
            return _context.IndicatorValue.Any(e => e.Id == id);
        }
        
        public IActionResult Reports()
        {
            return View();
        }
        
        public IActionResult ReportYears()
        {
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
            ViewBag.Regions = new SelectList(_context.Region.Where(r => r.Code != "00").OrderBy(r => r.NameCode).DistinctBy(r => r.NameCode), "Code", "NameCode");
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Years = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportYears(IndicatorValueViewModel indicatorValueViewModel, string backlink)
        {
            ViewBag.BackLink = backlink;
            string error = null;
            try
            {
                if (ModelState.IsValid)
                {
                    string sContentRootPath = _hostingEnvironment.ContentRootPath;
                    sContentRootPath = Path.Combine(sContentRootPath, "Download");
                    sContentRootPath = Path.Combine(sContentRootPath, "Report");
                    DirectoryInfo di = new DirectoryInfo(sContentRootPath);
                    foreach (FileInfo filed in di.GetFiles())
                    {
                        try
                        {
                            filed.Delete();
                        }
                        catch
                        {
                        }
                    }
                    List<Indicator> indicators = new List<Indicator>();
                    List<Group> groups = new List<Group>();
                    List<Bloc> blocs = new List<Bloc>();
                    for(int i=0;i< indicatorValueViewModel.IndicatorIds?.Length;i++)
                    {
                        if(indicatorValueViewModel.IndicatorIds[i]!=null)
                        {
                            indicators.Add(_context.Indicator.FirstOrDefault(ind => ind.Id == indicatorValueViewModel.IndicatorIds[i]));
                            if(indicators.Last().GroupId==null)
                            {
                                groups.Add(null);
                                blocs.Add(null);
                            }
                            else
                            {
                                groups.Add(_context.Group.FirstOrDefault(g => g.Id == indicators.Last().GroupId));
                                blocs.Add(_context.Bloc.FirstOrDefault(b => b.Id == groups.Last().BlocId));
                            }
                        }
                    }

                    string sFileName = $"{(indicators.Count > 0 ? indicators.First().Name : DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"))}.xlsx";
                    FileInfo file = new FileInfo(Path.Combine(sContentRootPath, sFileName));
                    if (file.Exists)
                    {
                        file.Delete();
                        file = new FileInfo(Path.Combine(sContentRootPath, sFileName));
                    }
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        for (int y = 0;y< indicatorValueViewModel.Years?.Length;y++)
                        {
                            var regions = _context.Region.Where(r => r.Year == indicatorValueViewModel.Years[y]
                                && r.Code != Startup.Configuration["KazakhstanCode"]
                                && indicatorValueViewModel.RegionIds.Contains(r.Code))
                            .OrderBy(r =>
                                r.NameRU.Contains("город Алматы") ? "яя" :
                                r.NameRU.Contains("город Астана") ? "яю" :
                                r.NameRU.Contains("Кызылординская область") ? "Костанайская область" :
                                r.NameRU.Contains("Костанайская область") ? "Кызылординская область" :
                                r.Name
                            ).ToList();
                            var indicatorValues = _context.IndicatorValue
                                .Where(i => i.Year == indicatorValueViewModel.Years[y] && indicators.Select(ind => ind.Id).Contains(i.IndicatorId))
                                .Include(i => i.Source)
                                .ToList();
                            if (regions.Count() > 0)
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(indicatorValueViewModel.Years[y].ToString());
                                worksheet.Cells[1, 2].Value = _sharedLocalizer["Bloc"];
                                worksheet.Cells[2, 2].Value = _sharedLocalizer["Group"];
                                worksheet.Cells[3, 2].Value = _sharedLocalizer["Indicator"];
                                for (int i = 0;i<indicators.Count;i++)
                                {
                                    worksheet.Cells[1, 3 + i].Value = blocs[i] != null ? blocs[i].Name : "";
                                    worksheet.Cells[2, 3 + i].Value = groups[i] != null ? groups[i].Name : "";
                                    worksheet.Cells[3, 3 + i].Value = indicators[i].Name;
                                    if (indicatorValues.FirstOrDefault(iv => iv.IndicatorId == indicators[i].Id) != null)
                                    {
                                        if (indicatorValues.FirstOrDefault(iv => iv.IndicatorId == indicators[i].Id).SourceId != null)
                                        {
                                            worksheet.Cells[4, 3 + i].Value = _context.Source.FirstOrDefault(s => s.Id == indicatorValues.FirstOrDefault(iv => iv.IndicatorId == indicators[i].Id).SourceId).Name;
                                        }
                                        else
                                        {
                                            worksheet.Cells[4, 3 + i].Value = null;
                                        }
                                    }
                                    else
                                    {
                                        worksheet.Cells[4, 3 + i].Value = null;
                                    }
                                }
                                worksheet.Cells[4, 1].Value = _sharedLocalizer["Source"];
                                worksheet.Cells[5, 1].Value = _sharedLocalizer["Code"];
                                worksheet.Cells[5, 1].Style.Font.Bold = true;
                                worksheet.Cells[5, 2].Value = _sharedLocalizer["Region"];
                                worksheet.Cells[5, 2].Style.Font.Bold = true;
                                for (int r = 0; r < regions.Count(); r++)
                                {
                                    worksheet.Cells[r + 6, 1].Value = regions[r].Code;
                                    worksheet.Cells[r + 6, 2].Value = regions[r].Name;
                                    for (int i = 0; i < indicators.Count; i++)
                                    {
                                        IndicatorValue indicatorValue = indicatorValues.FirstOrDefault(iv => iv.IndicatorId == indicators[i].Id && iv.RegionId == regions[r].Id);
                                        if (indicatorValue != null)
                                        {
                                            worksheet.Cells[r + 6, 3+i].Value = indicatorValue.Value;
                                        }
                                        else
                                        {
                                            worksheet.Cells[r + 6, 3+i].Value = null;
                                        }
                                    }
                                }
                                for(int i=1;i<=2+indicators.Count;i++)
                                {
                                    worksheet.Column(i).AutoFit();
                                }
                            }
                        }
                        package.Save();
                    }
                    var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(sContentRootPath, file.Name));
                    return File(fileBytes, mimeType, sFileName);
                }
            }
            catch (Exception e)
            {
                error = e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                ViewBag.Error = error;
            }
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", indicatorValueViewModel.BlocId);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode", indicatorValueViewModel.GroupId);
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString(), Selected = i.Id == indicatorValueViewModel.IndicatorId });
            ViewBag.Regions = new SelectList(_context.Region.Where(r => r.Code != "00").OrderBy(r => r.NameCode).DistinctBy(r => r.NameCode), "Code", "NameCode");
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Years = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return View(indicatorValueViewModel);
        }
        
        public IActionResult ReportRegion()
        {
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
            ViewBag.RegionCode = new SelectList(_context.Region.OrderBy(r => r.Name).DistinctBy(r => r.Code), "Code", "NameCode");
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportRegion(int? IndicatorId1,
            int? IndicatorId2,
            int? IndicatorId3,
            int? Year1,
            int? Year2,
            int? Year3,
            string RegionCode,
            string backlink)
        {
            ViewBag.BackLink = backlink;
            string error = null;
            try
            {
                if (ModelState.IsValid)
                {
                    string sContentRootPath = _hostingEnvironment.ContentRootPath;
                    sContentRootPath = Path.Combine(sContentRootPath, "Download");
                    sContentRootPath = Path.Combine(sContentRootPath, "Report");
                    DirectoryInfo di = new DirectoryInfo(sContentRootPath);
                    foreach (FileInfo filed in di.GetFiles())
                    {
                        try
                        {
                            filed.Delete();
                        }
                        catch
                        {
                        }
                    }
                    List<Indicator> indicators = new List<Indicator>();
                    if (IndicatorId1 != null)
                    {
                        indicators.Add(_context.Indicator.FirstOrDefault(i => i.Id == IndicatorId1));
                    }
                    if (IndicatorId2 != null)
                    {
                        indicators.Add(_context.Indicator.FirstOrDefault(i => i.Id == IndicatorId2));
                    }
                    if (IndicatorId3 != null)
                    {
                        indicators.Add(_context.Indicator.FirstOrDefault(i => i.Id == IndicatorId3));
                    }
                    Models.Region region = _context.Region.FirstOrDefault(r => r.Code == RegionCode);
                    string sFileName = $"{region.Name}.xlsx";
                    FileInfo file = new FileInfo(Path.Combine(sContentRootPath, sFileName));
                    if (file.Exists)
                    {
                        file.Delete();
                        file = new FileInfo(Path.Combine(sContentRootPath, sFileName));
                    }
                    List<int> years = new List<int>();
                    if (Year1 != null)
                    {
                        years.Add((int)Year1);
                    }
                    if (Year2 != null)
                    {
                        years.Add((int)Year2);
                    }
                    if (Year3 != null)
                    {
                        years.Add((int)Year3);
                    }
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(region.Name);
                        worksheet.Cells[1, 1].Value = _sharedLocalizer["Year"];
                        worksheet.Cells[1, 1].Style.Font.Bold = true;
                        for (int i = 0; i < indicators.Count; i++)
                        {
                            worksheet.Cells[1, i + 2].Value = indicators[i].Name;
                            worksheet.Cells[1, i + 2].Style.Font.Bold = true;
                        }
                        for (int y = 0; y < years.Count; y++)
                        {
                            worksheet.Cells[y + 2, 1].Value = years[y];
                            for (int i = 0; i < indicators.Count; i++)
                            {
                                var indicatorValue = _context.IndicatorValue
                                    .Include(iv => iv.Region)
                                    .Include(iv => iv.Source)
                                    .FirstOrDefault(iv => iv.Year == years[y] && iv.IndicatorId == indicators[i].Id && iv.Region.Code == RegionCode);
                                worksheet.Cells[y + 2, i + 2].Value = indicatorValue != null ? indicatorValue.Value : null;
                            }
                        }
                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        package.Save();
                    }
                    var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(sContentRootPath, file.Name));
                    return File(fileBytes, mimeType, sFileName);
                }
            }
            catch (Exception e)
            {
                error = e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                ViewBag.Error = error;
            }
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
            ViewBag.RegionCode = new SelectList(_context.Region.OrderBy(r => r.Name).DistinctBy(r => r.Code), "Code", "NameCode");
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return View();
        }
        
        public IActionResult ReportIndicator()
        {
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
            ViewBag.RegionCode = new SelectList(_context.Region.OrderBy(r => r.Name).DistinctBy(r => r.Code), "Code", "NameCode");
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportIndicator(int? IndicatorId,
            int? Year1,
            int? Year2,
            int? Year3,
            string RegionCode1,
            string RegionCode2,
            string RegionCode3,
            string backlink)
        {
            ViewBag.BackLink = backlink;
            string error = null;
            try
            {
                if (ModelState.IsValid)
                {
                    string sContentRootPath = _hostingEnvironment.ContentRootPath;
                    sContentRootPath = Path.Combine(sContentRootPath, "Download");
                    sContentRootPath = Path.Combine(sContentRootPath, "Report");
                    DirectoryInfo di = new DirectoryInfo(sContentRootPath);
                    foreach (FileInfo filed in di.GetFiles())
                    {
                        try
                        {
                            filed.Delete();
                        }
                        catch
                        {
                        }
                    }
                    List<string> regioncods = new List<string>();
                    if (!string.IsNullOrEmpty(RegionCode1))
                    {
                        regioncods.Add(RegionCode1);
                    }
                    if (!string.IsNullOrEmpty(RegionCode2))
                    {
                        regioncods.Add(RegionCode2);
                    }
                    if (!string.IsNullOrEmpty(RegionCode3))
                    {
                        regioncods.Add(RegionCode3);
                    }
                    Indicator indicator = _context.Indicator.FirstOrDefault(i => i.Id == IndicatorId);
                    string sFileName = $"{indicator.Name}.xlsx";
                    FileInfo file = new FileInfo(Path.Combine(sContentRootPath, sFileName));
                    if (file.Exists)
                    {
                        file.Delete();
                        file = new FileInfo(Path.Combine(sContentRootPath, sFileName));
                    }
                    List<int> years = new List<int>();
                    if (Year1 != null)
                    {
                        years.Add((int)Year1);
                    }
                    if (Year2 != null)
                    {
                        years.Add((int)Year2);
                    }
                    if (Year3 != null)
                    {
                        years.Add((int)Year3);
                    }
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(indicator.Name);
                        worksheet.Cells[1, 1].Value = _sharedLocalizer["Year"];
                        worksheet.Cells[1, 1].Style.Font.Bold = true;
                        for (int i = 0; i < regioncods.Count; i++)
                        {
                            worksheet.Cells[1, i + 2].Value = _context.Region.Where(r => r.Code == regioncods[i]).Select(r => r.Name).FirstOrDefault();
                            worksheet.Cells[1, i + 2].Style.Font.Bold = true;
                        }
                        for (int y = 0; y < years.Count; y++)
                        {
                            worksheet.Cells[y + 2, 1].Value = years[y];
                            for (int r = 0; r < regioncods.Count; r++)
                            {
                                var indicatorValue = _context.IndicatorValue
                                    .Include(iv => iv.Region)
                                    .Include(iv => iv.Source)
                                    .FirstOrDefault(iv => iv.Year == years[y] && iv.IndicatorId == IndicatorId && iv.Region.Code == regioncods[r]);
                                worksheet.Cells[y + 2, r + 2].Value = indicatorValue != null ? indicatorValue.Value : null;
                            }
                        }
                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        package.Save();
                    }
                    var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(sContentRootPath, file.Name));
                    return File(fileBytes, mimeType, sFileName);
                }
            }
            catch (Exception e)
            {
                error = e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                ViewBag.Error = error;
            }
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
            ViewBag.RegionCode = new SelectList(_context.Region.OrderBy(r => r.Name).DistinctBy(r => r.Code), "Code", "NameCode");
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return View();
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Data()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetRegionsByYear(int Year)
        {
            var regions = _context.Region.Where(r => r.Year == Year).ToArray().OrderBy(r => r.NameCode);
            JsonResult result = new JsonResult(regions);
            return result;
        }

        [HttpPost]
        public JsonResult GetGroupsByBloc(int? BlocId)
        {
            var groups = _context.Group.Where(g => g.BlocId == BlocId).ToArray().OrderBy(g => g.NameCode);
            JsonResult result = new JsonResult(groups);
            return result;
        }

        [HttpPost]
        public JsonResult GetIndicatorByGroup(int? GroupId)
        {
            var indicators = _context.Indicator.Where(i => i.GroupId == GroupId).ToArray().OrderBy(i => i.NameCode);
            JsonResult result = new JsonResult(indicators);
            return result;
        }

        [HttpPost]
        public JsonResult GetRegularIndicatorByGroup(int GroupId)
        {
            var indicators = _context.Indicator.Where(i => i.Type == Startup.Configuration["Regular"]).Where(i => i.GroupId == GroupId).ToArray().OrderBy(i => i.NameCode);
            JsonResult result = new JsonResult(indicators);
            return result;
        }

        [HttpPost]
        public JsonResult GetIndicatorByTypeBlocGroup(string Type, int? BlocId, int? GroupId)
        {
            var indicators = _context.Indicator.Where(i => true).Include(i => i.Group).OrderBy(i => i.NameCode);
            if (!string.IsNullOrEmpty(Type))
            {
                indicators = indicators.Where(i => i.Type == Type).OrderBy(i => i.NameCode);
            }
            if (BlocId != null)
            {
                indicators = indicators.Where(i => i.Group.BlocId == BlocId).OrderBy(i => i.NameCode);
            }
            if (GroupId != null)
            {
                indicators = indicators.Where(i => i.GroupId == GroupId).OrderBy(i => i.NameCode);
            }
            JsonResult result = new JsonResult(indicators.ToArray());
            return result;
        }

        public double StringCompare(string a, string b)
        {
            if (a == b) //Same string, no iteration needed.
                return 100;
            if ((a.Length == 0) || (b.Length == 0)) //One is empty, second is not
            {
                return 0;
            }
            double maxLen = a.Length > b.Length ? a.Length : b.Length;
            int minLen = a.Length < b.Length ? a.Length : b.Length;
            int sameCharAtIndex = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (b.IndexOf(a[i]) >= 0)
                {
                    sameCharAtIndex++;
                    b = b.Remove(b.IndexOf(a[i]), 1);
                }
            }
            double r = sameCharAtIndex / maxLen * 100;
            return r;
        }

        public void KazakhstanCreate()
        {
            // add region Kazakhstan
            for (int year = Convert.ToInt32(Startup.Configuration["YearMin"]); year <= DateTime.Now.Year; year++)
            {
                int kzcount = _context.Region.AsNoTracking().Count(r => r.Year == year && string.IsNullOrEmpty(r.Coordinates) && r.Code == Startup.Configuration["KazakhstanCode"]),
                    regionscount = _context.Region.AsNoTracking().Count(r => r.Year == year && !string.IsNullOrEmpty(r.Coordinates) && r.Code != Startup.Configuration["KazakhstanCode"]);
                if (kzcount == 0 && regionscount > 0)
                {
                    Models.Region kz = new Models.Region()
                    {
                        Area = _context.Region.AsNoTracking().Where(r => r.Year == year && !string.IsNullOrEmpty(r.Coordinates) && r.Code != Startup.Configuration["KazakhstanCode"]).Sum(r => r.Area),
                        Code = Startup.Configuration["KazakhstanCode"],
                        NameEN = _sharedLocalizer.WithCulture(new CultureInfo("en"))["Kazakhstan"],
                        NameKK = _sharedLocalizer.WithCulture(new CultureInfo("kk"))["Kazakhstan"],
                        NameRU = _sharedLocalizer.WithCulture(new CultureInfo("ru"))["Kazakhstan"],
                        Population = _context.Region.AsNoTracking().Where(r => r.Year == year && !string.IsNullOrEmpty(r.Coordinates) && r.Code != Startup.Configuration["KazakhstanCode"]).Sum(r => r.Population),
                        Year = year
                    };
                    _context.Add(kz);
                }
            }
            _context.SaveChanges();
            // add the values of indicators of the region of Kazakhstan
            List<Indicator> indicators = _context.Indicator.AsNoTracking().ToList();
            for (int year = Convert.ToInt32(Startup.Configuration["YearMin"]); year <= DateTime.Now.Year; year++)
            {
                foreach(var indicator in indicators)
                {
                    List<IndicatorValue> indicatorValues = _context.IndicatorValue.AsNoTracking().Include(i => i.Region).Where(i => i.Region.Year == year && i.IndicatorId == indicator.Id).ToList();
                    int indicatorValueKZCount = _context.IndicatorValue
                        .Include(i => i.Region)
                        .Count(i => i.Region.Year == year && i.IndicatorId == indicator.Id && string.IsNullOrEmpty(i.Region.Coordinates) && i.Region.Code == Startup.Configuration["KazakhstanCode"]);
                    if(indicatorValueKZCount == 0 && indicatorValues.Count() > 0)
                    {
                        IndicatorValue indicatorValueKZ = new IndicatorValue()
                        {
                            IndicatorId = indicator.Id,
                            RegionId = _context.Region.AsNoTracking().FirstOrDefault(r => r.Year == year && string.IsNullOrEmpty(r.Coordinates) && r.Code == Startup.Configuration["KazakhstanCode"]).Id,
                            SourceId = indicatorValues.FirstOrDefault().SourceId,
                            Year = indicatorValues.FirstOrDefault().Region.Year,
                            Value = indicatorValues.Average(i => i.Value)
                        };
                        _context.Add(indicatorValueKZ);
                    }
                }
            }
            _context.SaveChanges();
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
            Models.Region kz = _context.Region.FirstOrDefault(r => r.Year == Year && string.IsNullOrEmpty(r.Coordinates) && r.Code == Startup.Configuration["KazakhstanCode"]);
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
            KazakhstanRegionsEditByYear(Year);
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
    }
}