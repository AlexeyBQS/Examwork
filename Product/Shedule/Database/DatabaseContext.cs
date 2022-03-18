using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; } = default!;
        public DbSet<Cabinet> Cabinets { get; set; } = default!;
        public DbSet<Class> Classes { get; set; } = default!;
        public DbSet<Group> Groups { get; set; } = default!;
        public DbSet<Lesson> Lessons { get; set; } = default!;
        public DbSet<GroupLesson> GroupLessons { get; set; } = default!;
        public DbSet<SheduleLesson> SheduleLessons { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
