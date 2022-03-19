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
        public int? ClassId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Difficulty { get; set; } = default;

        public Class? Class { get; set; } = null!;

        public ICollection<GroupLesson>? GroupLessons { get; set; } = default!;
    }
}
