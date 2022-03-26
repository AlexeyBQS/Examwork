using Shedule.Database;
using Shedule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Shedule.ViewItemSources
{
    public class TeacherViewItemSource
    {
        public TeacherViewItemSource(Teacher teacher)
        {
            Teacher = teacher;
        }

        //Teacher
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
            ? Service.ConvertByteArrayToImage(Teacher.Photo ?? null!)
            : Service.DefaultPhoto;
    }
}
