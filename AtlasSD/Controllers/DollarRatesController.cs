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
using Microsoft.Extensions.Localization;

namespace AtlasSD.Controllers
{
    public class DollarRatesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public DollarRatesController(ApplicationDbContext context, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: DollarRates
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder, int? Year, int? Page)
        {
            var dollarRates = _context.DollarRate
                .Where(d => true);

            ViewBag.YearFilter = Year;

            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.ValueSort = SortOrder == "Value" ? "ValueDesc" : "Value";

            if (Year != null)
            {
                dollarRates = dollarRates.Where(d => d.Year == Year);
            }

            switch (SortOrder)
            {
                case "Year":
                    dollarRates = dollarRates.OrderBy(d => d.Year);
                    break;
                case "YearDesc":
                    dollarRates = dollarRates.OrderByDescending(d => d.Year);
                    break;
                case "Value":
                    dollarRates = dollarRates.OrderBy(d => d.Value);
                    break;
                case "ValueDesc":
                    dollarRates = dollarRates.OrderByDescending(d => d.Value);
                    break;
                default:
                    dollarRates = dollarRates.OrderBy(d => d.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(dollarRates.Count(), Page);

            var viewModel = new DollarRateIndexPageViewModel
            {
                Items = dollarRates.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            return View(viewModel);
        }

        // GET: DollarRates/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dollarRate = await _context.DollarRate
                .SingleOrDefaultAsync(m => m.Id == id);
            if (dollarRate == null)
            {
                return NotFound();
            }

            return View(dollarRate);
        }

        // GET: DollarRates/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return View();
        }

        // POST: DollarRates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,Year,Value")] DollarRate dollarRate)
        {
            var dollarRates = _context.DollarRate.AsNoTracking().ToList();
            if (dollarRates.Select(d => d.Year).Contains(dollarRate.Year))
            {
                ModelState.AddModelError("Year", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                _context.Add(dollarRate);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Создание Курса доллара",
                    New = $"{dollarRate.Year}; {dollarRate.Value}",
                    Old = ""
                });
                await _context.SaveChangesAsync();
                List<Indicator> indicators = _context.Indicator.AsNoTracking().Where(i => i.Formula.Contains("$")).ToList();
                for(int i = 0;i<indicators.Count();i++)
                {
                    new IndicatorsController(_context, _sharedLocalizer).CalculateIndicatorValues(indicators[i].Id);
                }
                return RedirectToAction("Index");
            }
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = i == dollarRate.Year });
            return View(dollarRate);
        }

        // GET: DollarRates/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dollarRate = await _context.DollarRate.SingleOrDefaultAsync(m => m.Id == id);
            if (dollarRate == null)
            {
                return NotFound();
            }
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = i == dollarRate.Year });
            return View(dollarRate);
        }

        // POST: DollarRates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Year,Value")] DollarRate dollarRate)
        {
            if (id != dollarRate.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    DollarRate dollarRate_old = _context.DollarRate.AsNoTracking().FirstOrDefault(d => d.Id == dollarRate.Id);
                    _context.Log.Add(new Log()
                    {
                        DateTime = DateTime.Now,
                        Email = User.Identity.Name,
                        Operation = "Редактирование Курса доллара",
                        New = $"{dollarRate.Year}; {dollarRate.Value}",
                        Old = $"{dollarRate_old.Year}; {dollarRate_old.Value}"
                    });
                    _context.Update(dollarRate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DollarRateExists(dollarRate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                List<Indicator> indicators = _context.Indicator.AsNoTracking().Where(i => i.Formula.Contains("$")).ToList();
                for (int i = 0; i < indicators.Count(); i++)
                {
                    new IndicatorsController(_context, _sharedLocalizer).CalculateIndicatorValues(indicators[i].Id);
                }
                return RedirectToAction("Index");
            }
            int year_min = Convert.ToInt32(Startup.Configuration["YearMin"]);
            ViewBag.Year = Enumerable.Range(year_min, DateTime.Now.Year - year_min + 1).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = i == dollarRate.Year });
            return View(dollarRate);
        }

        // GET: DollarRates/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dollarRate = await _context.DollarRate
                .SingleOrDefaultAsync(m => m.Id == id);
            if (dollarRate == null)
            {
                return NotFound();
            }
            ViewBag.Indicators = _context.Indicator
                .AsNoTracking()
                .Where(i => i.Formula.Contains("$"))
                .OrderBy(i => i.NameCode);
            return View(dollarRate);
        }

        // POST: DollarRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Indicator.AsNoTracking().FirstOrDefault(i => i.Formula.Contains("$")) == null)
            {
                var dollarRate = await _context.DollarRate.SingleOrDefaultAsync(m => m.Id == id);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Удаление Курса доллара",
                    New = "",
                    Old = $"{dollarRate.Year}; {dollarRate.Value}"
                });
                _context.DollarRate.Remove(dollarRate);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private bool DollarRateExists(int id)
        {
            return _context.DollarRate.Any(e => e.Id == id);
        }
    }
}
