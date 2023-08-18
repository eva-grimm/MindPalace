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
using MindPalace.Services.Interfaces;

namespace MindPalace.Controllers
{
    [Authorize]
    public class ToDoItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAccessoryService _accessoryService;

        public ToDoItemsController(ApplicationDbContext context,
            UserManager<AppUser> userManager, IAccessoryService accessoryService)
        {
            _context = context;
            _userManager = userManager;
            _accessoryService = accessoryService;
        }

        // GET: ToDoItems
        public async Task<IActionResult> Index(bool? showCompleted = null)
        {
            // get the user's ToDo items
            string userId = _userManager.GetUserId(User)!;

            List<ToDoItem> model = new();

            if (showCompleted is null) showCompleted = false;

            if (showCompleted.Value) model = 
                await _context.ToDoItems
                .Where(c => c.AppUserId == userId && c.Completed)
                .Include(c => c.Accessories)
                .ToListAsync();
            else model = await _context.ToDoItems
                .Where(c => c.AppUserId == userId && !c.Completed)
                .Include(c => c.Accessories)
                .ToListAsync();

            // Pass to view whether we're showing completed or uncompleted items
            ViewData["ShowCompleted"] = showCompleted.Value;

            return View(model);
        }

        // GET: ToDoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            string userId = _userManager.GetUserId(User)!;

            ToDoItem? toDoItem = await _context.ToDoItems
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == userId);

            if (toDoItem == null) return NotFound();

            return View(toDoItem);
        }

        // GET: ToDoItems/Create
        public async Task<IActionResult> Create()
        {
            string userId = _userManager.GetUserId(User)!;

            List<Accessory> accessories = await _context.Accessories
                .Where(c => c.AppUserId == userId).ToListAsync();

            // make Viewdata for todoitem's accessories
            ViewData["AccessoriesList"] = new MultiSelectList(accessories, "Id", "Name");

            return View();
        }

        // POST: ToDoItems/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppUserId,Name,Completed,CreatedDate,DueDate")] ToDoItem toDoItem, List<int> selected)
        {
            ModelState.Remove("AppUserId");
            if (ModelState.IsValid)
            {
                // Set User Id
                toDoItem.AppUserId = _userManager.GetUserId(User);

                // Set Created Date
                toDoItem.CreatedDate = DateTime.Now;

                _context.Add(toDoItem);
                await _context.SaveChangesAsync();
                await _accessoryService.AddAccessoriesToItemAsync(selected, toDoItem.Id);
                return RedirectToAction(nameof(Index));
            }
            return View(toDoItem);
        }

        // GET: ToDoItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            string userId = _userManager.GetUserId(User)!;

            // Get the first ToDoItem with Id matching id that belongs to the logged in user
            // else return default
            ToDoItem? toDoItem = await _context.ToDoItems
                .Include(c => c.Accessories)
                .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == userId);

            if (toDoItem == null) return NotFound();

            // make List of user's accessories for this todoitem
            List<Accessory> accessories = await _context.Accessories
                .Where(c => c.AppUserId == userId).ToListAsync();
            List<int> accessoryIds = toDoItem.Accessories.Select(c => c.Id).ToList();

            // make Viewdata for todoitem's accessories
            ViewData["AccessoriesList"] = new MultiSelectList(accessories, "Id", "Name");

            return View(toDoItem);
        }

        // POST: ToDoItems/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,Name,Completed,CreatedDate,DueDate")] ToDoItem toDoItem, List<int> selected)
        {
            //if (id != toDoItem.Id) return NotFound();

            ModelState.Remove("AppUserId");

            if (ModelState.IsValid)
            {
                try
                {
                    // Set User Id
                    toDoItem.AppUserId = _userManager.GetUserId(User);

                    // Set Created Date
                    toDoItem.CreatedDate = DateTime.Now;

                    _context.Update(toDoItem);
                    await _context.SaveChangesAsync();
                    await _accessoryService.RemoveAccessoriesFromItemAsync(toDoItem.Id);
                    await _accessoryService.AddAccessoriesToItemAsync(selected, toDoItem.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(toDoItem.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(toDoItem);
        }

        // GET: ToDoItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            ToDoItem? toDoItem = await _context.ToDoItems
                .Include(c => c.Accessories)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (toDoItem == null) return NotFound();

            return View(toDoItem);
        }

        // POST: ToDoItems/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ToDoItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ToDoItem'  is null.");
            }
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem != null)
            {
                _context.ToDoItems.Remove(toDoItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoItemExists(int id)
        {
          return (_context.ToDoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
