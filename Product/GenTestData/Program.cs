using GenTestData.Service;
using Microsoft.EntityFrameworkCore;
using Schedule.Database;
using System.Drawing;

#region Methods

void ExitProgram()
{
    Console.WriteLine();

    int currentTime = 0, timeExit = 5;
    while (currentTime != timeExit)
    {
        Console.WriteLine($"Program will close in {timeExit - currentTime}");
        currentTime++;

        Thread.Sleep(1000);
    }

    Environment.Exit(0);
}

void WriteExcetionToFile(Exception ex)
{
    string timeError = DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss");

    string exception =
          $"\n"
        + $"\n[Exception]"
        + $"\nHelpLink: {ex.HelpLink}"
        + $"\nHResult: {ex.HResult}"
        + $"\nExist InnerException: {ex.InnerException != null}"
        + $"\nMessage: {ex.Message}"
        + $"\nSource: {ex.Source}"
        + $"\nStackTrace: \n{ex.StackTrace}"
        + $"\nExist TargetSite: {ex.TargetSite != null}";

    string innerException = ex.InnerException != null ?
          $"\n"
        + $"\n[Exception - InnerException]"
        + $"\nHelpLink: {ex.InnerException?.HelpLink}"
        + $"\nHResult: {ex.InnerException?.HResult}"
        + $"\nExist InnerException: {ex.InnerException?.InnerException != null}"
        + $"\nMessage: {ex.InnerException?.Message}"
        + $"\nSource: {ex.InnerException?.Source}"
        + $"\nStackTrace: \n{ex.InnerException?.StackTrace}"
        + $"\nExist TargetSite: {ex.InnerException?.TargetSite != null}"
        : null!;

    string targetSite = ex.TargetSite != null ?
          $"\n"
        + $"\n[Exception - TargetSite]"
        + $"\nAttributes: {ex.TargetSite?.Attributes.ToString()}"
        + $"\nCallingConvention: {ex.TargetSite?.CallingConvention.ToString()}"
        + $"\nContainsGenericParameters: {ex.TargetSite?.ContainsGenericParameters}"
        + $"\nIsAbstract: {ex.TargetSite?.IsAbstract}"
        + $"\nIsAssembly: {ex.TargetSite?.IsAssembly}"
        + $"\nIsConstructedGenericMethod: {ex.TargetSite?.IsConstructedGenericMethod}"
        + $"\nIsConstructor: {ex.TargetSite?.IsConstructor}"
        + $"\nIsFamily: {ex.TargetSite?.IsFamily}"
        + $"\nIsFamilyAndAssembly: {ex.TargetSite?.IsFamilyAndAssembly}"
        + $"\nIsFamilyOrAssembly: {ex.TargetSite?.IsFamilyOrAssembly}"
        + $"\nIsFinal: {ex.TargetSite?.IsFinal}"
        + $"\nIsGenericMethod: {ex.TargetSite?.IsGenericMethod}"
        + $"\nIsGenericMethodDefinition: {ex.TargetSite?.IsGenericMethodDefinition}"
        + $"\nIsHideBySig: {ex.TargetSite?.IsHideBySig}"
        + $"\nIsPrivate: {ex.TargetSite?.IsPrivate}"
        + $"\nIsPublic: {ex.TargetSite?.IsPublic}"
        + $"\nIsSecurityCritical: {ex.TargetSite?.IsSecurityCritical}"
        + $"\nIsConstructor: {ex.TargetSite?.IsConstructor}"
        + $"\nIsSecuritySafeCritical: {ex.TargetSite?.IsSecuritySafeCritical}"
        + $"\nIsSecurityTransparent: {ex.TargetSite?.IsSecurityTransparent}"
        + $"\nIsSpecialName: {ex.TargetSite?.IsSpecialName}"
        + $"\nIsStatic: {ex.TargetSite?.IsStatic}"
        + $"\nMethodHandle: {ex.TargetSite?.MethodHandle.ToString()}"
        + $"\nMethodImplementationFlags: {ex.TargetSite?.MethodImplementationFlags.ToString()}"
        : null!;

    string content =
          $"Time error: {timeError.Replace("_", " ")}"
        + $"{exception}"
        + $"{innerException}"
        + $"{targetSite}";

    string path = Directory.GetCurrentDirectory();
    string filename = $"Error_{timeError}.log";

    File.WriteAllText($"{path}\\{filename}", content);

    Console.WriteLine($"Log error saved to {path}\\{filename}");
}

void LogAction(string message, Action action)
{
    Console.Write($"{message}... ");

    try
    {
        action?.Invoke();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error!");
        WriteExcetionToFile(ex);
        ExitProgram();
    }

    Console.WriteLine("Completed!");
}

