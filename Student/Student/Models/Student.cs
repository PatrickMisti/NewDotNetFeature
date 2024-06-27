using System.Runtime.Serialization;

namespace Student.Models;

public class Student : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDay { get; set; }
    public int Classroom { get; set; }
}