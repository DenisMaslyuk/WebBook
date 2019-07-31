using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBook.Models;

namespace WebBook.Controllers
{
    public class HistoriesController : Controller
    {
        private readonly BookLibraryContext _context;

        public HistoriesController(BookLibraryContext context)
        {
            _context = context;
        }

        // GET: Histories
        public async Task<IActionResult> Index(int? id)
        {
            var query = _context.History.Include(h => h.Book).Include(h => h.User);
            if (id.HasValue)
            {
                return View(await query.Where(b => b.Book.Id.Equals(id)).ToListAsync());
            }


            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> TakenBooks()
        {
            var bookLibraryContext = _context.History.Include(h => h.Book).Include(h => h.User);
            return View(await bookLibraryContext.ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> Return(int id)
        {
            var history = await _context.History
                .Include(h => h.Book)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (history.Book.Issued == true)
            {
                history.ReturnDate = System.DateTime.Now;
                history.Book.Issued = false;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("TakenBooks", "Histories");

        }

        

        // GET: Histories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.History
                .Include(h => h.Book)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        // POST: Histories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var history = await _context.History.FindAsync(id);
            _context.History.Remove(history);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoryExists(int id)
        {
            return _context.History.Any(e => e.Id == id);
        }
    }
}
