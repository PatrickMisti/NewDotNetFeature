using System.Runtime.Serialization;
using Connectivity.Messages;

namespace Student.Dtos;

[DataContract]
public class StudentDto
{
    [DataMember(Name = "id")]
    public int Id { get; set; } = default;
    [DataMember(Name = "firstName")]
    public required string FirstName { get; set; }
    [DataMember(Name = "lastName")]
    public required string LastName { get; set; }
    [DataMember(Name = "birthday")]
    public DateTime Birthday { get; set; }
    [DataMember(Name = "classroom")]
    public int Classroom { get; set; }

    public StudentClassMessage MapToMessage()
    {
        return new StudentClassMessage
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            Birthday = Birthday,
            Classroom = Classroom,
        };
    }

    public static StudentDto ToStudentDto(StudentClassMessage message)
    {
        return new StudentDto
        {
            Id = message.Id,
            FirstName = message.FirstName,
            LastName = message.LastName,
            Birthday = message.Birthday,
            Classroom = message.Classroom,
        };
    }
}