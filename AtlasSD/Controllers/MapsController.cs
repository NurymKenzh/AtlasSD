using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasSD.Data;
using AtlasSD.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;

namespace AtlasSD.Controllers
{
    public class MapsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public MapsController(ApplicationDbContext context, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Maps
        //[Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder, int? BlocId, int? GroupId, int? IndicatorId, string NameEN, string NameKK, string NameRU, string Description, int? Page)
        {
            string language = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;

            var maps = _context.Map
                .Include(m => m.Indicator)
                .Include(m => m.Indicator.Group)
                .Include(m => m.Indicator.Group.Bloc)
                .Where(m => true);

            ViewBag.BlocIdFilter = BlocId;
            ViewBag.GroupIdFilter = GroupId;
            ViewBag.IndicatorIdFilter = IndicatorId;
            ViewBag.NameENFilter = NameEN;
            ViewBag.NameKKFilter = NameKK;
            ViewBag.NameRUFilter = NameRU;
            ViewBag.DescriptionFilter = Description;

            ViewBag.BlocSort = SortOrder == "Bloc" ? "BlocDesc" : "Bloc";
            ViewBag.GroupSort = SortOrder == "Group" ? "GroupDesc" : "Group";
            ViewBag.IndicatorSort = SortOrder == "Indicator" ? "IndicatorDesc" : "Indicator";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";

            if (BlocId != null)
            {
                maps = maps.Where(m => m.Indicator.Group.BlocId == BlocId);
            }
            if (GroupId != null)
            {
                maps = maps.Where(m => m.Indicator.GroupId == GroupId);
            }
            if (IndicatorId != null)
            {
                maps = maps.Where(m => m.IndicatorId == IndicatorId);
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                maps = maps.Where(m => m.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                maps = maps.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                maps = maps.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(Description))
            {
                maps = maps.Where(m => m.DescriptionEN.ToLower().Contains(Description.ToLower())
                    || m.DescriptionKK.ToLower().Contains(Description.ToLower())
                    || m.DescriptionRU.ToLower().Contains(Description.ToLower())
                    || m.NameEN.ToLower().Contains(Description.ToLower())
                    || m.NameKK.ToLower().Contains(Description.ToLower())
                    || m.NameRU.ToLower().Contains(Description.ToLower())
                    || m.Indicator.Group.Bloc.NameEN.ToLower().Contains(Description.ToLower())
                    || m.Indicator.Group.Bloc.NameKK.ToLower().Contains(Description.ToLower())
                    || m.Indicator.Group.Bloc.NameRU.ToLower().Contains(Description.ToLower())
                    || m.Indicator.Group.NameEN.ToLower().Contains(Description.ToLower())
                    || m.Indicator.Group.NameKK.ToLower().Contains(Description.ToLower())
                    || m.Indicator.Group.NameRU.ToLower().Contains(Description.ToLower())
                    || m.Indicator.NameEN.ToLower().Contains(Description.ToLower())
                    || m.Indicator.NameKK.ToLower().Contains(Description.ToLower())
                    || m.Indicator.NameRU.ToLower().Contains(Description.ToLower())
                    || m.KeywordsEN.ToLower().Contains(Description.ToLower())
                    || m.KeywordsKK.ToLower().Contains(Description.ToLower())
                    || m.KeywordsRU.ToLower().Contains(Description.ToLower()));
            }

            switch (SortOrder)
            {
                case "Bloc":
                    switch (language)
                    {
                        case "en":
                            maps = maps.OrderBy(m => m.Indicator.Group.Bloc.NameEN);
                            break;
                        case "kk":
                            maps = maps.OrderBy(m => m.Indicator.Group.Bloc.NameKK);
                            break;
                        default:
                            maps = maps.OrderBy(m => m.Indicator.Group.Bloc.NameRU);
                            break;
                    }
                    break;
                case "BlocDesc":
                    switch (language)
                    {
                        case "en":
                            maps = maps.OrderByDescending(m => m.Indicator.Group.Bloc.NameEN);
                            break;
                        case "kk":
                            maps = maps.OrderByDescending(m => m.Indicator.Group.Bloc.NameKK);
                            break;
                        default:
                            maps = maps.OrderByDescending(m => m.Indicator.Group.Bloc.NameRU);
                            break;
                    }
                    break;
                case "Group":
                    switch (language)
                    {
                        case "en":
                            maps = maps.OrderBy(m => m.Indicator.Group.NameEN);
                            break;
                        case "kk":
                            maps = maps.OrderBy(m => m.Indicator.Group.NameKK);
                            break;
                        default:
                            maps = maps.OrderBy(m => m.Indicator.Group.NameRU);
                            break;
                    }
                    break;
                case "GroupDesc":
                    switch (language)
                    {
                        case "en":
                            maps = maps.OrderByDescending(m => m.Indicator.Group.NameEN);
                            break;
                        case "kk":
                            maps = maps.OrderByDescending(m => m.Indicator.Group.NameKK);
                            break;
                        default:
                            maps = maps.OrderByDescending(m => m.Indicator.Group.NameRU);
                            break;
                    }
                    break;
                case "Indicator":
                    maps = maps.OrderBy(m => m.Indicator.Name);
                    break;
                case "IndicatorDesc":
                    maps = maps.OrderByDescending(m => m.Indicator.Name);
                    break;
                case "NameEN":
                    maps = maps.OrderBy(m => m.NameEN);
                    break;
                case "NameENDesc":
                    maps = maps.OrderByDescending(m => m.NameEN);
                    break;
                case "NameKK":
                    maps = maps.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    maps = maps.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    maps = maps.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    maps = maps.OrderByDescending(m => m.NameRU);
                    break;
                default:
                    maps = maps.OrderBy(m => m.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(maps.Count(), Page);

            var viewModel = new MapIndexPageViewModel
            {
                Items = maps.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            ViewBag.BlocId = new SelectList(_context.Bloc.OrderBy(b => b.Name), "Id", "Name");
            ViewBag.GroupId = new SelectList(_context.Group.OrderBy(g => g.Name), "Id", "Name");
            ViewBag.IndicatorId = new SelectList(_context.Indicator.OrderBy(g => g.Name), "Id", "Name");

            return View(viewModel);
        }

        // GET: Maps/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var map = await _context.Map
                .Include(i => i.Indicator)
                .Include(i => i.Indicator.Group)
                .Include(i => i.Indicator.Group.Bloc)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (map == null)
            {
                return NotFound();
            }
            map.MapIntervals = new List<MapInterval>();
            for (int i = 0; i < map.Colors.Count(); i++)
            {
                map.MapIntervals.Add(new MapInterval()
                {
                    Color = map.Colors[i],
                    MinValue = map.MinValues[i]
                });
            }
            map.MapIndicators = new List<MapIndicator>();
            for (int i = 0; i < map.IndicatorColors.Count(); i++)
            {
                Indicator indicator = _context.Indicator
                    .Include(ind => ind.Group)
                    .Include(ind => ind.Group.Bloc)
                    .FirstOrDefault(ind => ind.Id == map.IndicatorIds[i]);
                map.MapIndicators.Add(new MapIndicator()
                {
                    Color = map.IndicatorColors[i],
                    IndicatorId = map.IndicatorIds[i],
                    Bloc = indicator.Group.Bloc,
                    Group = indicator.Group,
                    Indicator = indicator
                });
            }
            return View(map);
        }

        // GET: Maps/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            int BlocId = _context.Bloc.OrderBy(b => b.NameCode).FirstOrDefault().Id,
                GroupId = _context.Group.OrderBy(g => g.NameCode).Where(g => g.BlocId == BlocId).FirstOrDefault().Id;
            //    IndicatorId = _context.Indicator.OrderBy(i => i.NameCode).Where(i => i.GroupId == GroupId).FirstOrDefault().Id;
            Map map = new Map
            {
                //BlocId = BlocId,
                //GroupId = GroupId,
                //IndicatorId = IndicatorId,
                MapIntervals = new List<MapInterval>()
                {
                    new MapInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                MapIndicators = new List<MapIndicator>()
            };
            ViewData["IndicatorBlocIds"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode).Where(g => g.BlocId == BlocId), "Id", "NameCode");
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.GroupId == GroupId).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString() });
            return View(map);
        }

