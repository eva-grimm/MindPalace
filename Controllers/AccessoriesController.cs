using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MindPalace.Data;
using MindPalace.Models;

namespace MindPalace.Controllers
{
    [Authorize]
    public class AccessoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AccessoriesController(ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Accessories
        public async Task<IActionResult> Index()
        {
            // get the user's categories
            string userId = _userManager.GetUserId(User)!;
            IEnumerable<Accessory> accessories = await _context.Accessories
                .Where(c => c.AppUserId == userId)
                .ToListAsync();
            return View(accessories);
        }

        // GET: Accessories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            string userId = _userManager.GetUserId(User)!;

            Accessory? accessory = await _context.Accessories
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == userId);

            if (accessory == null) return NotFound();

            return View(accessory);
        }

        // GET: Accessories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accessories/Create
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Accessory accessory)
        {
            ModelState.Remove("AppUserId");

            if (ModelState.IsValid)
            {
                // Set AppUserId
                accessory.AppUserId = _userManager.GetUserId(User);

                _context.Add(accessory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accessory);
        }

        // GET: Accessories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            string userId = _userManager.GetUserId(User)!;

            // Get the first Accessory with Id matching id that belongs to the logged in user
            // else return default
            Accessory? accessory = await _context.Accessories
                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == userId);

            if (accessory == null)  return NotFound();

            return View(accessory);
        }

        // POST: Accessories/Edit/5
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppUserId,Name")] Accessory accessory)
        {
            if (id != accessory.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accessory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccessoryExists(accessory.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(accessory);
        }

        // GET: Accessories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Accessories == null)
            {
                return NotFound();
            }

            var accessory = await _context.Accessories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (accessory == null) return NotFound();

            return View(accessory);
        }

        // POST: Accessories/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Accessories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Accessory'  is null.");
            }
            var accessory = await _context.Accessories.FindAsync(id);
            if (accessory != null)
            {
                _context.Accessories.Remove(accessory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccessoryExists(int id)
        {
          return (_context.Accessories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
