using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MindPalace.Models;

namespace MindPalace.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<ToDoItem> ToDoItems { get; set; } = default!;
        public virtual DbSet<Accessory> Accessories { get; set; } = default!;
    }
}