using Microsoft.EntityFrameworkCore;
using MindPalace.Models;

namespace MindPalace.Services.Interfaces
{
    public interface IAccessoryService
    {
        public Task AddAccessoriesToItemAsync(List<int> accessoryIds, int toDoItemId);

        public Task RemoveAccessoriesFromItemAsync(int toDoItemId);
    }
}
