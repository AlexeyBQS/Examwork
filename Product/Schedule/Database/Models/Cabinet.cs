using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Database.Models
{
    public class Cabinet
    {
        public int CabinetId { get; set; } = default;
        public int? TeacherId { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public int? CountPlaces { get; set; } = default;
        public byte[]? Photo { get; set; } = null!;
        public string? Description { get; set; } = null!;

        public Teacher? Teacher { get; set; } = null!;

        public ICollection<Class>? Classes { get; set; } = default!;
        public ICollection<ClassLesson>? ClassesLessons { get; set; } = default!;
        public ICollection<ScheduleLesson>? ScheduleLessons { get; set; } = default!;

        public string ToShortString() => $"{Name}";
        public override string ToString() => $"{CabinetId}: {Name}";
    }
}
