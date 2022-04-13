using Schedule.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.ViewItemSources
{
    public class ScheduleLessonViewItemSource
    {
        public ScheduleLessonViewItemSource()
        {

        }
        public ScheduleLessonViewItemSource(ScheduleLesson scheduleLesson)
        {
            ScheduleLesson = scheduleLesson;
        }

        // ScheduleLesson
        public ScheduleLesson? ScheduleLesson { get; set; } = null!;

        public DateTime? Date => ScheduleLesson?.Date;
        public int? NumberLesson => ScheduleLesson?.NumberLesson;
        public int? ClassLessonId => ScheduleLesson?.ClassLessonId;
        public int? CabinetId => ScheduleLesson?.CabinetId;
        public int? PairCabinetId => ScheduleLesson?.PairCabinetId;

        public string ScheduleLesson_Name => ClassLesson?.PairClassLesson != null
            ? $"{ClassLesson?.Lesson?.Name}\n{ClassLesson?.PairClassLesson?.Lesson?.Name}"
            : $"{ClassLesson?.Lesson?.Name}";
        public string ScheduleLesson_Cabinet => PairCabinet != null
            ? $"{(Cabinet != null ? $"{Cabinet!.Name}" : "")}\n{(PairCabinet != null ? $"{PairCabinet!.Name}" : "")}"
            : $"{(Cabinet != null ? $"{Cabinet!.Name}" : "")}";

        // ScheduleLesson - ClassLesson
        public ClassLesson? ClassLesson => ScheduleLesson?.ClassLesson;

        public int? ClassLesson_ClassId => ClassLesson?.ClassId;
        public int? ClassLesson_TeacherId => ClassLesson?.TeacherId;
        public int? ClassLesson_PairClassLessonId => ClassLesson?.PairClassLessonId;
        public int? ClassLesson_DefaultCabinetId => ClassLesson?.DefaultCabinetId;
        public int? ClassLesson_CountLesson => ClassLesson?.CountLesson;
        public int? ClassLesson_Difficulty => ClassLesson?.Difficulty;

        // ScheduleLesson - Cabinet
        public Cabinet? Cabinet => ScheduleLesson?.Cabinet;

        public int? Cabinet_TeacherId => Cabinet?.TeacherId;
        public string? Cabinet_Name => Cabinet?.Name;
        public int? Cabinet_CountPlaces => Cabinet?.CountPlaces;
        public byte[]? Cabinet_Photo => Cabinet?.Photo;
        public string? Cabinet_Description => Cabinet?.Description;

        // ScheduleLesson - PairCabinet
        public Cabinet? PairCabinet => ScheduleLesson?.PairCabinet;

        public int? PairCabinet_TeacherId => PairCabinet?.TeacherId;
        public string? PairCabinet_Name => PairCabinet?.Name;
        public int? PairCabinet_CountPlaces => PairCabinet?.CountPlaces;
        public byte[]? PairCabinet_Photo => PairCabinet?.Photo;
        public string? PairCabinet_Description => PairCabinet?.Description;
    }
}
