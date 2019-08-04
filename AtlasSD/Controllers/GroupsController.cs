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
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public GroupsController(ApplicationDbContext context, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Groups
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder, int? BlocId, string Code, string NameEN, string NameKK, string NameRU, int? Page)
        {
            var groups = _context.Group
                .Include(g => g.Bloc)
                .Where(g => true);

            ViewBag.BlocIdFilter = BlocId;
            ViewBag.CodeFilter = Code;
            ViewBag.NameENFilter = NameEN;
            ViewBag.NameKKFilter = NameKK;
            ViewBag.NameRUFilter = NameRU;

            ViewBag.BlocSort = SortOrder == "Bloc" ? "BlocDesc" : "Bloc";
            ViewBag.CodeSort = SortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";

            if (BlocId != null)
            {
                groups = groups.Where(g => g.BlocId == BlocId);
            }
            if (!string.IsNullOrEmpty(Code))
            {
                groups = groups.Where(g => g.Code.ToLower().Contains(Code.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                groups = groups.Where(g => g.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                groups = groups.Where(g => g.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                groups = groups.Where(g => g.NameRU.ToLower().Contains(NameRU.ToLower()));
            }

            switch (SortOrder)
            {
                case "Bloc":
                    groups = groups.OrderBy(g => g.Bloc.Name);
                    break;
                case "BlocDesc":
                    groups = groups.OrderByDescending(g => g.Bloc.Name);
                    break;
                case "Code":
                    groups = groups.OrderBy(g => g.Code);
                    break;
                case "CodeDesc":
                    groups = groups.OrderByDescending(g => g.Code);
                    break;
                case "NameEN":
                    groups = groups.OrderBy(g => g.NameEN);
                    break;
                case "NameENDesc":
                    groups = groups.OrderByDescending(g => g.NameEN);
                    break;
                case "NameKK":
                    groups = groups.OrderBy(g => g.NameKK);
                    break;
                case "NameKKDesc":
                    groups = groups.OrderByDescending(g => g.NameKK);
                    break;
                case "NameRU":
                    groups = groups.OrderBy(g => g.NameRU);
                    break;
                case "NameRUDesc":
                    groups = groups.OrderByDescending(g => g.NameRU);
                    break;
                default:
                    groups = groups.OrderBy(g => g.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(groups.Count(), Page);

            var viewModel = new GroupIndexPageViewModel
            {
                Items = groups.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            ViewBag.BlocId = new SelectList(_context.Bloc.OrderBy(b => b.Name), "Id", "Name");

            return View(viewModel);
        }

        // GET: Groups/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Group
                .Include(g => g.Bloc)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // GET: Groups/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,BlocId,NameEN,NameKK,NameRU")] Group group, string backlink)
        {
            ViewBag.BackLink = backlink;
            var groups = _context.Group
                .Where(g => g.BlocId == group.BlocId)
                .Include(g => g.Bloc)
                .ToList();
            if (groups.Select(g => g.NameEN).Contains(group.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (groups.Select(g => g.NameKK).Contains(group.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (groups.Select(g => g.NameRU).Contains(group.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                for (int i = 1; i < 100; i++)
                {
                    if (groups.Count(g => g.Code == g.Bloc.Code + i.ToString().PadLeft(2, '0')) == 0)
                    {
                        group.Bloc = _context.Bloc.FirstOrDefault(b => b.Id == group.BlocId);
                        group.Code = group.Bloc.Code + i.ToString().PadLeft(2, '0');
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(group.Code))
                {
                    _context.Add(group);
                    _context.Log.Add(new Log()
                    {
                        DateTime = DateTime.Now,
                        Email = User.Identity.Name,
                        Operation = "Создание Группы показателей",
                        New = $"{group.NameEN}; {group.NameKK}; {group.NameRU}",
                        Old = ""
                    });
                    await _context.SaveChangesAsync();
                }
                if (backlink.ToLower().Contains("group"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", group.BlocId);
            return View(group);
        }

        // GET: Groups/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Group.SingleOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", group.BlocId);
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlocId,NameEN,NameKK,NameRU,Code")] Group group, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (id != group.Id)
            {
                return NotFound();
            }
            ViewBag.BackLink = backlink;
            var groups = _context.Group
                .AsNoTracking()
                .Where(g => g.BlocId == group.BlocId)
                .Include(g => g.Bloc)
                .ToList();
            if (groups.Where(g => g.Id != group.Id).Select(g => g.NameEN).Contains(group.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (groups.Where(g => g.Id != group.Id).Select(g => g.NameKK).Contains(group.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (groups.Where(g => g.Id != group.Id).Select(g => g.NameRU).Contains(group.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    group.Bloc = _context.Bloc.AsNoTracking().FirstOrDefault(b => b.Id == group.BlocId);
                    for (int i = 1; i < 100; i++)
                    {
                        if (groups.Count(g => g.Code == g.Bloc.Code + i.ToString().PadLeft(2, '0')) == 0 || group.Bloc.Code + i.ToString().PadLeft(2, '0') == group.Code)
                        {
                            group.Code = group.Bloc.Code + i.ToString().PadLeft(2, '0');
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(group.Code))
                    {
                        Group group_old = _context.Group.AsNoTracking().FirstOrDefault(g => g.Id == group.Id);
                        _context.Log.Add(new Log()
                        {
                            DateTime = DateTime.Now,
                            Email = User.Identity.Name,
                            Operation = "Редактирование Группы показателей",
                            New = $"{group.NameEN}; {group.NameKK}; {group.NameRU}",
                            Old = $"{group_old.NameEN}; {group_old.NameKK}; {group_old.NameRU}"
                        });
                        _context.Update(group);
                        var indicators = _context.Indicator
                            .Where(i => i.GroupId == id)
                            .Include(i => i.Group)
                            .Include(i => i.Group.Bloc)
                            .OrderBy(i => i.Code)
                            .ToList();
                        for (int ii = 0; ii < indicators.Count(); ii++)
                        {
                            for (int i = 1; i < 100; i++)
                            {
                                if (indicators.Count(ind => ind.Code == ind.Group.Code + i.ToString().PadLeft(2, '0')) == 0 ||
                                    indicators[ii].Group.Code + i.ToString().PadLeft(2, '0') == indicators[ii].Code)
                                {
                                    indicators[ii].Code = indicators[ii].Group.Code + i.ToString().PadLeft(2, '0');
                                    break;
                                }
                            }
                            _context.Update(indicators[ii]);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(group.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (backlink.ToLower().Contains("group"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", group.BlocId);
            return View(group);
        }

        // GET: Groups/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Group
                .Include(g => g.Bloc)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }
            ViewBag.Indicators = _context.Indicator.Where(i => i.GroupId == id).OrderBy(i => i.NameCode);
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (_context.Indicator.FirstOrDefault(i => i.GroupId == id) == null)
            {
                var group = await _context.Group.SingleOrDefaultAsync(m => m.Id == id);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Удаление Группы показателей",
                    New = "",
                    Old = $"{group.NameEN}; {group.NameKK}; {group.NameRU}"
                });
                _context.Group.Remove(group);
                await _context.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(backlink))
            {
                if (backlink.ToLower().Contains("group"))
                {
                    return Redirect(backlink);
                }
            }
                
            return RedirectToAction("Index");
        }

        private bool GroupExists(int id)
        {
            return _context.Group.Any(e => e.Id == id);
        }
    }
}
