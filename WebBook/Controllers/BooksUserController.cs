using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBook.Models;

namespace WebBook.Controllers
{
    public class BooksUserController : Controller
    {
        private readonly BookLibraryContext _context;

        public BooksUserController(BookLibraryContext context)
        {
            _context = context;
        }

        // GET: BooksUser
        public async Task<IActionResult> Index()
        {
            var query = _context.Books.Where(h=>h.OutDate==null);
            return View(await query.ToListAsync());
        }
        
        [HttpPost]
        public async Task<IActionResult> Take(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book.Issued == false)
            {
               // var currentUserEmailClaim = User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(User.Identity.Name));
                _context.History.Add(new History(id, user.Id));
                book.Issued = true;
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction("Index", "BooksUser");

        }



        // GET: BooksUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (books == null)
            {
                return NotFound();
            }

            return View(books);
        }
    }
}