        // POST: Maps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create(Map map, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (map.MapIntervals == null)
            {
                map.MapIntervals = new List<MapInterval>();
            }
            for (int i = map.MapIntervals.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(map.MapIntervals[i].Color))
                {
                    map.MapIntervals.RemoveAt(i);
                }
            }
            if (map.MapIndicators == null)
            {
                map.MapIndicators = new List<MapIndicator>();
            }
            for (int i = map.MapIndicators.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(map.MapIndicators[i].Color))
                {
                    map.MapIndicators.RemoveAt(i);
                }
            }
            var maps = _context.Map
                .AsNoTracking()
                .ToList();
            if (maps.Where(m => m.IndicatorId == map.IndicatorId).Select(m => m.NameEN).Contains(map.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (maps.Where(m => m.IndicatorId == map.IndicatorId).Select(m => m.NameKK).Contains(map.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (maps.Where(m => m.IndicatorId == map.IndicatorId).Select(m => m.NameRU).Contains(map.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                map.MapIntervals = map.MapIntervals.OrderBy(m => m.MinValue).ToList();
                map.MinValues = map.MapIntervals.Select(m => m.MinValue).ToArray();
                map.Colors = map.MapIntervals.Select(m => m.Color).ToArray();
                map.IndicatorIds = map.MapIndicators.Select(m => m.IndicatorId).ToArray();
                map.IndicatorColors = map.MapIndicators.Select(m => m.Color).ToArray();
                _context.Add(map);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Создание Карты",
                    New = $"{map.NameEN}; {map.NameKK}; {map.NameRU}",
                    Old = ""
                });
                await _context.SaveChangesAsync();
                if(!string.IsNullOrEmpty(backlink))
                {
                    if (backlink.ToLower().Contains("map"))
                    {
                        return Redirect(backlink);
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["IndicatorBlocIds"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", map.BlocId);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode).Where(g => g.BlocId == map.BlocId), "Id", "NameCode", map.GroupId);
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.GroupId == map.GroupId).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString(), Selected = i.Id == map.IndicatorId });
            if (map.MapIntervals == null)
            {
                map.MapIntervals = new List<MapInterval>();
            }
            if (map.MapIndicators == null)
            {
                map.MapIndicators = new List<MapIndicator>();
            }
            List<SelectList> IndicatorsIndicatorId = new List<SelectList>();
            List<SelectList> IndicatorsGroupId = new List<SelectList>();
            List<SelectList> IndicatorsBlocId = new List<SelectList>();
            for (int i = 0; i < map.MapIndicators.Count; i++)
            {
                IndicatorsIndicatorId.Add(new SelectList(_context.Indicator.Where(ind => ind.GroupId == map.MapIndicators[i].GroupId).OrderBy(ind => ind.NameCode).ToList(), "Id", "NameCode", map.MapIndicators[i].IndicatorId));
                IndicatorsGroupId.Add(new SelectList(_context.Group.Where(g => g.BlocId == map.MapIndicators[i].BlocId).OrderBy(g => g.NameCode).ToList(), "Id", "NameCode", map.MapIndicators[i].GroupId));
                IndicatorsBlocId.Add(new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", map.MapIndicators[i].BlocId));
            }
            ViewBag.IndicatorsIndicatorId = IndicatorsIndicatorId;
            ViewBag.IndicatorsGroupId = IndicatorsGroupId;
            ViewBag.IndicatorsBlocId = IndicatorsBlocId;
            return View(map);
        }

        // GET: Maps/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var map = await _context.Map.SingleOrDefaultAsync(m => m.Id == id);
            if (map == null)
            {
                return NotFound();
            }
            map.MapIntervals = new List<MapInterval>();
            for (int i = 0; i < map.Colors.Count(); i++)
            {
                map.MapIntervals.Add(new MapInterval()
                {
                    Color = map.Colors[i],
                    MinValue = map.MinValues[i]
                });
            }
            map.MapIndicators = new List<MapIndicator>();
            for (int i = 0; i < map.IndicatorColors.Count(); i++)
            {
                map.MapIndicators.Add(new MapIndicator()
                {
                    Color = map.IndicatorColors[i],
                    IndicatorId = map.IndicatorIds[i]
                });
            }
            ViewData["IndicatorBlocIds"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            map.Indicator = _context.Indicator.FirstOrDefault(i => i.Id == map.IndicatorId);
            map.Indicator.Group = _context.Group.FirstOrDefault(g => g.Id == map.Indicator.GroupId);
            if(map.Indicator.Group!=null)
            {
                map.Indicator.Group.Bloc = _context.Bloc.FirstOrDefault(b => b.Id == map.Indicator.Group.BlocId);
                map.GroupId = map.Indicator.Group.Id;
                map.BlocId = map.Indicator.Group.Bloc.Id;
            }            
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", map.BlocId);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode).Where(g => g.BlocId == map.BlocId), "Id", "NameCode", map.GroupId);
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.GroupId == map.GroupId).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString(), Selected = i.Id == map.IndicatorId });
            if (map.MapIntervals == null)
            {
                map.MapIntervals = new List<MapInterval>();
            }
            if (map.MapIndicators == null)
            {
                map.MapIndicators = new List<MapIndicator>();
            }
            List<SelectList> IndicatorsIndicatorId = new List<SelectList>();
            List<SelectList> IndicatorsGroupId = new List<SelectList>();
            List<SelectList> IndicatorsBlocId = new List<SelectList>();
            for (int i = 0; i < map.MapIndicators.Count; i++)
            {
                Indicator indicator = _context.Indicator.FirstOrDefault(ind => ind.Id == map.MapIndicators[i].IndicatorId);
                Group group = _context.Group.FirstOrDefault(g => g.Id == indicator.GroupId);
                Bloc bloc = _context.Bloc.FirstOrDefault(b => b.Id == group.BlocId);
                map.MapIndicators[i].GroupId = group.Id;
                map.MapIndicators[i].BlocId = bloc.Id;
                IndicatorsIndicatorId.Add(new SelectList(_context.Indicator.Where(ind => ind.GroupId == map.MapIndicators[i].GroupId).OrderBy(ind => ind.NameCode).ToList(), "Id", "NameCode", map.MapIndicators[i].IndicatorId));
                IndicatorsGroupId.Add(new SelectList(_context.Group.Where(g => g.BlocId == map.MapIndicators[i].BlocId).OrderBy(g => g.NameCode).ToList(), "Id", "NameCode", map.MapIndicators[i].GroupId));
                IndicatorsBlocId.Add(new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", map.MapIndicators[i].BlocId));
            }
            ViewBag.IndicatorsIndicatorId = IndicatorsIndicatorId;
            ViewBag.IndicatorsBlocId = IndicatorsBlocId;
            ViewBag.IndicatorsGroupId = IndicatorsGroupId;
            return View(map);
        }

        // POST: Maps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(Map map, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (map.MapIntervals == null)
            {
                map.MapIntervals = new List<MapInterval>();
            }
            for (int i = map.MapIntervals.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(map.MapIntervals[i].Color))
                {
                    map.MapIntervals.RemoveAt(i);
                }
            }
            if (map.MapIndicators == null)
            {
                map.MapIndicators = new List<MapIndicator>();
            }
            for (int i = map.MapIndicators.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(map.MapIndicators[i].Color))
                {
                    map.MapIndicators.RemoveAt(i);
                }
            }
            var maps = _context.Map
                .AsNoTracking()
                .ToList();
            if (maps.Where(m => m.IndicatorId == map.IndicatorId && m.Id != map.Id).Select(m => m.NameEN).Contains(map.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (maps.Where(m => m.IndicatorId == map.IndicatorId && m.Id != map.Id).Select(m => m.NameKK).Contains(map.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (maps.Where(m => m.IndicatorId == map.IndicatorId && m.Id != map.Id).Select(m => m.NameRU).Contains(map.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                map.MapIntervals = map.MapIntervals.OrderBy(m => m.MinValue).ToList();
                map.MinValues = map.MapIntervals.Select(m => m.MinValue).ToArray();
                map.Colors = map.MapIntervals.Select(m => m.Color).ToArray();
                map.IndicatorIds = map.MapIndicators.Select(m => m.IndicatorId).ToArray();
                map.IndicatorColors = map.MapIndicators.Select(m => m.Color).ToArray();
                //_context.Add(map);
                Map map_old = _context.Map.AsNoTracking().FirstOrDefault(m => m.Id == map.Id);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Редактирование Карты",
                    New = $"{map.NameEN}; {map.NameKK}; {map.NameRU}",
                    Old = $"{map_old.NameEN}; {map_old.NameKK}; {map_old.NameRU}"
                });
                _context.Update(map);
                await _context.SaveChangesAsync();
                if (backlink.ToLower().Contains("map"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            ViewData["IndicatorBlocIds"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", map.BlocId);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode).Where(g => g.BlocId == map.BlocId), "Id", "NameCode", map.GroupId);
            ViewBag.IndicatorId = _context.Indicator.Where(i => i.GroupId == map.GroupId).Select(i => new SelectListItem { Text = i.NameCode, Value = i.Id.ToString(), Selected = i.Id == map.IndicatorId });
            if (map.MapIntervals == null)
            {
                map.MapIntervals = new List<MapInterval>();
            }
            if (map.MapIndicators == null)
            {
                map.MapIndicators = new List<MapIndicator>();
            }
            List<SelectList> IndicatorsIndicatorId = new List<SelectList>();
            List<SelectList> IndicatorsGroupId = new List<SelectList>();
            List<SelectList> IndicatorsBlocId = new List<SelectList>();
            for (int i = 0; i < map.MapIndicators.Count; i++)
            {
                IndicatorsIndicatorId.Add(new SelectList(_context.Indicator.Where(ind => ind.GroupId == map.MapIndicators[i].GroupId).OrderBy(ind => ind.NameCode).ToList(), "Id", "NameCode", map.MapIndicators[i].IndicatorId));
                IndicatorsGroupId.Add(new SelectList(_context.Group.Where(g => g.BlocId == map.MapIndicators[i].BlocId).OrderBy(g => g.NameCode).ToList(), "Id", "NameCode", map.MapIndicators[i].GroupId));
                IndicatorsBlocId.Add(new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", map.MapIndicators[i].BlocId));
            }
            ViewBag.IndicatorsIndicatorId = IndicatorsIndicatorId;
            ViewBag.IndicatorsBlocId = IndicatorsBlocId;
            ViewBag.IndicatorsGroupId = IndicatorsGroupId;
            return View(map);
        }

        // GET: Maps/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var map = await _context.Map
                .Include(i => i.Indicator)
                .Include(i => i.Indicator.Group)
                .Include(i => i.Indicator.Group.Bloc)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (map == null)
            {
                return NotFound();
            }
            map.MapIntervals = new List<MapInterval>();
            for (int i = 0; i < map.Colors.Count(); i++)
            {
                map.MapIntervals.Add(new MapInterval()
                {
                    Color = map.Colors[i],
                    MinValue = map.MinValues[i]
                });
            }
            map.MapIndicators = new List<MapIndicator>();
            for (int i = 0; i < map.IndicatorColors.Count(); i++)
            {
                Indicator indicator = _context.Indicator
                    .Include(ind => ind.Group)
                    .Include(ind => ind.Group.Bloc)
                    .FirstOrDefault(ind => ind.Id == map.IndicatorIds[i]);
                map.MapIndicators.Add(new MapIndicator()
                {
                    Color = map.IndicatorColors[i],
                    IndicatorId = map.IndicatorIds[i],
                    Bloc = indicator.Group.Bloc,
                    Group = indicator.Group,
                    Indicator = indicator
                });
            }
            return View(map);
        }

        // POST: Maps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id, string backlink)
        {
            ViewBag.BackLink = backlink;
            var map = await _context.Map.SingleOrDefaultAsync(m => m.Id == id);
            _context.Log.Add(new Log()
            {
                DateTime = DateTime.Now,
                Email = User.Identity.Name,
                Operation = "Редактирование Карты",
                New = "",
                Old = $"{map.NameEN}; {map.NameKK}; {map.NameRU}"
            });
            _context.Map.Remove(map);
            await _context.SaveChangesAsync();
            if(!string.IsNullOrEmpty(backlink))
            {
                if (backlink.ToLower().Contains("map"))
                {
                    return Redirect(backlink);
                }
            }
            return RedirectToAction("Index");
        }

        private bool MapExists(int id)
        {
            return _context.Map.Any(e => e.Id == id);
        }

        [HttpPost]
        public JsonResult GetGroupsByBloc(int BlocId)
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

        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var map = await _context.Map
                .Include(m => m.Indicator)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (map == null)
            {
                return NotFound();
            }
            ViewBag.MapId = id;
            ViewBag.YearMin = Startup.Configuration["YearMin"];
            //ViewBag.YearCurrent = DateTime.Today.Year;
            ViewBag.YearMax = _context.Region.Select(r => r.Year).Max();
            ViewBag.MaxZoom = Startup.Configuration["MaxZoom"];
            map.MapIndicators = new List<MapIndicator>();
            for (int i = 0; i < map.IndicatorIds.Count(); i++)
            {
                map.MapIndicators.Add(new MapIndicator()
                {
                    Indicator = _context.Indicator.FirstOrDefault(ind => ind.Id == map.IndicatorIds[i])
                });
            }
            List<SelectListItem> MapSources = new List<SelectListItem>();
            var MapSourcesData = new[]{
                    new SelectListItem{ Value="OpenStreetMap",Text="Open Street Map", Selected = true},
                    new SelectListItem{ Value="ArcGIS",Text="ArcGIS"},
                    new SelectListItem{ Value="Bing",Text="Bing"},
                    new SelectListItem{ Value="BingAerial",Text="Bing Aerial"},
                    new SelectListItem{ Value="Stamen",Text="Stamen"}
                    };
            MapSources = MapSourcesData.OrderBy(s => s.Text).ToList();
            ViewBag.MapSources = MapSources;
            ViewBag.KZ = _sharedLocalizer["Kazakhstan"];
            return View(map);
        }

        [HttpPost]
        public ActionResult GetMap(int? id)
        {
            Map map = _context.Map.SingleOrDefault(m => m.Id == id);
            int yearmax = _context.Region.Select(r => r.Year).Max();
            string[] layers = new string[yearmax - Convert.ToInt32(Startup.Configuration["YearMin"]) + 1];
            var regionsall = _context.Region.ToList();
            //List<IndicatorValue> indicatorvalues = 
            for (int year = Convert.ToInt32(Startup.Configuration["YearMin"]); year <= yearmax; year++)
            {
                int i = year - Convert.ToInt32(Startup.Configuration["YearMin"]);
                var regions = regionsall.Where(r => r.Year == year).ToList();

                foreach (var region in regions)
                {
                    foreach (var regionall in regionsall)
                    {
                        if (region.Coordinates == regionall.Coordinates && region.Year > regionall.Year)
                        {
                            region.Coordinates = regionall.Id.ToString();
                            break;
                        }
                    }
                }
                var indicatorValues = _context.IndicatorValue.Where(iv => iv.Year == year && iv.IndicatorId == map.IndicatorId).ToList();
                JObject layer = JObject.FromObject(new
                {
                    type = "FeatureCollection",
                    crs = new
                    {
                        type = "name",
                        properties = new
                        {
                            name = "urn:ogc:def:crs:EPSG::3857"
                        }
                    },
                    features = from region in regions
                        select new
                        {
                            type = "Feature",
                            properties = new
                            {
                                Id = region.Id,
                                Code = region.Code,
                                Name = region.Name,
                                Year = region.Year,
                                CoordinatesId = region.Coordinates !=null ? (region.Coordinates.Length < 100 ? region.Coordinates : "") : "",
                                Value = indicatorValues.FirstOrDefault(iv => iv.RegionId == region.Id) == null ? (decimal?)null : indicatorValues.FirstOrDefault(iv => iv.RegionId == region.Id).Value,
                                Values = _context.IndicatorValue
                                    .Where(iv => iv.RegionId == region.Id && map.IndicatorIds.Contains(iv.IndicatorId))
                                    .OrderBy(iv => map.IndicatorIds.ToList().IndexOf(iv.IndicatorId))
                                    .Select(iv => iv.Value).ToArray()
                            },
                            geometry = new
                            {
                                type = "Polygon",
                                coordinates = region.Coordinates != null ? (region.Coordinates.Length < 100 ? "[[[0.0,1.1],[2.2,3.3],[4.4,5.5],[6.6,7.7]]]" : region.Coordinates.Replace("\r\n", "")) : "[[[7700000,6130000],[7700001,6130000],[7700000,6130001],[7700001,6130001]]]"
                            }
                        }
                });
                layers[i] = layer.ToString().Replace("\"[", "[").Replace("]\"", "]").Replace("  ", "");
            }
            decimal? chartmax = _context.IndicatorValue
                .Where(iv => map.IndicatorIds.Contains(iv.IndicatorId))
                .Select(iv => iv.Value)
                .Max();
            return Json(new
            {
                layers = layers,
                map = map,
                chartmax = chartmax
            });
        }

        [HttpPost]
        public ActionResult GetIndicatorValueMaxMin(int? IndicatorId)
        {
            decimal? min = _context.IndicatorValue.Include(i => i.Region).Where(i => i.IndicatorId == IndicatorId && i.Region.Code != Startup.Configuration["KazakhstanCode"]).Select(i => i.Value).Min(),
                max = _context.IndicatorValue.Include(i => i.Region).Where(i => i.IndicatorId == IndicatorId && i.Region.Code != Startup.Configuration["KazakhstanCode"]).Select(i => i.Value).Max();
            return Json(new
            {
                min = min,
                max = max
            });
        }
    }
}