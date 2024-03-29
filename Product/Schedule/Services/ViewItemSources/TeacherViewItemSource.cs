﻿using Schedule.Database.Models;
using Schedule.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Schedule.ViewItemSources
{
    public class TeacherViewItemSource
    {
        public TeacherViewItemSource(Teacher teacher)
        {
            Teacher = teacher;
        }

        // Teacher
        public Teacher Teacher { get; set; } = null!;

        public int TeacherId => Teacher.TeacherId;
        public string? Surname => Teacher.Surname;
        public string? Name => Teacher.Name;
        public string? Patronymic => Teacher.Patronymic;
        public DateTime? Birthday => Teacher.Birthday ?? null!;
        public string? Passport => Teacher.Passport;
        public string? PhoneNumber => Teacher.PhoneNumber;
        public byte[]? Photo => Teacher.Photo;
        public string? Education => Teacher.Education;

        public string ToShortString => Teacher.ToShortString();
        public string? Birthday_ToString => Teacher.Birthday?.ToShortDateString() ?? null!;

        public BitmapImage Photo_Image =>
            Photo != null || Photo?.Length > 0
            ? Service.ConvertByteArrayToBitmapImage(Photo ?? null!)
            : Service.DefaultPhoto;
    }
}
