using Schedule.Database;
using Schedule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Schedule.ViewItemSources
{
    public class CabinetViewItemSource
    {
        public CabinetViewItemSource(Cabinet cabinet)
        {
            Cabinet = cabinet;
        }

        // Cabinet
        public Cabinet Cabinet { get; set; } = null!;

        public int CabinetId => Cabinet.CabinetId;
        public int? TeacherId => Cabinet.TeacherId;
        public string? Name => Cabinet.Name;
        public int? CountPlaces => Cabinet.CountPlaces;
        public byte[]? Photo => Cabinet.Photo;
        public string? Description => Cabinet.Description;

        public BitmapImage Photo_Image =>
            Photo != null || Photo?.Length > 0
            ? Service.ConvertByteArrayToBitmapImage(Photo ?? null!)
            : Service.DefaultPhoto;

        // Class - Teacher
        public Teacher? Teacher => Cabinet.Teacher;

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
            ? Service.ConvertByteArrayToBitmapImage(Teacher_Photo ?? null!)
            : Service.DefaultPhoto;
    }
}
