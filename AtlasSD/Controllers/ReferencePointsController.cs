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
    public class ReferencePointsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public ReferencePointsController(ApplicationDbContext context, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: ReferencePoints
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder, int? BlocId, int? GroupId, int? IndicatorId, int? Page)
        {
            var referencePoints = _context.ReferencePoint
                .Include(r => r.Indicator)
                .Include(r => r.Indicator.Group)
                .Include(r => r.Indicator.Group.Bloc)
                .Where(r => true);

            ViewBag.BlocIdFilter = BlocId;
            ViewBag.GroupIdFilter = GroupId;
            ViewBag.IndicatorIdFilter = IndicatorId;

            ViewBag.BlocSort = SortOrder == "Bloc" ? "BlocDesc" : "Bloc";
            ViewBag.GroupSort = SortOrder == "Group" ? "GroupDesc" : "Group";
            ViewBag.IndicatorSort = SortOrder == "Indicator" ? "IndicatorDesc" : "Indicator";

            if (BlocId != null)
            {
                referencePoints = referencePoints.Where(r => r.Indicator.Group.BlocId == BlocId);
            }
            if (GroupId != null)
            {
                referencePoints = referencePoints.Where(r => r.Indicator.GroupId == GroupId);
            }
            if (IndicatorId != null)
            {
                referencePoints = referencePoints.Where(r => r.IndicatorId == IndicatorId);
            }

            switch (SortOrder)
            {
                case "Bloc":
                    referencePoints = referencePoints.OrderBy(r => r.Indicator.Group.Bloc.Name);
                    break;
                case "BlocDesc":
                    referencePoints = referencePoints.OrderByDescending(r => r.Indicator.Group.Bloc.Name);
                    break;
                case "Group":
                    referencePoints = referencePoints.OrderBy(r => r.Indicator.Group.Name);
                    break;
                case "GroupDesc":
                    referencePoints = referencePoints.OrderByDescending(r => r.Indicator.Group.Name);
                    break;
                case "Indicator":
                    referencePoints = referencePoints.OrderBy(r => r.Indicator.Name);
                    break;
                case "IndicatorDesc":
                    referencePoints = referencePoints.OrderByDescending(r => r.Indicator.Name);
                    break;
                default:
                    referencePoints = referencePoints.OrderBy(r => r.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(referencePoints.Count(), Page);

            var viewModel = new ReferencePointIndexPageViewModel
            {
                Items = referencePoints.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            ViewBag.BlocId = new SelectList(_context.Bloc.OrderBy(b => b.Name), "Id", "Name");
            ViewBag.GroupId = new SelectList(_context.Group.OrderBy(g => g.Name), "Id", "Name");
            ViewBag.IndicatorId = new SelectList(_context.Indicator.OrderBy(g => g.Name), "Id", "Name");
            
            return View(viewModel);
        }

        // GET: ReferencePoints/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referencePoint = await _context.ReferencePoint
                .Include(r => r.Indicator)
                .Include(r => r.Indicator.Group)
                .Include(r => r.Indicator.Group.Bloc)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (referencePoint == null)
            {
                return NotFound();
            }

            return View(referencePoint);
        }

        // GET: ReferencePoints/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewData["IndicatorId"] = new SelectList(_context.Indicator.OrderBy(g => g.NameCode), "Id", "NameCode");
            return View();
        }

        // POST: ReferencePoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create(ReferencePointViewModel referencePointViewModel, string backlink)
        {
            ViewBag.BackLink = backlink;
            if(_context.ReferencePoint.Where(r => r.IndicatorId == referencePointViewModel.IndicatorId).Count() > 0)
            {
                ModelState.AddModelError("IndicatorId", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                ReferencePoint referencePoint = new ReferencePoint()
                {
                    IndicatorId = referencePointViewModel.IndicatorId,
                    Min = referencePointViewModel.Min,
                    Max = referencePointViewModel.Max
                };
                _context.Add(referencePoint);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Создание Референтной точки",
                    New = $"{referencePoint.Min}; {referencePoint.Max}",
                    Old = ""
                });
                await _context.SaveChangesAsync();
                new IndicatorsController(_context, _sharedLocalizer).CalculateIndicatorsValues((int)referencePointViewModel.IndicatorId);
                if (backlink.ToLower().Contains("referencepoint"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            referencePointViewModel.Indicator = _context.Indicator.FirstOrDefault(i => i.Id == referencePointViewModel.IndicatorId);
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", referencePointViewModel.BlocId);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode).Where(g => g.BlocId == referencePointViewModel.BlocId), "Id", "NameCode", referencePointViewModel.GroupId);
            ViewData["IndicatorId"] = new SelectList(_context.Indicator.OrderBy(i => i.NameCode).Where(i => i.GroupId == referencePointViewModel.GroupId), "Id", "NameCode", referencePointViewModel.IndicatorId);
            return View(referencePointViewModel);
        }

        // GET: ReferencePoints/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referencePoint = await _context.ReferencePoint.SingleOrDefaultAsync(m => m.Id == id);
            if (referencePoint == null)
            {
                return NotFound();
            }
            ReferencePointViewModel referencePointViewModel = new ReferencePointViewModel()
            {
                IndicatorId = referencePoint.IndicatorId,
                Indicator = _context.Indicator.FirstOrDefault(i => i.Id == referencePoint.IndicatorId),
                Min = referencePoint.Min,
                Max = referencePoint.Max
            };

            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            ViewData["IndicatorId"] = new SelectList(_context.Indicator.OrderBy(g => g.NameCode), "Id", "NameCode", referencePoint.IndicatorId);
            return View(referencePointViewModel);
        }

        // POST: ReferencePoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, ReferencePointViewModel referencePointViewModel, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (id != referencePointViewModel.Id)
            {
                return NotFound();
            }
            if (_context.ReferencePoint.Where(r => r.IndicatorId == referencePointViewModel.IndicatorId && r.Id != referencePointViewModel.Id).Count() > 0)
            {
                ModelState.AddModelError("IndicatorId", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    ReferencePoint referencePoint = new ReferencePoint()
                    {
                        Id = referencePointViewModel.Id,
                        IndicatorId = referencePointViewModel.IndicatorId,
                        Min = referencePointViewModel.Min,
                        Max = referencePointViewModel.Max
                    };
                    ReferencePoint referencePoint_old = _context.ReferencePoint.AsNoTracking().FirstOrDefault(r => r.Id == referencePoint.Id);
                    _context.Log.Add(new Log()
                    {
                        DateTime = DateTime.Now,
                        Email = User.Identity.Name,
                        Operation = "Редактирование Референтной точки",
                        New = $"{referencePoint.Min}; {referencePoint.Max}",
                        Old = $"{referencePoint_old.Min}; {referencePoint_old.Max}"
                    });
                    _context.Update(referencePoint);
                    await _context.SaveChangesAsync();
                    new IndicatorsController(_context, _sharedLocalizer).CalculateIndicatorsValues((int)referencePointViewModel.IndicatorId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReferencePointExists(referencePointViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (backlink.ToLower().Contains("referencepoint"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            ViewData["IndicatorId"] = new SelectList(_context.Indicator.OrderBy(g => g.NameCode), "Id", "NameCode", referencePointViewModel.IndicatorId);
            referencePointViewModel.Indicator = _context.Indicator.FirstOrDefault(i => i.Id == referencePointViewModel.IndicatorId);
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", referencePointViewModel.BlocId);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode).Where(g => g.BlocId == referencePointViewModel.BlocId), "Id", "NameCode", referencePointViewModel.GroupId);
            ViewData["IndicatorId"] = new SelectList(_context.Indicator.OrderBy(i => i.NameCode).Where(i => i.GroupId == referencePointViewModel.GroupId), "Id", "NameCode", referencePointViewModel.IndicatorId);
            return View(referencePointViewModel);
        }

        // GET: ReferencePoints/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var referencePoint = await _context.ReferencePoint
                .Include(r => r.Indicator)
                .Include(r => r.Indicator.Group)
                .Include(r => r.Indicator.Group.Bloc)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (referencePoint == null)
            {
                return NotFound();
            }
            ViewBag.Indicators = _context.Indicator
                .Where(i => i.Formula.Contains($"Max({Startup.Configuration["FormulaIdFirstChar"]}{referencePoint.IndicatorId.ToString()}{Startup.Configuration["FormulaIdLastChar"]}") ||
                    i.Formula.Contains($"Min({Startup.Configuration["FormulaIdFirstChar"]}{referencePoint.IndicatorId.ToString()}{Startup.Configuration["FormulaIdLastChar"]}"))
                .OrderBy(i => i.NameCode);
            return View(referencePoint);
        }

        // POST: ReferencePoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id, string backlink)
        {
            ViewBag.BackLink = backlink;
            var referencePoint = await _context.ReferencePoint.SingleOrDefaultAsync(m => m.Id == id);
            if (_context.Indicator
                .Where(i => i.Formula.Contains($"Max({Startup.Configuration["FormulaIdFirstChar"]}{referencePoint.IndicatorId.ToString()}{Startup.Configuration["FormulaIdLastChar"]}") ||
                    i.Formula.Contains($"Min({Startup.Configuration["FormulaIdFirstChar"]}{referencePoint.IndicatorId.ToString()}{Startup.Configuration["FormulaIdLastChar"]}"))
                    .Count() == 0)
            {
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Удаление Референтной точки",
                    New = "",
                    Old = $"{referencePoint.Min}; {referencePoint.Max}"
                });
                _context.ReferencePoint.Remove(referencePoint);
                await _context.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(backlink))
            {
                if (backlink.ToLower().Contains("referencepoint"))
                {
                    return Redirect(backlink);
                }
            }
                
            return RedirectToAction("Index");
        }

        private bool ReferencePointExists(int id)
        {
            return _context.ReferencePoint.Any(e => e.Id == id);
        }

        [HttpPost]
        public JsonResult GetGroupsByBloc(int BlocId)
        {
            var groups = _context.Group.Where(g => g.BlocId == BlocId).ToArray().OrderBy(g => g.NameCode);
            JsonResult result = new JsonResult(groups);
            return result;
        }

        [HttpPost]
        public JsonResult GetIndicatorByGroup(int GroupId)
        {
            var indicators = _context.Indicator.Where(i => i.GroupId == GroupId).ToArray().OrderBy(i => i.NameCode);
            JsonResult result = new JsonResult(indicators);
            return result;
        }
    }
}
