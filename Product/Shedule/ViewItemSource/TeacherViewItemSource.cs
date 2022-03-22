using Shedule.Database;
using Shedule.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Shedule.ViewItemSource
{
    public class TeacherViewItemSource
    {
        public Teacher Teacher { get; set; } = null!;

        public TeacherViewItemSource(Teacher teacher)
        {
            Teacher = teacher;
        }

        public int TeacherId => Teacher.TeacherId;
        public string? Surname => Teacher.Surname;
        public string? Name => Teacher.Name;
        public string? Patronymic => Teacher.Patronymic;
        public DateTime? Birthday => Teacher.Birthday ?? null!;
        public string? Passport => Teacher.Passport;
        public byte[]? Photo => Teacher.Photo;
        public string? Education => Teacher.Education;

        public string? Birthday_ToString => Teacher.Birthday?.ToShortDateString() ?? null!;
        public BitmapImage Photo_Image =>
            Photo != null || Photo?.Length > 0 
            ? MyService.ConvertByteArrayToImage(Teacher.Photo ?? null!)
            : MyService.DefaultPhoto;
    }
}
