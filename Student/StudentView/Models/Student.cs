using System.Runtime.Serialization;

namespace StudentView.Models;

[DataContract]
public class Student
{
    [DataMember(Name = "id")]
    public int Id { get; set; } = default;
    [DataMember(Name = "firstName")]
    public string FirstName { get; set; } = string.Empty;
    [DataMember(Name = "lastName")]
    public string LastName { get; set; } = string.Empty;
    [DataMember(Name = "birthday")]
    public DateTime? Birthday { get; set; } = DateTime.Today;
    [DataMember(Name = "classroom")]
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