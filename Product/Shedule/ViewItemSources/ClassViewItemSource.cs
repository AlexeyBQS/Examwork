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
    public class ClassViewItemSource
    {
        public ClassViewItemSource(Class _class)
        {
            Class = _class;
        }

        // Class
        public Class Class { get; set; } = null!;

        public int ClassId => Class.ClassId;
        public int? TeacherId => Class.TeacherId;
        public int? CabinetId => Class.CabinetId;
        public string? Name => Class.Name;
        public int? CountPupils => Class.CountPupils;
        public byte[]? Photo => Class.Photo;

        public BitmapImage Photo_Image =>
            Photo != null || Photo?.Length > 0
            ? Service.ConvertByteArrayToImage(Photo ?? null!)
            : Service.DefaultPhoto;

        // Class - Teacher
        public Teacher? Teacher => Class.Teacher;

        public string? Teacher_Surname => Teacher?.Surname;
        public string? Teacher_Name => Teacher?.Name;
        public string? Teacher_Patronymic => Teacher?.Patronymic;
        public DateTime? Teacher_Birthday => Teacher?.Birthday;
        public string? Teacher_Passport => Teacher?.Passport;
        public string? Teacher_PhoneNumber => Teacher?.PhoneNumber;
        public byte[]? Teacher_Photo => Teacher?.Photo;
        public string? Teacher_Education => Teacher?.Education;

        public string? Teacher_ToShortString => Teacher?.ToShortString();
        public BitmapImage Teacher_Photo_Image =>
            Teacher_Photo != null || Teacher_Photo?.Length > 0
            ? Service.ConvertByteArrayToImage(Teacher_Photo ?? null!)
            : Service.DefaultPhoto;

        // Class - Cabinet
        public Cabinet? Cabinet => Class.Cabinet;

        public int? Cabinet_TeacherId => Cabinet?.TeacherId;
        public string? Cabinet_Name => Cabinet?.Name;
        public int? Cabinet_CountPlaces => Cabinet?.CountPlaces;
        public byte[]? Cabinet_Photo => Cabinet?.Photo;
        public string? Cabinet_Description => Cabinet?.Description;

        public string? Cabinet_ToShortString => Cabinet?.ToShortString();
        public BitmapImage Cabinet_Photo_Image =>
            Cabinet_Photo != null || Cabinet_Photo?.Length > 0
            ? Service.ConvertByteArrayToImage(Cabinet_Photo ?? null!)
            : Service.DefaultPhoto;

        // Class - Cabinet - Teacher
        public Teacher? Cabinet_Teacher => Cabinet?.Teacher;

        public string? Cabinet_Teacher_Surname => Cabinet_Teacher?.Surname;
        public string? Cabinet_Teacher_Name => Cabinet_Teacher?.Name;
        public string? Cabinet_Teacher_Patronymic => Cabinet_Teacher?.Patronymic;
        public DateTime? Cabinet_Teacher_Birthday => Cabinet_Teacher?.Birthday;
        public string? Cabinet_Teacher_Passport => Cabinet_Teacher?.Passport;
        public string? Cabinet_Teacher_PhoneNumber => Cabinet_Teacher?.PhoneNumber;
        public byte[]? Cabinet_Teacher_Photo => Cabinet_Teacher?.Photo;
        public string? Cabinet_Teacher_Education => Cabinet_Teacher?.Education;

        public string? Cabinet_Teacher_ToShortString => Cabinet_Teacher?.ToShortString();
        public BitmapImage Cabinet_Teacher_Photo_Image =>
            Cabinet_Teacher_Photo != null || Cabinet_Teacher_Photo?.Length > 0
            ? Service.ConvertByteArrayToImage(Cabinet_Teacher_Photo ?? null!)
            : Service.DefaultPhoto;
    }
}