void AddRangeEntities<T>(IEnumerable<T> entities) where T : class
{
    if (entities == null) return;

    using DatabaseContext context = new();

    object[] dbSets = new object[]
    {
        context.Teachers,
        context.Cabinets,
        context.Classes,
        context.Lessons,
        context.ClassLessons,
        context.ScheduleLessons,
    };

    DbSet<T> dbSet = null!;

    for (int i = 0; i < dbSets.Length && dbSet == null; i++)
    {
        dbSet = (dbSets[i] as DbSet<T>)!;
    }

    if (dbSet != null)
    {
        dbSet.AddRange(entities);
        context.SaveChanges();
    }
}

#endregion

#region Generators

IEnumerable<Teacher> GenTeachers(string[] dataTeachers)
{
    Random rand = new();
    List<string> lastPassports = new();
    List<string> lastPhone = new();
    List<Teacher> teachers = new();

    foreach (string data in dataTeachers)
    {
        string[] dataSplit = data.Split(' ');
        string passport = string.Empty;
        string phone = string.Empty;


        do { passport = ((long)(new Random().NextDouble() * 10000000000)).ToString(); }
        while (passport.Length != 10 && !lastPassports.Contains(passport));

        do { phone = "+79" + ((long)(new Random().NextDouble() * 1000000000)).ToString(); }
        while (phone.Length != 12 && !lastPhone.Contains(phone));

        teachers.Add(new Teacher
        {
            Surname = dataSplit[1],
            Name = dataSplit[2],
            Patronymic = dataSplit[3],
            Birthday = new DateTime(rand.Next(1960, 1999), rand.Next(1, 13), rand.Next(1, 27)),
            Passport = passport,
            PhoneNumber = phone,
            Photo = Service.ConvertFileToImageByteArray($"TestData\\Teacher\\Teacher{dataSplit[0]}.jpg"),
            Education = "Образование"
        });
    }

    return teachers;
}

IEnumerable<Cabinet> GenCabinets(IEnumerable<Teacher> teachers)
{
    List<Cabinet> cabinets = new();
    Random rand = new();

    foreach (Teacher teacher in teachers)
    {
        cabinets.Add(new Cabinet
        {
            TeacherId = teacher.TeacherId,
            Name = $"к{teacher.TeacherId}",
            CountPlaces = rand.Next(25, 30),
            Photo = null!,
            Description = "Описание"
        });
    }

    return cabinets;
}

IEnumerable<Class> GenClasses(IEnumerable<Teacher> teachers)
{
    List<Class> classes = new();
    Random random = new();

    int id = 1;
    for (int i = 1; i <= 11; ++i)
    {
        for (int j = 1; j <= 3 && (((i - 1) * 3) + j <= teachers.Count()); ++j)
        {
            classes.Add(new Class
            {
                TeacherId = id,
                CabinetId = id,
                Name = $"{i}{new string[] { "А", "Б", "В" }[j - 1]}",
                CountPupils = random.Next(25, 30),
                MaxDifficulty = random.Next(40, 50),
                Photo = null!
            });

            ++id;
        }
    }

    return classes;
}

IEnumerable<Lesson> GenLessons(string[] dataLessons)
{
    List<Lesson> lessons = new();

    foreach (string lesson in dataLessons)
    {
        lessons.Add(new Lesson
        {
            Name = lesson,
            Description = "Описание"
        });
    }

    return lessons;
}

