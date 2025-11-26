using Microsoft.EntityFrameworkCore;
using WebProgOdev.Models;

namespace WebProgOdev.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Service> Services { get; set; } = default!;
        public DbSet<Trainer> Trainers { get; set; } = default!;
        public DbSet<Appointment> Appointments { get; set; } = default!;

    }
}