using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBook.Models;


namespace WebBook.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly BookLibraryContext _context;

        public ReportController(BookLibraryContext context)
        {
            _context = context;
        }
        // GET: Report
        public async Task<IActionResult> Index()
        {
            var bookLibraryContext = _context.History.Include(h => h.Book).Include(h => h.User).
                Where(h => h.Book.Issued == true && h.ReturnDate == null);
            
            ViewData["Message"] = bookLibraryContext.Count();
            return View(await bookLibraryContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> IndexPrint()
        {
            var bookLibraryContext = _context.History.Include(h => h.Book).Include(h => h.User).
                Where(h => h.Book.Issued == true && h.ReturnDate == null);

            ViewData["Message"] = bookLibraryContext.Count();
            return View(await bookLibraryContext.ToListAsync());
        }

        public async Task<IActionResult> Turnover()
        {
            var bookLibraryContext = _context.History.Include(h => h.Book).Include(h=>h.User)
                .GroupBy(h=>h.Book.Name).Select(g=>g.First());

            return View(await bookLibraryContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Turnover(DateTime FirstDate,DateTime SecondDate,int id)
        {
            var bookLibraryContext = _context.History.Include(h => h.Book).Include(h => h.User)
                .Where(d => d.Issue >= FirstDate && d.ReturnDate <= SecondDate && d.ReturnDate != null && d.Book.Id==id);
            return View(await bookLibraryContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> TurnoverPrint(DateTime FirstDate, DateTime SecondDate, int id)
        {
            ViewData["Data1"] = FirstDate.ToShortDateString();
            ViewData["Data2"] = SecondDate.ToShortDateString();
            var bookLibraryContext = _context.History.Include(h => h.Book).Include(h => h.User)
                .Where(d => d.Issue >= FirstDate && d.ReturnDate <= SecondDate && d.ReturnDate != null && d.Book.Id == id);
            return View(await bookLibraryContext.ToListAsync());
        }
        public async Task<IActionResult> PeriodHistory()
        {
            var bookLibraryContext = _context.History.Include(h => h.Book).Include(h => h.User);
            return View(await bookLibraryContext.ToListAsync());
        }
        
        [HttpPost]
        public async Task<IActionResult> PeriodHistory(DateTime FirstDate, DateTime SecondDate)
        {
            var bookLibraryContext = _context.History.Include(h => h.Book).Include(h => h.User)
                .Where(d => d.Issue >= FirstDate && d.ReturnDate <= SecondDate);
            return View(await bookLibraryContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> PeriodHistoryPrint(DateTime FirstDate, DateTime SecondDate)
        {
            ViewData["Data1"] = FirstDate.ToShortDateString();
            ViewData["Data2"] = SecondDate.ToShortDateString();
            var bookLibraryContext = _context.History.Include(h => h.Book).Include(h => h.User)
                .Where(d => d.Issue >= FirstDate && d.ReturnDate <= SecondDate);
            return View(await bookLibraryContext.ToListAsync());
        }
    }
}
