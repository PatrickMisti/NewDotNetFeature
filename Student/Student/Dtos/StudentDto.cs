using Connectivity.Messages;

namespace Student.Dtos;

public class StudentDto
{
    public int Id { get; set; } = default;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthday { get; set; }
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