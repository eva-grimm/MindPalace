using System.ComponentModel.DataAnnotations;

namespace MindPalace.Models
{
    public class Accessory
    {
        // Primary key
        public int Id { get; set; }

        // Foreign key
        public string? AppUserId { get; set; }

        [Required]
        public string? Name { get; set; }

        // Navigation Properties
        public virtual AppUser? AppUser { get; set; }
        public ICollection<ToDoItem> ToDoItems { get; set; } = new HashSet<ToDoItem>();
    }
}