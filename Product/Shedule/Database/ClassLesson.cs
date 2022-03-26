using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Database
{
    public class ClassLesson
    {
        public int ClassLessonId { get; set; } = default;
        public int? ClassId { get; set; } = null!;
        public int? LessonId { get; set; } = null!;
        public int? TeacherId { get; set; } = null!;
        public int? FirstClassLessonId { get; set; } = null!;
        public int? SecondClassLessonId { get; set; } = null!;
        public int? DefaultCabinetId { get; set; } = null!;
        public int? CountLesson { get; set; } = null!;
        public int? Difficulty { get; set; } = null!;

        public Class? Class { get; set; } = null!;
        public Lesson? Lesson { get; set; } = null!;
        public Teacher? Teacher { get; set; } = null!;
        public ClassLesson? FirstClassLesson { get; set; } = null!;
        public ClassLesson? SecondClassLesson { get; set; } = null!;
        public Cabinet? DefaultCabinet { get; set; } = null!;

        public ICollection<SheduleLesson>? SheduleLessons { get; set; } = default!;
    }
}
