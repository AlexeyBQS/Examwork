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
        public int ClassLessonId { get; set; } = default;
        public int? CabinetId { get; set; } = null!;

        public ClassLesson ClassLesson { get; set; } = null!;
        public Cabinet? Cabinet { get; set; } = null!;
    }
}
