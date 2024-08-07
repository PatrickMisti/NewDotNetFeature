namespace StudentView.Models;

public class Student
{
    public int Id { get; set; } = default;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; } = DateTime.Today;
    public int Classroom { get; set; } = 0;

    public Student() {}

    public Student(int id, string firstName, string lastName, DateTime birthday, int classroom)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Birthday = birthday;
        Classroom = classroom;
    }
}