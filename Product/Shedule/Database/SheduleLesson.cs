using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.Database
{
    public class SheduleLesson
    {
        public DateTime Date { get; set; } = default;
        public int NumberLesson { get; set; } = default;
        public int? GroupLessonId { get; set; } = null!;
        public int? CabinetId { get; set; } = null!;

        public GroupLesson? GroupLesson { get; set; } = default!;
        public Cabinet? Cabinet { get; set; } = default!;
    }
}
