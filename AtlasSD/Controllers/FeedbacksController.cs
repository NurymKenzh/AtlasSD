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
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbacksController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Feedbacks
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index(string SortOrder, string Email, int? Page)
        {
            var feedbacks = _context.Feedback
                .Where(f => true);

            ViewBag.EmailFilter = Email;

            ViewBag.EmailSort = SortOrder == "Email" ? "EmailDesc" : "Email";
            ViewBag.DateTimeSort = SortOrder == "DateTime" ? "DateTimeDesc" : "DateTime";

            if (!string.IsNullOrEmpty(Email))
            {
                feedbacks = feedbacks.Where(f => f.Email.ToLower().Contains(Email.ToLower()));
            }

            switch (SortOrder)
            {
                case "Email":
                    feedbacks = feedbacks.OrderBy(f => f.Email);
                    break;
                case "EmailDesc":
                    feedbacks = feedbacks.OrderByDescending(f => f.Email);
                    break;
                case "DateTime":
                    feedbacks = feedbacks.OrderBy(f => f.DateTime);
                    break;
                case "DateTimeDesc":
                    feedbacks = feedbacks.OrderByDescending(f => f.DateTime);
                    break;
                default:
                    feedbacks = feedbacks.OrderBy(f => f.Id);
                    break;
            }

            var pager = new Pager(feedbacks.Count(), Page);

            var viewModel = new FeedbackIndexPageViewModel
            {
                Items = feedbacks.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: Feedbacks/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback
                .SingleOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Message")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.DateTime = DateTime.Now;
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction("Contact", "Home", null);
            }
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback
                .SingleOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedback.SingleOrDefaultAsync(m => m.Id == id);
            _context.Feedback.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedback.Any(e => e.Id == id);
        }
    }
}
