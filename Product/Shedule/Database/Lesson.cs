using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Database
{
    public class Lesson
    {
        public int LessonId { get; set; } = default;
        public string? Name { get; set; } = null!;
        public string? Description { get; set; } = null!;

        public ICollection<ClassLesson>? ClassLessons { get; set; } = default!;
    }
}
