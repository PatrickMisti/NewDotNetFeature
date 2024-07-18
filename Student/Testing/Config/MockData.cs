namespace Testing.Config;

public static class MockData
{
    public static IList<StudentRunner.Model.Student> GetListStudent =
    [
        new StudentRunner.Model.Student
        {
            Id = 1,
            FirstName = "Herbert",
            LastName = "Shiller",
            BirthDay = DateTime.UtcNow,
            Classroom = 2
        },
        new StudentRunner.Model.Student
        {
            Id = 2,
            FirstName = "Lara",
            LastName = "Summer",
            BirthDay = DateTime.UtcNow,
            Classroom = 3
        }
    ];
}