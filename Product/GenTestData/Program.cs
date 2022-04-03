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
        + $"\nStackTrace: {ex.InnerException?.StackTrace}"
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

byte[] GetImageByteArray(string path)
{
    Image image = Image.FromFile(path);

    using (MemoryStream ms = new MemoryStream())
    {
        image.Save(ms, image.RawFormat);
        return ms.ToArray();
    }
}

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
            Photo = GetImageByteArray($"TestData\\Teacher\\Teacher{dataSplit[0]}.jpg"),
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
            Name = $"Кабинет {teacher.TeacherId}",
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

IEnumerable<ClassLesson> GenClassLessons(IEnumerable<Teacher> teachers, IEnumerable<Cabinet> cabinets, IEnumerable<Class> classes, IEnumerable<Lesson> lessons)
{
    return default!;
}

IEnumerable<ScheduleLesson> GenScheduleLessons()
{
    return default!;
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

LogAction("Creating class lessons", () => classLessons = GenClassLessons(teachers, cabinets, classes, lessons));
LogAction("Adding class lessons in database", () => AddRangeEntities(classLessons));
Console.WriteLine();

LogAction("Creating schedule lessons", () => scheduleLessons = GenScheduleLessons());
LogAction("Adding schedule lessons in database", () => AddRangeEntities(scheduleLessons));

Console.WriteLine("\nSuccess creating and adding data on database!");

ExitProgram();

#endregion