using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Database
{
    public class GroupLesson
    {
        public int GroupLessonId { get; set; } = default;
        public int? GroupId { get; set; } = null!;
        public int? LessonId { get; set; } = null!;
        public int? TeacherId { get; set; } = null!;
        public int CountLesson { get; set; } = default;

        public Group Group { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
        public Teacher Teacher { get; set; } = null!;

        public ICollection<SheduleLesson> SheduleLessons { get; set; } = default!;
    }
}
