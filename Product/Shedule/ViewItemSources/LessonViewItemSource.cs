﻿using Shedule.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule.ViewItemSources
{
    public class LessonViewItemSource
    {
        public LessonViewItemSource(Lesson lesson)
        {
            Lesson = lesson;
        }

        // Lesson
        public Lesson Lesson { get; set; } = null!;

        public int LessonId => Lesson.LessonId;
        public string? Name => Lesson.Name;
        public string? Description => Lesson.Description;
    }
}
