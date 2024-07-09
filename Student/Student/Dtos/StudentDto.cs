using Connectivity.Messages;

namespace Student.Dtos;

public class StudentDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthday { get; set; }
    public int Classroom { get; set; }

    public CreateStudentMessage MapToMessage()
    {
        return new CreateStudentMessage
        {
            FirstName = FirstName,
            LastName = LastName,
            Birthday = Birthday,
            Classroom = Classroom,
        };
    }
}