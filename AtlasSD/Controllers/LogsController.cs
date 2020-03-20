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

namespace AtlasSD.Controllers
{
    public class LogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Logs
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index(string SortOrder, string Email, string Operation, int? Page)
        {
            var logs = _context.Log
                .Where(b => true);

            ViewBag.CodeFilter = Email;
            ViewBag.NameENFilter = Operation;

            ViewBag.EmailSort = SortOrder == "Email" ? "EmailDesc" : "Email";
            ViewBag.OperationSort = SortOrder == "Operation" ? "OperationDesc" : "Operation";
            ViewBag.DateTimeSort = SortOrder == "DateTime" ? "DateTimeDesc" : "DateTime";

            if (!string.IsNullOrEmpty(Email))
            {
                logs = logs.Where(l => l.Email.ToLower().Contains(Email.ToLower()));
            }
            if (!string.IsNullOrEmpty(Operation))
            {
                logs = logs.Where(l => l.Operation.ToLower().Contains(Operation.ToLower()));
            }

            switch (SortOrder)
            {
                case "Email":
                    logs = logs.OrderBy(l => l.Email);
                    break;
                case "EmailDesc":
                    logs = logs.OrderByDescending(l => l.Email);
                    break;
                case "Operation":
                    logs = logs.OrderBy(l => l.Operation);
                    break;
                case "OperationDesc":
                    logs = logs.OrderByDescending(l => l.Operation);
                    break;
                case "DateTime":
                    logs = logs.OrderBy(l => l.DateTime);
                    break;
                case "DateTimeDesc":
                    logs = logs.OrderByDescending(l => l.DateTime);
                    break;
                default:
                    logs = logs.OrderBy(l => l.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(logs.Count(), Page);

            var viewModel = new LogIndexPageViewModel
            {
                Items = logs.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: Logs/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Log
                .SingleOrDefaultAsync(m => m.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }


        private bool LogExists(int id)
        {
            return _context.Log.Any(e => e.Id == id);
        }
    }
}
