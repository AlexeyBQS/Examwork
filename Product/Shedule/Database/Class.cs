using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Database
{
    public class Class
    {
        public int ClassId { get; set; } = default;
        public int? TeacherId { get; set; } = null!;
        public int? CabinetId { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public int? CountPupils { get; set; } = null!;
        public byte[]? Photo { get; set; } = null!;

        public Teacher? Teacher { get; set; } = null!;
        public Cabinet? Cabinet { get; set; } = null!;

        public ICollection<ClassLesson>? ClassLessons { get; set; } = default!;
    }
}
