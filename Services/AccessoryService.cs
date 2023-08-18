using Microsoft.EntityFrameworkCore;
using MindPalace.Data;
using MindPalace.Models;
using MindPalace.Services.Interfaces;

namespace MindPalace.Services
{
    public class AccessoryService : IAccessoryService
    {
        private readonly ApplicationDbContext _context;
        public AccessoryService (ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task AddAccessoriesToItemAsync(List<int> accessoryIds, int toDoItemId)
        {
            try
            {
                // get ToDoItem to add categories to
                ToDoItem? toDoItem = await _context.ToDoItems
                    .Include(c => c.Accessories)
                    .FirstOrDefaultAsync(c => c.Id == toDoItemId);

                // if this ToDoItem doesn't exist, stop
                if (toDoItem is null) return;

                foreach (int accessoryId in accessoryIds)
                {
                    // make sure accessory exists
                    Accessory? category = await _context.Accessories
                        .FirstOrDefaultAsync(c => c.Id == accessoryId);

                    // if it does, add the ToDoItem to that accessory
                    if (category is not null) toDoItem.Accessories.Add(category);
                }

                // save changes to database
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveAccessoriesFromItemAsync(int toDoItemId)
        {
            try
            {
                // get ToDoItem to remove categories from
                ToDoItem? toDoItem = await _context.ToDoItems
                    .Include(c => c.Accessories)
                    .FirstOrDefaultAsync(c => c.Id == toDoItemId);

                if (toDoItem is not null)
                {
                    // remove all of their categories
                    toDoItem.Accessories.Clear();

                    // save those changes to the database
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