IEnumerable<ClassLesson> GenClassLessons(IEnumerable<Class> classes, IEnumerable<Lesson> lessons)
{
    List<ClassLesson> classLessons = new();

    Random rand = new();

    foreach (Class _class in classes)
    {
        foreach (Lesson lesson in lessons)
        {
            bool add = false;
            ClassLesson classLesson = new();

            classLesson.ClassId = _class.ClassId;
            classLesson.LessonId = null!;
            classLesson.TeacherId = _class.TeacherId;
            classLesson.PairClassLessonId = null!;
            classLesson.DefaultCabinetId = null!;
            classLesson.CountLesson = rand.Next(30, 60);
            classLesson.Difficulty = rand.Next(5, 10);

            if (_class.Name == "1А" || _class.Name == "1Б" || _class.Name == "1В" ||
                _class.Name == "2А" || _class.Name == "2Б" || _class.Name == "2В" ||
                _class.Name == "3А" || _class.Name == "3Б" || _class.Name == "3В" ||
                _class.Name == "4А" || _class.Name == "4Б" || _class.Name == "4В")
            {
                if (lesson.Name == "Окружающий мир" || lesson.Name == "Математика" || lesson.Name == "Музыка" ||
                    lesson.Name == "Русский язык"   || lesson.Name == "Литература" || lesson.Name == "ФЗК")
                {
                    add = true;
                }
            }
            else if (_class.Name == "5А" || _class.Name == "5Б" || _class.Name == "5В" ||
                     _class.Name == "6А" || _class.Name == "6Б" || _class.Name == "6В" ||
                     _class.Name == "7А" || _class.Name == "7Б" || _class.Name == "7В" ||
                     _class.Name == "8А" || _class.Name == "8Б" || _class.Name == "8В" ||
                     _class.Name == "9А" || _class.Name == "9Б" || _class.Name == "9В")
            {
                if (lesson.Name == "История" || lesson.Name == "Литература" || lesson.Name == "Английский язык" ||
                    lesson.Name == "Алгебра" || lesson.Name == "География"  || lesson.Name == "Обществознание"  ||
                    lesson.Name == "Музыка"  || lesson.Name == "Геометрия"  || lesson.Name == "Немецкий язык"   ||
                    lesson.Name == "Физика"  || lesson.Name == "Риторика"   || lesson.Name == "Русский язык"    ||
                    lesson.Name == "Химия"   || lesson.Name == "Экология"   || lesson.Name == "Информатика"     ||
                    lesson.Name == "ОБЖ"     || lesson.Name == "Биология"   || lesson.Name == "Технология"      ||
                    lesson.Name == "ФЗК")       
                {
                    add = true;
                }
            }
            else if (_class.Name == "10А" || _class.Name == "10Б" || _class.Name == "10В" ||
                     _class.Name == "11А" || _class.Name == "11Б" || _class.Name == "11В")
            {
                if (lesson.Name == "Алгебра" || lesson.Name == "Астрономия" || lesson.Name == "Английский язык" ||
                    lesson.Name == "История" || lesson.Name == "Литература" || lesson.Name == "Обществознание"  ||
                    lesson.Name == "Физика"  || lesson.Name == "География"  || lesson.Name == "Немецкий язык"   ||
                    lesson.Name == "Химия"   || lesson.Name == "Геометрия"  || lesson.Name == "Русский язык"    ||
                    lesson.Name == "ОБЖ"     || lesson.Name == "Биология"   || lesson.Name == "Информатика"     ||
                    lesson.Name == "ФЗК"     || lesson.Name == "Черчение"   || lesson.Name == "Технология")
                {
                    add = true;
                }
            }

            if (add)
            {
                classLesson.LessonId = lesson.LessonId;

                if (lesson.Name != "Английский язык" || lesson.Name != "Немецкий язык")
                {
                    classLesson.TeacherId = _class.TeacherId;
                    classLesson.DefaultCabinetId = _class.CabinetId;
                }

                classLessons.Add(classLesson);
            }
        }
    }

    return classLessons;
}

IEnumerable<ClassLesson> GenPairClassLesson()
{
    using DatabaseContext context = new();

    int? englishLessonId = context.Lessons.FirstOrDefault(lesson => lesson.Name == "Английский язык")?.LessonId ?? null!;
    int? germanLessonId = context.Lessons.FirstOrDefault(lesson => lesson.Name == "Немецкий язык")?.LessonId ?? null!;

    int countClass = 33;

    for (int classId = 1; classId <= countClass; ++classId)
    {
        ClassLesson? english = context.ClassLessons
            .Where(classLessons => classLessons.ClassId == classId)
            .FirstOrDefault(classLesson => classLesson.LessonId == englishLessonId);

        ClassLesson? german = context.ClassLessons
            .Where(classLessons => classLessons.ClassId == classId)
            .FirstOrDefault(classLesson => classLesson.LessonId == germanLessonId);

        if (english != null && german != null)
        {
            english.PairClassLessonId = german.ClassLessonId;
            german.PairClassLessonId = english.ClassLessonId;
        }
    }

    context.SaveChanges();

    return context.ClassLessons.ToList();
}

