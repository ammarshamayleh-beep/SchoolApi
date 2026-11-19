using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Auth;
using SchoolApi.Models;

namespace SchoolApi.Data;

// Fills the database with starting data, only if it is empty
public static class DbSeeder
{
    public static async Task SeedAsync(SchoolContext context, IPasswordHasher<User> passwordHasher)
    {
        await SeedSchoolAsync(context);
        await SeedUsersAsync(context, passwordHasher);
    }

    // Classrooms, teachers, subjects, students and grades
    private static async Task SeedSchoolAsync(SchoolContext context)
    {
        // Already has data? Do nothing
        if (await context.Classrooms.AnyAsync())
        {
            return;
        }

        // 1. Classrooms
        var grade10A = new Classroom { Name = "Grade 10 - A", Year = 2026 };
        var grade10B = new Classroom { Name = "Grade 10 - B", Year = 2026 };
        var grade11A = new Classroom { Name = "Grade 11 - A", Year = 2026 };

        // 2. Teachers
        var khaled = new Teacher { FirstName = "Khaled", LastName = "Haddad", Email = "khaled@school.edu", HireDate = new DateTime(2018, 9, 1) };
        var rana = new Teacher { FirstName = "Rana", LastName = "Saleh", Email = "rana@school.edu", HireDate = new DateTime(2020, 9, 1) };
        var omar = new Teacher { FirstName = "Omar", LastName = "Nasser", Email = "omar@school.edu", HireDate = new DateTime(2021, 2, 1) };
        var lina = new Teacher { FirstName = "Lina", LastName = "Jaber", Email = "lina@school.edu", HireDate = new DateTime(2023, 9, 1) };

        // 3. Subjects (History has no teacher yet)
        var subjects = new List<Subject>
        {
            new Subject { Name = "Mathematics", Code = "MATH-10", Teacher = khaled },
            new Subject { Name = "Physics", Code = "PHYS-10", Teacher = khaled },
            new Subject { Name = "English", Code = "ENG-10", Teacher = rana },
            new Subject { Name = "Computer Science", Code = "CS-10", Teacher = omar },
            new Subject { Name = "Arabic", Code = "ARB-10", Teacher = lina },
            new Subject { Name = "History", Code = "HIST-10" }
        };

        // 4. Students
        var students = new List<Student>
        {
            new Student { FirstName = "Ahmad", LastName = "Saleh", Email = "ahmad@school.edu", DateOfBirth = new DateTime(2010, 3, 15), Classroom = grade10A },
            new Student { FirstName = "Sara", LastName = "Khalil", Email = "sara@school.edu", DateOfBirth = new DateTime(2010, 7, 2), Classroom = grade10A },
            new Student { FirstName = "Yousef", LastName = "Nasser", Email = "yousef@school.edu", DateOfBirth = new DateTime(2010, 1, 20), Classroom = grade10A },
            new Student { FirstName = "Noor", LastName = "Haddad", Email = "noor@school.edu", DateOfBirth = new DateTime(2010, 11, 5), Classroom = grade10A },
            new Student { FirstName = "Ali", LastName = "Mansour", Email = "ali@school.edu", DateOfBirth = new DateTime(2010, 5, 9), Classroom = grade10B },
            new Student { FirstName = "Dana", LastName = "Omari", Email = "dana@school.edu", DateOfBirth = new DateTime(2010, 8, 30), Classroom = grade10B },
            new Student { FirstName = "Hamza", LastName = "Qasem", Email = "hamza@school.edu", DateOfBirth = new DateTime(2010, 2, 14), Classroom = grade10B },
            new Student { FirstName = "Maya", LastName = "Zoubi", Email = "maya@school.edu", DateOfBirth = new DateTime(2010, 12, 1), Classroom = grade10B },
            new Student { FirstName = "Tariq", LastName = "Jaber", Email = "tariq@school.edu", DateOfBirth = new DateTime(2009, 4, 18), Classroom = grade11A },
            new Student { FirstName = "Reem", LastName = "Saleh", Email = "reem@school.edu", DateOfBirth = new DateTime(2009, 9, 22), Classroom = grade11A },
            new Student { FirstName = "Zaid", LastName = "Khalil", Email = "zaid@school.edu", DateOfBirth = new DateTime(2009, 6, 11), Classroom = grade11A },
            new Student { FirstName = "Leen", LastName = "Nasser", Email = "leen@school.edu", DateOfBirth = new DateTime(2009, 10, 3), Classroom = grade11A }
        };

        // 5. Grades: every student gets a grade in every subject, for term 1
        var random = new Random(1);
        var grades = new List<Grade>();

        foreach (var student in students)
        {
            foreach (var subject in subjects)
            {
                grades.Add(new Grade
                {
                    Student = student,
                    Subject = subject,
                    Term = "2026-T1",
                    Score = random.Next(50, 101)
                });
            }
        }

        // 6. Add everything and save once
        context.Classrooms.AddRange(grade10A, grade10B, grade11A);
        context.Teachers.AddRange(khaled, rana, omar, lina);
        context.Subjects.AddRange(subjects);
        context.Students.AddRange(students);
        context.Grades.AddRange(grades);

        await context.SaveChangesAsync();
    }

    // Login accounts, to test the roles. These passwords are for learning only.
    private static async Task SeedUsersAsync(SchoolContext context, IPasswordHasher<User> passwordHasher)
    {
        // Already has accounts? Do nothing
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var users = new List<User>
        {
            new User { Email = "admin@school.edu", Role = Roles.Admin }
        };

        // Every teacher and every student logs in with his school email
        foreach (var teacher in await context.Teachers.ToListAsync())
        {
            users.Add(new User { Email = teacher.Email, Role = Roles.Teacher, TeacherId = teacher.Id });
        }

        foreach (var student in await context.Students.ToListAsync())
        {
            users.Add(new User { Email = student.Email, Role = Roles.Student, StudentId = student.Id });
        }

        foreach (var user in users)
        {
            string password = user.Role switch
            {
                Roles.Admin => "Admin123!",
                Roles.Teacher => "Teacher123!",
                _ => "Student123!"
            };

            user.PasswordHash = passwordHasher.HashPassword(user, password);
        }

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}
