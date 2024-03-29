﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Database.Models
{
    public class ClassLesson
    {
        public int ClassLessonId { get; set; } = default;
        public int? ClassId { get; set; } = null!;
        public int? LessonId { get; set; } = null!;
        public int? TeacherId { get; set; } = null!;
        public int? PairClassLessonId { get; set; } = null!;
        public int? DefaultCabinetId { get; set; } = null!;
        public int? CountLesson { get; set; } = null!;
        public int? Difficulty { get; set; } = null!;

        public Class? Class { get; set; } = null!;
        public Lesson? Lesson { get; set; } = null!;
        public Teacher? Teacher { get; set; } = null!;
        public ClassLesson? PairClassLesson { get; set; } = null!;
        public Cabinet? DefaultCabinet { get; set; } = null!;

        public ICollection<ScheduleLesson>? ScheduleLessons { get; set; } = default!;

        public override string ToString() => $"{ClassLessonId}{(PairClassLessonId != null ? "п" : "")}-{LessonId}: {Lesson?.Name}{(PairClassLesson != null ? $" {PairClassLesson.Lesson?.Name}" : "")}";
        public string ToShortString() => $"{Lesson?.Name}{(PairClassLesson != null ? $" {PairClassLesson.Lesson?.Name}" : "")}";
    }
}
