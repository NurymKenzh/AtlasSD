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
    public class BlocsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public BlocsController(ApplicationDbContext context, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Blocs
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder, string Code, string NameEN, string NameKK, string NameRU, int? Page)
        {
            var blocs = _context.Bloc
                .Where(b => true);

            ViewBag.CodeFilter = Code;
            ViewBag.NameENFilter = NameEN;
            ViewBag.NameKKFilter = NameKK;
            ViewBag.NameRUFilter = NameRU;

            ViewBag.CodeSort = SortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";

            if (!string.IsNullOrEmpty(Code))
            {
                blocs = blocs.Where(b => b.Code.ToLower().Contains(Code.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                blocs = blocs.Where(b => b.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                blocs = blocs.Where(b => b.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                blocs = blocs.Where(b => b.NameRU.ToLower().Contains(NameRU.ToLower()));
            }

            switch (SortOrder)
            {
                case "Code":
                    blocs = blocs.OrderBy(b => b.Code);
                    break;
                case "CodeDesc":
                    blocs = blocs.OrderByDescending(b => b.Code);
                    break;
                case "NameEN":
                    blocs = blocs.OrderBy(b => b.NameEN);
                    break;
                case "NameENDesc":
                    blocs = blocs.OrderByDescending(b => b.NameEN);
                    break;
                case "NameKK":
                    blocs = blocs.OrderBy(b => b.NameKK);
                    break;
                case "NameKKDesc":
                    blocs = blocs.OrderByDescending(b => b.NameKK);
                    break;
                case "NameRU":
                    blocs = blocs.OrderBy(b => b.NameRU);
                    break;
                case "NameRUDesc":
                    blocs = blocs.OrderByDescending(b => b.NameRU);
                    break;
                default:
                    blocs = blocs.OrderBy(b => b.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(blocs.Count(), Page);

            var viewModel = new BlocIndexPageViewModel
            {
                Items = blocs.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: Blocs/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloc = await _context.Bloc
                .SingleOrDefaultAsync(m => m.Id == id);
            if (bloc == null)
            {
                return NotFound();
            }

            return View(bloc);
        }

        // GET: Blocs/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blocs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,NameRU,NameEN,NameKK")] Bloc bloc, string backlink)
        {
            ViewBag.BackLink = backlink;
            var blocs = _context.Bloc.AsNoTracking().ToList();
            if(blocs.Select(b => b.NameEN).Contains(bloc.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (blocs.Select(b => b.NameKK).Contains(bloc.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (blocs.Select(b => b.NameRU).Contains(bloc.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                for (int i = 0; i < Startup.Configuration["BlocCodes"].Length; i++)
                {
                    if (blocs.Count(b => b.Code == Startup.Configuration["BlocCodes"].Substring(i, 1)) == 0)
                    {
                        bloc.Code = Startup.Configuration["BlocCodes"].Substring(i, 1);
                        break;
                    }
                }
                _context.Add(bloc);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Создание Блока",
                    New = $"{bloc.NameEN}; {bloc.NameKK}; {bloc.NameRU}",
                    Old = ""
                });
                await _context.SaveChangesAsync();
                if (backlink.ToLower().Contains("bloc"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            return View(bloc);
        }

        // GET: Blocs/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloc = await _context.Bloc.SingleOrDefaultAsync(m => m.Id == id);
            if (bloc == null)
            {
                return NotFound();
            }
            return View(bloc);
        }

        // POST: Blocs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameRU,NameEN,NameKK,Code")] Bloc bloc, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (id != bloc.Id)
            {
                return NotFound();
            }
            var blocs = _context.Bloc.AsNoTracking().ToList();
            if (blocs.Where(b => b.Id != bloc.Id).Select(b => b.NameEN).Contains(bloc.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (blocs.Where(b => b.Id != bloc.Id).Select(b => b.NameKK).Contains(bloc.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (blocs.Where(b => b.Id != bloc.Id).Select(b => b.NameRU).Contains(bloc.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(bloc.Code))
                    {
                        bloc.Code = _context.Bloc.FirstOrDefault(b => b.Id == bloc.Id).Code;
                    }
                    Bloc bloc_old = _context.Bloc.AsNoTracking().FirstOrDefault(b => b.Id == bloc.Id);
                    _context.Log.Add(new Log()
                    {
                        DateTime = DateTime.Now,
                        Email = User.Identity.Name,
                        Operation = "Редактирование Блока",
                        New = $"{bloc.NameEN}; {bloc.NameKK}; {bloc.NameRU}",
                        Old = $"{bloc_old.NameEN}; {bloc_old.NameKK}; {bloc_old.NameRU}"
                    });
                    _context.Update(bloc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlocExists(bloc.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (backlink.ToLower().Contains("bloc"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            return View(bloc);
        }

        // GET: Blocs/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloc = await _context.Bloc
                .SingleOrDefaultAsync(m => m.Id == id);
            if (bloc == null)
            {
                return NotFound();
            }
            ViewBag.Groups = _context.Group.Where(g => g.BlocId == id).OrderBy(g => g.NameCode);
            return View(bloc);
        }

        // POST: Blocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id, string backlink)
        {
            ViewBag.BackLink = backlink;
            if(_context.Group.FirstOrDefault(g => g.BlocId == id)==null)
            {
                var bloc = await _context.Bloc.SingleOrDefaultAsync(m => m.Id == id);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Удаление Блока",
                    New = "",
                    Old = $"{bloc.NameEN}; {bloc.NameKK}; {bloc.NameRU}"
                });
                _context.Bloc.Remove(bloc);
                await _context.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(backlink))
            {
                if (backlink.ToLower().Contains("bloc"))
                {
                    return Redirect(backlink);
                }
            }
            return RedirectToAction("Index");
        }

        private bool BlocExists(int id)
        {
            return _context.Bloc.Any(e => e.Id == id);
        }
    }
}
