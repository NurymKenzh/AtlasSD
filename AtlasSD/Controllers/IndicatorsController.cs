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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.Emit;
using System.Runtime.Loader;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;

namespace AtlasSD.Controllers
{
    public class IndicatorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public IndicatorsController(ApplicationDbContext context, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Indicators
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder, string Type, int? BlocId, int? GroupId, string Code, string NameEN, string NameKK, string NameRU, int? Page)
        {
            string language = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;

            var indicators = _context.Indicator
                .Include(i => i.Group)
                .Include(i => i.Group.Bloc)
                .Where(i => true);

            ViewBag.TypeFilter = Type;
            ViewBag.BlocIdFilter = BlocId;
            ViewBag.GroupIdFilter = GroupId;
            ViewBag.CodeFilter = Code;
            ViewBag.NameENFilter = NameEN;
            ViewBag.NameKKFilter = NameKK;
            ViewBag.NameRUFilter = NameRU;

            ViewBag.TypeSort = SortOrder == "Type" ? "TypeDesc" : "Type";
            ViewBag.BlocSort = SortOrder == "Bloc" ? "BlocDesc" : "Bloc";
            ViewBag.GroupSort = SortOrder == "Group" ? "GroupDesc" : "Group";
            ViewBag.CodeSort = SortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";

            if (!string.IsNullOrEmpty(Type))
            {
                indicators = indicators.Where(g => g.Type.ToLower().Contains(Type.ToLower()));
            }
            if (BlocId != null)
            {
                indicators = indicators.Where(i => i.Group.BlocId == BlocId);
            }
            if (GroupId != null)
            {
                indicators = indicators.Where(i => i.GroupId == GroupId);
            }
            if (!string.IsNullOrEmpty(Code))
            {
                indicators = indicators.Where(g => g.Code.ToLower().Contains(Code.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                indicators = indicators.Where(i => i.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                indicators = indicators.Where(i => i.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                indicators = indicators.Where(i => i.NameRU.ToLower().Contains(NameRU.ToLower()));
            }

            switch (SortOrder)
            {
                case "Type":
                    indicators = indicators.OrderBy(i => _sharedLocalizer[i.Type]);
                    break;
                case "TypeDesc":
                    indicators = indicators.OrderByDescending(i => _sharedLocalizer[i.Type]);
                    break;
                case "Bloc":
                    switch (language)
                    {
                        case "en":
                            indicators = indicators.OrderBy(i => i.Group.Bloc.NameEN);
                            break;
                        case "kk":
                            indicators = indicators.OrderBy(i => i.Group.Bloc.NameKK);
                            break;
                        default:
                            indicators = indicators.OrderBy(i => i.Group.Bloc.NameRU);
                            break;
                    }
                    break;
                case "BlocDesc":
                    switch (language)
                    {
                        case "en":
                            indicators = indicators.OrderByDescending(i => i.Group.Bloc.NameEN);
                            break;
                        case "kk":
                            indicators = indicators.OrderByDescending(i => i.Group.Bloc.NameKK);
                            break;
                        default:
                            indicators = indicators.OrderByDescending(i => i.Group.Bloc.NameRU);
                            break;
                    }
                    break;
                case "Group":
                    switch (language)
                    {
                        case "en":
                            indicators = indicators.OrderBy(i => i.Group.NameEN);
                            break;
                        case "kk":
                            indicators = indicators.OrderBy(i => i.Group.NameKK);
                            break;
                        default:
                            indicators = indicators.OrderBy(i => i.Group.NameRU);
                            break;
                    }
                    break;
                case "GroupDesc":
                    switch (language)
                    {
                        case "en":
                            indicators = indicators.OrderByDescending(i => i.Group.NameEN);
                            break;
                        case "kk":
                            indicators = indicators.OrderByDescending(i => i.Group.NameKK);
                            break;
                        default:
                            indicators = indicators.OrderByDescending(i => i.Group.NameRU);
                            break;
                    }
                    break;
                case "Code":
                    indicators = indicators.OrderBy(g => g.Code);
                    break;
                case "CodeDesc":
                    indicators = indicators.OrderByDescending(g => g.Code);
                    break;
                case "NameEN":
                    indicators = indicators.OrderBy(i => i.NameEN);
                    break;
                case "NameENDesc":
                    indicators = indicators.OrderByDescending(i => i.NameEN);
                    break;
                case "NameKK":
                    indicators = indicators.OrderBy(i => i.NameKK);
                    break;
                case "NameKKDesc":
                    indicators = indicators.OrderByDescending(i => i.NameKK);
                    break;
                case "NameRU":
                    indicators = indicators.OrderBy(i => i.NameRU);
                    break;
                case "NameRUDesc":
                    indicators = indicators.OrderByDescending(i => i.NameRU);
                    break;
                default:
                    indicators = indicators.OrderBy(i => i.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(indicators.Count(), Page);

            var viewModel = new IndicatorIndexPageViewModel
            {
                Items = indicators.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            string[] indicatorTypes = new string[3] { Startup.Configuration["Regular"], Startup.Configuration["Calculated"], Startup.Configuration["Integral"] };
            ViewData["IndicatorTypes"] = new SelectList(indicatorTypes.Select((text, index) => new SelectListItem { Text = _sharedLocalizer[text], Value = text }), "Value", "Text").OrderBy(s => s.Text);
            ViewBag.BlocId = new SelectList(_context.Bloc.OrderBy(b => b.Name), "Id", "Name");
            ViewBag.GroupId = new SelectList(_context.Group.OrderBy(g => g.Name), "Id", "Name");

            return View(viewModel);
        }

        // GET: Indicators/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indicator = await _context.Indicator
                .Include(i => i.Group)
                .Include(i => i.Group.Bloc)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (indicator == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(indicator.Formula))
            {
                var indicatorsall = _context.Indicator.ToList();
                foreach (var ind in indicatorsall)
                {
                    if (!string.IsNullOrEmpty(ind.Code))
                    {
                        indicator.Formula = indicator.Formula.Replace($"{Startup.Configuration["FormulaIdFirstChar"]}{ind.Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}", ind.Code);
                    }
                }
            }
            return View(indicator);
        }

        // GET: Indicators/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            string[] indicatorTypes = new string[3] { Startup.Configuration["Regular"], Startup.Configuration["Calculated"], Startup.Configuration["Integral"] };
            ViewData["IndicatorTypes"] = new SelectList(indicatorTypes.Select((text, index) => new SelectListItem { Text = _sharedLocalizer[text], Value = text }), "Value", "Text");
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            return View();
        }

        // POST: Indicators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,Type,GroupId,NameEN,NameKK,NameRU,Formula")] Indicator indicator, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (indicator.Type == Startup.Configuration["Integral"])
            {
                indicator.GroupId = null;
            }
            if(indicator.Type == Startup.Configuration["Regular"])
            {
                indicator.Formula = null;
            }
            List<IndicatorValue> indicatorValues = new List<IndicatorValue>();
            var indicatorsall = _context.Indicator.ToList();
            if(indicator.Type == Startup.Configuration["Calculated"] || indicator.Type == Startup.Configuration["Integral"])
            {
                indicator.Formula = string.IsNullOrEmpty(indicator.Formula) ? "" : indicator.Formula;
                string formula_test = indicator.Formula;
                if (!CheckFormula(formula_test, indicator.Type))
                {
                    ModelState.AddModelError("Formula", _sharedLocalizer["WrongFormula"]);
                }
                else
                {
                    formula_test = indicator.Formula;
                    foreach (var ind in indicatorsall)
                    {
                        if(!string.IsNullOrEmpty(ind.Code))
                        {
                            formula_test = formula_test.Replace(ind.Code, $"{Startup.Configuration["FormulaIdFirstChar"]}{ind.Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}");
                        }
                    }
                    indicatorValues = CalculateIndicatorValues(formula_test, 0);
                    if(indicatorValues.Count == 0)
                    {
                        ModelState.AddModelError("Formula", _sharedLocalizer["ErrorDuringFormulaExecuting"]);
                    }
                }
            }
            if (indicatorsall.Where(i => i.GroupId == indicator.GroupId).Select(i => i.NameEN).Contains(indicator.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (indicatorsall.Where(i => i.GroupId == indicator.GroupId).Select(i => i.NameKK).Contains(indicator.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (indicatorsall.Where(i => i.GroupId == indicator.GroupId).Select(i => i.NameRU).Contains(indicator.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                if(indicator.Type != Startup.Configuration["Integral"])
                {
                    var indicators = _context.Indicator
                        .Where(i => i.GroupId == indicator.GroupId)
                        .Include(i => i.Group)
                        .ToList();
                    for (int i = 1; i < 100; i++)
                    {
                        if (indicators.Count(ind => ind.Code == ind.Group.Code + i.ToString().PadLeft(2, '0')) == 0)
                        {
                            indicator.Group = _context.Group.FirstOrDefault(g => g.Id == indicator.GroupId);
                            indicator.Code = indicator.Group.Code + i.ToString().PadLeft(2, '0');
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(indicator.Code) || indicator.Type == Startup.Configuration["Integral"])
                {
                    if(!string.IsNullOrEmpty(indicator.Formula))
                    {
                        foreach (var ind in indicatorsall)
                        {
                            if (!string.IsNullOrEmpty(ind.Code))
                            {
                                indicator.Formula = indicator.Formula.Replace(ind.Code, $"{Startup.Configuration["FormulaIdFirstChar"]}{ind.Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}");
                            }
                        }
                    }
                    _context.Add(indicator);
                    _context.Log.Add(new Log()
                    {
                        DateTime = DateTime.Now,
                        Email = User.Identity.Name,
                        Operation = "Создание Показателя",
                        New = $"{indicator.NameEN}; {indicator.NameKK}; {indicator.NameRU}; {_sharedLocalizer[indicator.Type]}",
                        Old = ""
                    });
                    await _context.SaveChangesAsync();
                    for (int i = 0; i < indicatorValues.Count(); i++)
                    {
                        indicatorValues[i].IndicatorId = indicator.Id;
                        _context.Add(indicatorValues[i]);
                    }
                    await _context.SaveChangesAsync();
                }
                if (backlink.ToLower().Contains("indicator"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            string[] indicatorTypes = new string[3] { Startup.Configuration["Regular"], Startup.Configuration["Calculated"], Startup.Configuration["Integral"] };
            ViewData["IndicatorTypes"] = new SelectList(indicatorTypes.Select((text, index) => new SelectListItem { Text = _sharedLocalizer[text], Value = text }), "Value", "Text", indicator.Type);
            indicator.Group = _context.Group.FirstOrDefault(g => g.Id == indicator.GroupId);
            ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", indicator.Group != null ? indicator.Group.BlocId : 0);
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode", indicator.GroupId);
            return View(indicator);
        }

        // GET: Indicators/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indicator = await _context.Indicator.Include(i => i.Group).SingleOrDefaultAsync(m => m.Id == id);
            if (indicator == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(indicator.Formula))
            {
                var indicatorsall = _context.Indicator.ToList();
                foreach (var ind in indicatorsall)
                {
                    if (!string.IsNullOrEmpty(ind.Code))
                    {
                        indicator.Formula = indicator.Formula.Replace($"{Startup.Configuration["FormulaIdFirstChar"]}{ind.Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}", ind.Code);
                    }
                }
            }
            string[] indicatorTypes = new string[3] { Startup.Configuration["Regular"], Startup.Configuration["Calculated"], Startup.Configuration["Integral"] };
            ViewData["IndicatorTypes"] = new SelectList(indicatorTypes.Select((text, index) => new SelectListItem { Text = _sharedLocalizer[text], Value = text }), "Value", "Text", indicator.Type);
            if(indicator.Group!=null)
            {
                ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", indicator.Group.BlocId);
            }
            else
            {
                ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            }
            ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode", indicator.GroupId);
            ViewData["BlocIdPaste"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupIdPaste"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            return View(indicator);
        }

        // POST: Indicators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,GroupId,NameEN,NameKK,NameRU,Formula,Code")] Indicator indicator, string backlink)
        {
            ViewBag.BackLink = backlink;
            if (id != indicator.Id)
            {
                return NotFound();
            }
            if (indicator.Type == Startup.Configuration["Integral"])
            {
                indicator.GroupId = null;
            }
            if (indicator.Type == Startup.Configuration["Regular"])
            {
                indicator.Formula = null;
            }
            List<IndicatorValue> indicatorValues = new List<IndicatorValue>();
            var indicatorsall = _context.Indicator
                .AsNoTracking()
                .Where(i => i.Id != indicator.Id)
                .Include(i => i.Group)
                .ToList();
            // расчет значений индикатора
            if (indicator.Type == Startup.Configuration["Calculated"] || indicator.Type == Startup.Configuration["Integral"])
            {
                indicator.Formula = string.IsNullOrEmpty(indicator.Formula) ? "" : indicator.Formula;
                string formula_test = indicator.Formula;
                if (!CheckFormula(formula_test, indicator.Type))
                {
                    ModelState.AddModelError("Formula", _sharedLocalizer["WrongFormula"]);
                }
                else
                {
                    formula_test = indicator.Formula;
                    for (int i = 0; i < indicatorsall.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(indicatorsall[i].Code))
                        {
                            formula_test = formula_test.Replace(indicatorsall[i].Code, $"{Startup.Configuration["FormulaIdFirstChar"]}{indicatorsall[i].Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}");
                        }
                    }
                    indicatorValues = CalculateIndicatorValues(formula_test, indicator.Id);
                    if (indicatorValues.Count == 0)
                    {
                        ModelState.AddModelError("Formula", _sharedLocalizer["ErrorDuringFormulaExecuting"]);
                    }
                }
            }
            if (indicatorsall.Where(i => i.GroupId == indicator.GroupId && i.Id != indicator.Id).Select(i => i.NameEN).Contains(indicator.NameEN))
            {
                ModelState.AddModelError("NameEN", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (indicatorsall.Where(i => i.GroupId == indicator.GroupId && i.Id != indicator.Id).Select(i => i.NameKK).Contains(indicator.NameKK))
            {
                ModelState.AddModelError("NameKK", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (indicatorsall.Where(i => i.GroupId == indicator.GroupId && i.Id != indicator.Id).Select(i => i.NameRU).Contains(indicator.NameRU))
            {
                ModelState.AddModelError("NameRU", _sharedLocalizer["ErrorDublicateValue"]);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (indicator.Type != Startup.Configuration["Integral"])
                    {
                        var indicators = indicatorsall
                            .Where(i => i.GroupId == indicator.GroupId)
                            .ToList();
                        indicator.Group = _context.Group.AsNoTracking().FirstOrDefault(g => g.Id == indicator.GroupId);
                        for (int i = 1; i < 100; i++)
                        {
                            if (indicators.Count(ind => ind.Code == ind.Group.Code + i.ToString().PadLeft(2, '0')) == 0 ||
                                indicator.Group.Code + i.ToString().PadLeft(2, '0') == indicator.Code)
                            {
                                indicator.Code = indicator.Group.Code + i.ToString().PadLeft(2, '0');
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(indicator.Code) || indicator.Type == Startup.Configuration["Integral"])
                    {
                        if (!string.IsNullOrEmpty(indicator.Formula))
                        {
                            for(int i=0;i< indicatorsall.Count;i++)
                            {
                                if (!string.IsNullOrEmpty(indicatorsall[i].Code))
                                {
                                    if (!string.IsNullOrEmpty(indicatorsall[i].Code))
                                    {
                                        indicator.Formula = indicator.Formula.Replace(indicatorsall[i].Code, $"{Startup.Configuration["FormulaIdFirstChar"]}{indicatorsall[i].Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}");
                                    }
                                }
                            }
                        }
                        Indicator indicator_old = _context.Indicator.AsNoTracking().FirstOrDefault(ind => ind.Id == indicator.Id);
                        _context.Log.Add(new Log()
                        {
                            DateTime = DateTime.Now,
                            Email = User.Identity.Name,
                            Operation = "Редактирование Показателя",
                            New = $"{indicator.NameEN}; {indicator.NameKK}; {indicator.NameRU}; {_sharedLocalizer[indicator.Type]}",
                            Old = $"{indicator_old.NameEN}; {indicator_old.NameKK}; {indicator_old.NameRU}; {_sharedLocalizer[indicator_old.Type]}"
                        });
                        _context.Update(indicator);
                        await _context.SaveChangesAsync();
                        for (int i = 0; i < indicatorValues.Count(); i++)
                        {
                            if (indicatorValues[i].Id == 0)
                            {
                                _context.Add(indicatorValues[i]);
                            }
                            else
                            {
                                _context.Update(indicatorValues[i]);
                            }
                        }
                        await _context.SaveChangesAsync();
                        //// => расчет значений зависимых индикаторов
                        //var indicatorsrelated = _context.Indicator
                        //    .AsNoTracking()
                        //    .Where(i => i.Formula.Contains($"{Startup.Configuration["FormulaIdFirstChar"]}{id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}"))
                        //    .ToList();
                        //foreach (var indicatorrelated in indicatorsrelated)
                        //{
                        //    List<IndicatorValue> indicatorrelatedValues = new List<IndicatorValue>();
                        //    indicatorrelated.Formula = string.IsNullOrEmpty(indicatorrelated.Formula) ? "" : indicatorrelated.Formula;
                        //    string formula_test = indicatorrelated.Formula;
                        //    if (!CheckFormula(formula_test, indicatorrelated.Type))
                        //    {
                                
                        //    }
                        //    else
                        //    {
                        //        formula_test = indicatorrelated.Formula;
                        //        for (int i = 0; i < indicatorsall.Count; i++)
                        //        {
                        //            if (!string.IsNullOrEmpty(indicatorsall[i].Code))
                        //            {
                        //                formula_test = formula_test.Replace(indicatorsall[i].Code, $"{Startup.Configuration["FormulaIdFirstChar"]}{indicatorsall[i].Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}");
                        //            }
                        //        }
                        //        indicatorrelatedValues = CalculateIndicatorValues(formula_test, indicatorrelated.Id);
                        //    }
                        //    for (int i = 0; i < indicatorrelatedValues.Count(); i++)
                        //    {
                        //        if (indicatorrelatedValues[i].Id == 0)
                        //        {
                        //            _context.Add(indicatorrelatedValues[i]);
                        //        }
                        //        else
                        //        {
                        //            _context.Update(indicatorrelatedValues[i]);
                        //        }
                        //    }
                        //}
                        //// <=
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IndicatorExists(indicator.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (backlink.ToLower().Contains("indicator"))
                {
                    return Redirect(backlink);
                }
                return RedirectToAction("Index");
            }
            string[] indicatorTypes = new string[3] { Startup.Configuration["Regular"], Startup.Configuration["Calculated"], Startup.Configuration["Integral"] };
            ViewData["IndicatorTypes"] = new SelectList(indicatorTypes.Select((text, index) => new SelectListItem { Text = _sharedLocalizer[text], Value = text }), "Value", "Text", indicator.Type);
            indicator.Group = _context.Group.FirstOrDefault(g => g.Id == indicator.GroupId);
            if(indicator.Group!=null)
            {
                ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode", indicator.Group.BlocId);
                ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode", indicator.GroupId);
            }
            else
            {
                ViewData["BlocId"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
                ViewData["GroupId"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            }
            ViewData["BlocIdPaste"] = new SelectList(_context.Bloc.OrderBy(b => b.NameCode), "Id", "NameCode");
            ViewData["GroupIdPaste"] = new SelectList(_context.Group.OrderBy(g => g.NameCode), "Id", "NameCode");
            return View(indicator);
        }

        // GET: Indicators/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var indicator = await _context.Indicator
                .Include(i => i.Group)
                .Include(i => i.Group.Bloc)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (indicator == null)
            {
                return NotFound();
            }
            if(!string.IsNullOrEmpty(indicator.Formula))
            {
                var indicatorsall = _context.Indicator.ToList();
                foreach (var ind in indicatorsall)
                {
                    if (!string.IsNullOrEmpty(ind.Code))
                    {
                        indicator.Formula = indicator.Formula.Replace($"{Startup.Configuration["FormulaIdFirstChar"]}{ind.Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}", ind.Code);
                    }
                }
            }
            ViewBag.Indicators = _context.Indicator
                .Where(i => i.Formula.Contains($"{Startup.Configuration["FormulaIdFirstChar"]}{indicator.Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}"))
                .OrderBy(i => i.NameCode);
            ViewBag.Maps = _context.Map
                .Where(i => i.IndicatorId == id || i.IndicatorIds.Contains((int)id))
                .OrderBy(i => i.Name);
            return View(indicator);
        }

        // POST: Indicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id, string backlink)
        {
            ViewBag.BackLink = backlink;
            if((_context.Indicator
                .AsNoTracking()
                .Where(i => i.Formula.Contains($"{Startup.Configuration["FormulaIdFirstChar"]}{id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}"))
                .Count() == 0) &&
                (ViewBag.Maps = _context.Map
                .Where(i => i.IndicatorId == id || i.IndicatorIds.Contains((int)id))
                .Count() == 0))
            {
                var indicator = await _context.Indicator.SingleOrDefaultAsync(m => m.Id == id);
                _context.Log.Add(new Log()
                {
                    DateTime = DateTime.Now,
                    Email = User.Identity.Name,
                    Operation = "Удаление Показателя",
                    New = "",
                    Old = $"{indicator.NameEN}; {indicator.NameKK}; {indicator.NameRU}; {_sharedLocalizer[indicator.Type]}"
                });
                _context.Indicator.Remove(indicator);
                await _context.SaveChangesAsync();
            }
            if(!string.IsNullOrEmpty(backlink))
            {
                if (backlink.ToLower().Contains("indicator"))
                {
                    return Redirect(backlink);
                }
            }
            return RedirectToAction("Index");
        }

        private bool IndicatorExists(int id)
        {
            return _context.Indicator.Any(e => e.Id == id);
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

        [HttpPost]
        public JsonResult GetIndicatorByGroupPaste(int GroupId, int IndicatorId)
        {
            var indicators = _context.Indicator.Where(i => i.GroupId == GroupId && i.Id != IndicatorId && i.Type == Startup.Configuration["Regular"]).ToArray().OrderBy(i => i.NameCode);
            JsonResult result = new JsonResult(indicators);
            return result;
        }

        [HttpPost]
        public JsonResult GetReferenceByIndicatorPaste(string IndicatorCode)
        {
            Indicator indicator = _context.Indicator.FirstOrDefault(i => i.Code == IndicatorCode);
            int referenceCount = 0;
            if (indicator != null)
            {
                referenceCount = _context.ReferencePoint.Count(r => r.IndicatorId == indicator.Id);
            }
            JsonResult result = new JsonResult(referenceCount);
            return result;
        }

        public bool CheckFormula(string Formula, string IndicatorType)
        {
            bool r = true;
            string formula_test = Formula;
            formula_test = formula_test.Replace("+", "");
            formula_test = formula_test.Replace("-", "");
            formula_test = formula_test.Replace("*", "");
            formula_test = formula_test.Replace("/", "");
            formula_test = formula_test.Replace("(", "");
            formula_test = formula_test.Replace(")", "");
            formula_test = formula_test.Replace(",", ".");
            formula_test = formula_test.Replace(".", "");
            formula_test = formula_test.Replace("Area", "");
            formula_test = formula_test.Replace("Population", "");
            formula_test = formula_test.Replace("$", "");
            if (IndicatorType == Startup.Configuration["Integral"])
            {
                formula_test = formula_test.Replace("Min", "");
                formula_test = formula_test.Replace("Max", "");
            }
            foreach (var ind in _context.Indicator.AsNoTracking().ToList())
            {
                if(!string.IsNullOrEmpty(ind.Code))
                {
                    formula_test = formula_test.Replace(ind.Code, "");
                }
            }
            for (int n = 0; n <= 9; n++)
            {
                formula_test = formula_test.Replace(n.ToString(), "");
            }
            if (formula_test.Trim() != "")
            {
                r = false;
            }
            return r;
        }

        public List<IndicatorValue> CalculateIndicatorValues(string Formula, int IndicatorId)
        {
            List<IndicatorValue> indicatorValues = new List<IndicatorValue>();
            string formula_test = Formula;
            List<Region> regions = _context.Region.ToList();
            foreach (var region in regions)
            {
                IndicatorValue indicatorValue = _context.IndicatorValue.AsNoTracking().FirstOrDefault(iv => iv.RegionId == region.Id && iv.IndicatorId == IndicatorId);
                if (indicatorValue == null)
                {
                    indicatorValue = new IndicatorValue()
                    {
                        Id = 0,
                        IndicatorId = IndicatorId
                    };
                }
                indicatorValue.RegionId = region.Id;
                indicatorValue.Region = region;
                indicatorValue.Year = region.Year;
                indicatorValues.Add(indicatorValue);
            }
            // => calculate values
            string code = $"decimal?[] r = new decimal?[{indicatorValues.Count().ToString()}];";
            for (int i = 0; i < indicatorValues.Count(); i++)
            {
                code += @"
                        ";
                string formula_code = formula_test;
                while (formula_code.IndexOf("Area") >= 0)
                {
                    formula_code = formula_code.Replace("Area", indicatorValues[i].Region.Area.ToString());
                }
                while (formula_code.IndexOf("Population") >= 0)
                {
                    formula_code = formula_code.Replace("Population", indicatorValues[i].Region.Area.ToString());
                }
                while (formula_code.IndexOf("$") >= 0)
                {
                    DollarRate dollarRate = _context.DollarRate.AsNoTracking().FirstOrDefault(d => d.Year == indicatorValues[i].Region.Year);
                    decimal? dollarRateValue = dollarRate == null ? null : dollarRate.Value;
                    formula_code = formula_code.Replace("$", dollarRateValue == null ? "null" : dollarRateValue.ToString());
                }
                while (formula_code.IndexOf("Min") >= 0)
                {
                    int M = formula_code.IndexOf("Min"),
                        I = formula_code.IndexOf(Startup.Configuration["FormulaIdFirstChar"], M),
                        d = formula_code.IndexOf(Startup.Configuration["FormulaIdLastChar"], I);
                    int indicatorId = Convert.ToInt32(formula_code.Substring(I + 1, d - I - 1));
                    ReferencePoint referencePoint = _context.ReferencePoint
                        .FirstOrDefault(r => r.IndicatorId == indicatorId);
                    decimal referencePointValue = referencePoint != null ? referencePoint.Min : 0;
                    formula_code = formula_code.Remove(M, d - M + 1 + 1);
                    formula_code = formula_code.Insert(M, referencePointValue.ToString());
                }
                while (formula_code.IndexOf("Max") >= 0)
                {
                    int M = formula_code.IndexOf("Max"),
                        I = formula_code.IndexOf(Startup.Configuration["FormulaIdFirstChar"], M),
                        d = formula_code.IndexOf(Startup.Configuration["FormulaIdLastChar"], I);
                    int indicatorId = Convert.ToInt32(formula_code.Substring(I + 1, d - I - 1));
                    ReferencePoint referencePoint = _context.ReferencePoint
                        .FirstOrDefault(r => r.IndicatorId == indicatorId);
                    decimal referencePointValue = referencePoint != null ? referencePoint.Max : 0;
                    formula_code = formula_code.Remove(M, d - M + 1 + 1);
                    formula_code = formula_code.Insert(M, referencePointValue.ToString());
                }
                while (formula_code.IndexOf(Startup.Configuration["FormulaIdFirstChar"]) >= 0)
                {
                    int I = formula_code.IndexOf(Startup.Configuration["FormulaIdFirstChar"]),
                        d = formula_code.IndexOf(Startup.Configuration["FormulaIdLastChar"]);
                    int indicatorId = Convert.ToInt32(formula_code.Substring(I + 1, d - I - 1));
                    IndicatorValue indicatorValue = _context.IndicatorValue
                        .AsNoTracking()
                        .FirstOrDefault(iv => iv.IndicatorId == indicatorId && iv.RegionId == indicatorValues[i].RegionId && iv.Year == indicatorValues[i].Year);
                    decimal? indicatorValueValue = indicatorValue != null ? indicatorValue.Value : null;
                    string indicatorValueValueString = indicatorValueValue != null ? indicatorValueValue.ToString() : "null";
                    formula_code = formula_code.Remove(I, d - I + 1);
                    formula_code = formula_code.Insert(I, indicatorValueValueString);
                }
                formula_code = formula_code.Replace(',', '.');
                for (int j = formula_code.Length - 1; j >= 0; j--)
                {
                    bool insert = false;
                    if (Char.IsDigit(formula_code[j]))
                    {
                        insert = true;
                    }
                    if (j != formula_code.Length - 1)
                    {
                        if (formula_code[j + 1] == '.' || Char.IsDigit(formula_code[j + 1]))
                        {
                            insert = false;
                        }
                    }
                    if (insert)
                    {
                        formula_code = formula_code.Insert(j + 1, "M");
                    }
                }
                if(formula_code.Contains("null"))
                {
                    formula_code = "null";
                }
                code += $"r[{i.ToString()}] = null;" + "try{" + $"r[{i.ToString()}] = (decimal?)({formula_code});" + "} catch { }";
            }
            string codeToCompile = @"using System;
                    namespace RoslynCompile
                    {
                        public class Calculator
                        {
                            public decimal?[] Calculate()
                            {
                                " + code + @"
                                return r;
                            }
                        }
                    }";
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = new MetadataReference[]
            {
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location)
            };
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);
                    indicatorValues = new List<IndicatorValue>();
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType("RoslynCompile.Calculator");
                    var instance = assembly.CreateInstance("RoslynCompile.Calculator");
                    var meth = type.GetMember("Calculate").First() as MethodInfo;
                    decimal?[] r = meth.Invoke(instance, null) as decimal?[];
                    for (int i = 0; i < indicatorValues.Count(); i++)
                    {
                        indicatorValues[i].Value = r[i];
                    }
                }
            }
            // <=
            return indicatorValues;
        }

        // расчет значений индикатора
        public void CalculateIndicatorValues(int IndicatorId)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>();
            string defaultConnection = Startup.Configuration.GetSection("ConnectionStrings")["DefaultConnection"];
            options.UseNpgsql(defaultConnection);
            using (var contextlocal = new ApplicationDbContext(options.Options))
            {
                Indicator indicator = contextlocal.Indicator.AsNoTracking().FirstOrDefault(i => i.Id == IndicatorId);
                var indicatorsall = contextlocal.Indicator
                    .AsNoTracking()
                    .Where(i => i.Id != IndicatorId)
                    .Include(i => i.Group)
                    .ToList();
                List<IndicatorValue> indicatorValues = new List<IndicatorValue>();
                string formula_test = indicator.Formula;
                foreach (var ind in indicatorsall)
                {
                    if (!string.IsNullOrEmpty(ind.Code))
                    {
                        formula_test = formula_test.Replace(ind.Code, $"{Startup.Configuration["FormulaIdFirstChar"]}{ind.Id.ToString()}{Startup.Configuration["FormulaIdLastChar"]}");
                    }
                }
                indicatorValues = CalculateIndicatorValues(indicator.Formula, IndicatorId);
                for (int i = 0; i < indicatorValues.Count(); i++)
                {
                    if (indicatorValues[i].Id == 0)
                    {
                        contextlocal.Add(indicatorValues[i]);
                    }
                    else
                    {
                        contextlocal.Update(indicatorValues[i]);
                    }
                }
                contextlocal.SaveChanges();
            }
        }

        // расчет значений зависимых индикаторов
        public void CalculateIndicatorsValues(int IndicatorId)
        {
            Indicator indicator = _context.Indicator.AsNoTracking().FirstOrDefault(i => i.Id == IndicatorId);
            ReferencePoint referencePoint = _context.ReferencePoint.AsNoTracking().FirstOrDefault(i => i.IndicatorId == IndicatorId);
            var indicatorsrelated = _context.Indicator
                .AsNoTracking()
                .Where(i => i.Formula.Contains($"{Startup.Configuration["FormulaIdFirstChar"]}{IndicatorId.ToString()}{Startup.Configuration["FormulaIdLastChar"]}"))
                .ToList();
            foreach (var indicatorrelated in indicatorsrelated)
            {
                CalculateIndicatorValues(indicatorrelated.Id);
            }
        }
    }
}
