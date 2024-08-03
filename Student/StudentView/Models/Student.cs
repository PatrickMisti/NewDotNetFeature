namespace StudentView.Models;

public class Student
{
    public int Id { get; set; } = default;
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime Birthday { get; set; }
    public int Classroom { get; set; }

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