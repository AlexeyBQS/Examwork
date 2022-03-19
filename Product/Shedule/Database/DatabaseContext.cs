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

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=SheduleDatabase.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Teachers
            modelBuilder.Entity<Teacher>()
                .HasKey(teacher => teacher.TeacherId);

            modelBuilder.Entity<Teacher>()
                .Property(teacher => teacher.Photo)
                .HasColumnType("image");

            // Cabinets
            modelBuilder.Entity<Cabinet>()
                .HasKey(cabinet => cabinet.CabinetId);

            modelBuilder.Entity<Cabinet>()
                .HasOne<Teacher>(cabinet => cabinet.Teacher)
                .WithMany(teacher => teacher.Cabinets)
                .HasForeignKey(cabinet => cabinet.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Classes
            modelBuilder.Entity<Class>()
                .HasKey(_class => _class.ClassId);

            // Groups
            modelBuilder.Entity<Group>()
                .HasKey(group => group.GroupId);

            modelBuilder.Entity<Group>()
                .HasOne(group => group.Teacher)
                .WithMany(teacher => teacher.Groups)
                .HasForeignKey(group => group.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Group>()
                .HasOne(group => group.Class)
                .WithMany(_class => _class.Groups)
                .HasForeignKey(group => group.ClassId)
                .OnDelete(DeleteBehavior.SetNull);

            // Lessons
            modelBuilder.Entity<Lesson>()
                .HasKey(lesson => lesson.LessonId);

            modelBuilder.Entity<Lesson>()
                .HasOne(lesson => lesson.Class)
                .WithMany(_class => _class.Lessons)
                .HasForeignKey(lesson => lesson.ClassId)
                .OnDelete(DeleteBehavior.SetNull);

            // GroupLessons
            modelBuilder.Entity<GroupLesson>()
                .HasKey(groupLesson => groupLesson.GroupLessonId);

            modelBuilder.Entity<GroupLesson>()
                .HasOne<Group>(groupLesson => groupLesson.Group)
                .WithMany(group => group.GroupLessons)
                .HasForeignKey(groupLesson => groupLesson.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GroupLesson>()
                .HasOne<Lesson>(groupLesson => groupLesson.Lesson)
                .WithMany(lesson => lesson.GroupLessons)
                .HasForeignKey(groupLesson => groupLesson.LessonId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GroupLesson>()
                .HasOne<Teacher>(groupLesson => groupLesson.Teacher)
                .WithMany(teacher => teacher.GroupLessons)
                .HasForeignKey(groupLesson => groupLesson.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            // SheduleLessons
            modelBuilder.Entity<SheduleLesson>()
                .HasKey(SheduleLesson => new { SheduleLesson.Date, SheduleLesson.NumberLesson, SheduleLesson.GroupLessonId });

            modelBuilder.Entity<SheduleLesson>()
                .HasOne<GroupLesson>(sheduleLesson => sheduleLesson.GroupLesson)
                .WithMany(groupLesson => groupLesson.SheduleLessons)
                .HasForeignKey(sheduleLesson => sheduleLesson.GroupLessonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SheduleLesson>()
                .HasOne<Cabinet>(sheduleLesson => sheduleLesson.Cabinet)
                .WithMany(cabinet => cabinet.SheduleLessons)
                .HasForeignKey(sheduleLesson => sheduleLesson.CabinetId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
