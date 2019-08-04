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
    public class SourcesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public SourcesController(ApplicationDbContext context, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Sources
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder, string NameEN, string NameKK, string NameRU, int? Page)
        {
            var sources = _context.Source
                .Where(s => true);
            
            ViewBag.NameENFilter = NameEN;
            ViewBag.NameKKFilter = NameKK;
            ViewBag.NameRUFilter = NameRU;
            
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            
            if (!string.IsNullOrEmpty(NameEN))
            {
                sources = sources.Where(s => s.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                sources = sources.Where(s => s.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                sources = sources.Where(s => s.NameRU.ToLower().Contains(NameRU.ToLower()));
            }

            switch (SortOrder)
            {
                case "NameEN":
                    sources = sources.OrderBy(s => s.NameEN);
                    break;
                case "NameENDesc":
                    sources = sources.OrderByDescending(s => s.NameEN);
                    break;
                case "NameKK":
                    sources = sources.OrderBy(s => s.NameKK);
                    break;
                case "NameKKDesc":
                    sources = sources.OrderByDescending(s => s.NameKK);
                    break;
                case "NameRU":
                    sources = sources.OrderBy(s => s.NameRU);
                    break;
                case "NameRUDesc":
                    sources = sources.OrderByDescending(s => s.NameRU);
                    break;
                default:
                    sources = sources.OrderBy(s => s.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(sources.Count(), Page);

            var viewModel = new SourceIndexPageViewModel
            {
                Items = sources.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: Sources/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var source = await _context.Source
                .SingleOrDefaultAsync(m => m.Id == id);
            if (source == null)
            {
                return NotFound();
            }

            return View(source);
        }

        // GET: Sources/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,NameEN,NameKK,NameRU")] Source source, string backlink)
        {
            ViewBag.BackLink = backlink;
            var sources = _context.Source.ToList();
            if (sources.Select(s => s.NameEN).Contains(source.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (sources.Select(s => s.NameKK).Contains(source.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (sources.Select(s => s.NameRU).Contains(source.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                _context.Add(source);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Создание Типа источника данных",
                    New = $"{source.NameEN}; {source.NameKK}; {source.NameRU}",
                    Old = ""
                });
                await _context.SaveChangesAsync();
                if (backlink.ToLower().Contains("source"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            return View(source);
        }

        // GET: Sources/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var source = await _context.Source.SingleOrDefaultAsync(m => m.Id == id);
            if (source == null)
            {
                return NotFound();
            }
            return View(source);
        }

        // POST: Sources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameEN,NameKK,NameRU")] Source source, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (id != source.Id)
            {
                return NotFound();
            }
            var sources = _context.Source
                .AsNoTracking()
                .ToList();
            if (sources.Where(s => s.Id != source.Id).Select(s => s.NameEN).Contains(source.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (sources.Where(s => s.Id != source.Id).Select(s => s.NameKK).Contains(source.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (sources.Where(s => s.Id != source.Id).Select(s => s.NameRU).Contains(source.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Source source_old = _context.Source.AsNoTracking().FirstOrDefault(s => s.Id == source.Id);
                    _context.Log.Add(new Log()
                    {
                        DateTime = DateTime.Now,
                        Email = User.Identity.Name,
                        Operation = "Редактирование Типа источника данных",
                        New = $"{source.NameEN}; {source.NameKK}; {source.NameRU}",
                        Old = $"{source_old.NameEN}; {source_old.NameKK}; {source_old.NameRU}"
                    });
                    _context.Update(source);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SourceExists(source.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (backlink.ToLower().Contains("source"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            return View(source);
        }

        // GET: Sources/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var source = await _context.Source
                .SingleOrDefaultAsync(m => m.Id == id);
            if (source == null)
            {
                return NotFound();
            }

            return View(source);
        }

        // POST: Sources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id, string backlink)
        {
            ViewBag.BackLink = backlink;
            List<int> indicatorIds = _context.IndicatorValue.Where(i => i.SourceId == id).Select(i => i.IndicatorId).Distinct().ToList();
            _context.IndicatorValue.RemoveRange(_context.IndicatorValue.Where(i => i.SourceId == id));
            await _context.SaveChangesAsync();
            foreach(int indicatorId in indicatorIds)
            {
                new IndicatorsController(_context, _sharedLocalizer).CalculateIndicatorsValues(indicatorId);
            }
            var source = await _context.Source.SingleOrDefaultAsync(m => m.Id == id);
            _context.Log.Add(new Log()
            {
                DateTime = DateTime.Now,
                Email = User.Identity.Name,
                Operation = "Удаление Типа источника данных",
                New = "",
                Old = $"{source.NameEN}; {source.NameKK}; {source.NameRU}"
            });
            _context.Source.Remove(source);
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(backlink))
            {
                if (backlink.ToLower().Contains("source"))
                {
                    return Redirect(backlink);
                }
            }
                
            return RedirectToAction("Index");
        }

        private bool SourceExists(int id)
        {
            return _context.Source.Any(e => e.Id == id);
        }
    }
}
