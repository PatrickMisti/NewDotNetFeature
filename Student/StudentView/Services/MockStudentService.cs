using StudentView.Models;

namespace StudentView.Services;

public class MockStudentService
{
    public static IList<Student> GetAllStudent()
    {
        return
        [
            new Student(1, "Herbert", "Schmid", DateTime.Now, 1),
            new Student(2, "Laurel", "Schmid", DateTime.Now, 2),
            new Student(3, "Simone", "Schmid", DateTime.Now, 3),
            new Student(4, "Luisa", "Schmid", DateTime.Now, 4),
        ];
    }

    public static Student? GetStudentById(int id)
    {
        return GetAllStudent().FirstOrDefault(d => d.Id == id);
    }
}