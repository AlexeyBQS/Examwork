using Microsoft.EntityFrameworkCore;
using Schedule.Database.Models;
using Schedule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; } = default!;
        public DbSet<Cabinet> Cabinets { get; set; } = default!;
        public DbSet<Class> Classes { get; set; } = default!;
        public DbSet<Lesson> Lessons { get; set; } = default!;
        public DbSet<ClassLesson> ClassLessons { get; set; } = default!;
        public DbSet<ScheduleLesson> ScheduleLessons { get; set; } = default!;
        public DbSet<Password> Passwords { get; set; } = default!;

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConfigManager.ConnectionString);
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
                .Property(cabinet => cabinet.Photo)
                .HasColumnType("image");

            modelBuilder.Entity<Cabinet>()
                .HasOne(cabinet => cabinet.Teacher)
                .WithMany(teacher => teacher.Cabinets)
                .HasForeignKey(cabinet => cabinet.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Classes
            modelBuilder.Entity<Class>()
                .HasKey(_class => _class.ClassId);

            modelBuilder.Entity<Class>()
                .Property(_class => _class.Photo)
                .HasColumnType("image");

            modelBuilder.Entity<Class>()
                .HasOne(_class => _class.Teacher)
                .WithMany(teacher => teacher.Classes)
                .HasForeignKey(_class => _class.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Class>()
                .HasOne(_class => _class.Cabinet)
                .WithMany(cabinet => cabinet.Classes)
                .HasForeignKey(_class => _class.CabinetId)
                .OnDelete(DeleteBehavior.SetNull);

            // Lessons
            modelBuilder.Entity<Lesson>()
                .HasKey(lesson => lesson.LessonId);

            // ClassLessons
            modelBuilder.Entity<ClassLesson>()
                .HasKey(classLesson => classLesson.ClassLessonId);

            modelBuilder.Entity<ClassLesson>()
                .HasOne(classLesson => classLesson.Class)
                .WithMany(_class => _class.ClassLessons)
                .HasForeignKey(classLesson => classLesson.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassLesson>()
                .HasOne(classLesson => classLesson.Lesson)
                .WithMany(lesson => lesson.ClassLessons)
                .HasForeignKey(classLesson => classLesson.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassLesson>()
                .HasOne(classLesson => classLesson.Teacher)
                .WithMany(teacher => teacher.ClassLessons)
                .HasForeignKey(classLessons => classLessons.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ClassLesson>()
                .HasOne(classLesson => classLesson.PairClassLesson)
                .WithMany()
                .HasForeignKey(classLessons => classLessons.PairClassLessonId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ClassLesson>()
                .HasOne(classLesson => classLesson.DefaultCabinet)
                .WithMany(cabinet => cabinet.ClassesLessons)
                .HasForeignKey(classLesson => classLesson.DefaultCabinetId)
                .OnDelete(DeleteBehavior.SetNull);

            // ScheduleLessons
            modelBuilder.Entity<ScheduleLesson>()
                .HasKey(scheduleLesson => new { scheduleLesson.Date, scheduleLesson.NumberLesson, scheduleLesson.ClassLessonId });

            modelBuilder.Entity<ScheduleLesson>()
                .HasOne(scheduleLesson => scheduleLesson.ClassLesson)
                .WithMany(classLesson => classLesson.ScheduleLessons)
                .HasForeignKey(scheduleLesson => scheduleLesson.ClassLessonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ScheduleLesson>()
                .HasOne(scheduleLesson => scheduleLesson.Cabinet)
                .WithMany(cabinet => cabinet.ScheduleLessons)
                .HasForeignKey(scheduleLesson => scheduleLesson.CabinetId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