IEnumerable<ScheduleLesson> GenScheduleLessons()
{
    using DatabaseContext context = new();

    IQueryable<Class> classes = context.Classes;
    IQueryable<Lesson> lessons = context.Lessons;
    List<ScheduleLesson> scheduleLessons = new();

    DateOnly dateCurrent = new(2021, 1, 2);
    DateOnly dateEnd = new(2022, 5, 31);

    while (dateCurrent <= dateEnd)
    {
        if (dateCurrent.DayOfWeek != DayOfWeek.Saturday && dateCurrent.DayOfWeek != DayOfWeek.Sunday)
        {
            foreach (Class _class in classes)
            {
                IQueryable<ClassLesson> classLessons = context.ClassLessons
                    .Where(classLesson => classLesson.ClassId == _class.ClassId)
                    .Include(classLesson => classLesson.PairClassLesson);

                int numberLesson = 1;
                foreach (ClassLesson classLesson in classLessons)
                {
                    bool add = false;
                    string lessonName = lessons
                        .First(lesson => lesson.LessonId == classLesson.LessonId).Name!;

                    if (numberLesson <= 6)
                    {
                        if (_class.Name == "1А" || _class.Name == "1Б" || _class.Name == "1В" ||
                            _class.Name == "2А" || _class.Name == "2Б" || _class.Name == "2В" ||
                            _class.Name == "3А" || _class.Name == "3Б" || _class.Name == "3В" ||
                            _class.Name == "4А" || _class.Name == "4Б" || _class.Name == "4В")
                        {
                            if (lessonName == "Литература" || lessonName == "Окружающий мир" ||
                                lessonName == "Математика" || lessonName == "Русский язык")
                            {
                                add = true;
                            }
                        }
                        else if (_class.Name == "5А" || _class.Name == "5Б" || _class.Name == "5В" ||
                                 _class.Name == "6А" || _class.Name == "6Б" || _class.Name == "6В" ||
                                 _class.Name == "7А" || _class.Name == "7Б" || _class.Name == "7В" ||
                                 _class.Name == "8А" || _class.Name == "8Б" || _class.Name == "8В" ||
                                 _class.Name == "9А" || _class.Name == "9Б" || _class.Name == "9В")
                        {
                            if (lessonName == "Информатика" || lessonName == "Английский язык" ||
                                lessonName == "Литература"  || lessonName == "Немецкий язык"   ||
                                lessonName == "Алгебра"     || lessonName == "Русский язык")
                            {
                                add = true;
                            }
                        }
                        else if (_class.Name == "10А" || _class.Name == "10Б" || _class.Name == "10В" ||
                                 _class.Name == "11А" || _class.Name == "11Б" || _class.Name == "11В")
                        {
                            if (lessonName == "Информатика" || lessonName == "Английский язык" ||
                                lessonName == "Литература"  || lessonName == "Немецкий язык" ||
                                lessonName == "Алгебра"     || lessonName == "Русский язык")
                            {
                                add = true;
                            }
                        }
                    }

                    if (add)
                    {
                        scheduleLessons.Add(new ScheduleLesson
                        {
                            Date = new(dateCurrent.Year, dateCurrent.Month, dateCurrent.Day),
                            NumberLesson = numberLesson,
                            ClassLessonId = classLesson.ClassLessonId,
                            CabinetId = classLesson.DefaultCabinetId,
                            PairCabinetId = classLesson.PairClassLesson?.DefaultCabinetId
                        });

                        ++numberLesson;
                    }
                }
            }
        }

        dateCurrent = dateCurrent.AddDays(1);
    }

    return scheduleLessons;
}

#endregion

#region Main

string[] dataTeachers = null!;
string[] dataLessons = null!;

IEnumerable<Teacher> teachers = default!;
IEnumerable<Cabinet> cabinets = default!;
IEnumerable<Class> classes = default!;
IEnumerable<Lesson> lessons = default!;
IEnumerable<ClassLesson> classLessons = default!;
IEnumerable<ScheduleLesson> scheduleLessons = default!;

LogAction("Getting data to create teachers"
    , () => dataTeachers = File.ReadAllLines($"{Directory.GetCurrentDirectory()}\\TestData\\Teacher\\Data.txt"));

LogAction("Getting data to create lessons"
    , () => dataLessons = File.ReadAllLines($"{Directory.GetCurrentDirectory()}\\TestData\\Lesson\\Data.txt"));
Console.WriteLine();

LogAction("Creating teachers", () => teachers = GenTeachers(dataTeachers));
LogAction("Adding teachers in database", () => AddRangeEntities(teachers));
Console.WriteLine();

LogAction("Creating cabinets", () => cabinets = GenCabinets(teachers));
LogAction("Adding cabinets in database", () => AddRangeEntities(cabinets));
Console.WriteLine();

LogAction("Creating classes", () => classes = GenClasses(teachers));
LogAction("Adding classes in database", () => AddRangeEntities(classes));
Console.WriteLine();

LogAction("Creating lessons", () => lessons = GenLessons(dataLessons));
LogAction("Adding lessons in database", () => AddRangeEntities(lessons));
Console.WriteLine();

LogAction("Creating class lessons", () => classLessons = GenClassLessons(classes, lessons));
LogAction("Adding class lessons in database", () => AddRangeEntities(classLessons));
LogAction("Creating reference to pair lessons", () => classLessons = GenPairClassLesson());
Console.WriteLine();

LogAction("Creating schedule lessons", () => scheduleLessons = GenScheduleLessons());
LogAction("Adding schedule lessons in database", () => AddRangeEntities(scheduleLessons));

Console.WriteLine("\nSuccess creating and adding data on database!");

ExitProgram();

#endregion