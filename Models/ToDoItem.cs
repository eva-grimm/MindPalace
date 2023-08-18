using System.ComponentModel.DataAnnotations;

namespace MindPalace.Models
{
    public class ToDoItem
    {
        private DateTime _createdDate;
        private DateTime? _dueDate;

        // Primary key
        public int Id { get; set; }

        // Foreign key
        public string? AppUserId { get; set; }

        // Properties
        public string? Name { get; set; }
        public bool Completed { get; set; }

        // Dates
        [DataType(DataType.Date), Display(Name = "Date Created")]
        public DateTime CreatedDate
        {
            get => _createdDate.ToLocalTime();
            set => _createdDate = value.ToUniversalTime();
        }
        [DataType(DataType.Date), Display(Name = "Due Date")]
        public DateTime? DueDate
        {
            get => _dueDate?.ToLocalTime();
            set => _dueDate = value.HasValue ? value.Value.ToUniversalTime() : null;
        }

        // Navigation Properties
        public virtual AppUser? AppUser { get; set; }
        public ICollection<Accessory> Accessories { get; set; } = new HashSet<Accessory>();
    }
}
