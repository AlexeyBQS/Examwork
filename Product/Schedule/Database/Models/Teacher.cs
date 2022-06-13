using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Database.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; } = default;
        public string? Surname { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public string? Patronymic { get; set; } = null!;
        public DateTime? Birthday { get; set; } = default;
        public string? Passport { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public byte[]? Photo { get; set; } = null!;
        public string? Education { get; set; } = null!;

        public ICollection<Cabinet>? Cabinets { get; set; } = default!;
        public ICollection<Class>? Classes { get; set; } = default!;
        public ICollection<ClassLesson>? ClassLessons { get; set; } = default!;

        public string ToShortString() => $"{Surname} {Name![0]}. {Patronymic![0]}.";
        public override string ToString() => $"{TeacherId}: {Surname} {Name} {Patronymic}";
    }
}
