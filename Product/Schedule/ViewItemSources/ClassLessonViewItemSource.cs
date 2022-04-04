using Schedule.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.ViewItemSources
{
    public class ClassLessonViewItemSource
    {
        public ClassLessonViewItemSource(Lesson lesson, ClassLesson classLesson = null!)
        {
            Lesson = lesson;
            ClassLesson = classLesson;
        }

        // Lesson
        public Lesson Lesson { get; set; } = null!;

        public int LessonId => Lesson.LessonId;
        public string? Name => Lesson.Name;
        public string? Description => Lesson.Description;

        // ClassLesson
        public ClassLesson? ClassLesson { get; set; } = null!;

        public bool ClassLesson_IsEnabled => ClassLesson != null;

        public int? ClassLessonId => ClassLesson?.ClassLessonId;
        public int? ClassId => ClassLesson?.ClassId;
        public int? TeacherId => ClassLesson?.TeacherId;
        public int? PairClassLessonId => ClassLesson?.PairClassLessonId;
        public int? DefaultCabinetId => ClassLesson?.DefaultCabinetId;
        public int? CountLesson => ClassLesson?.CountLesson;
        public int? Difficulty => ClassLesson?.Difficulty;
    }
}
