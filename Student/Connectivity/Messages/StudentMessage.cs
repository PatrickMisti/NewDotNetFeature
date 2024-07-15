namespace Connectivity.Messages;


public record StudentClassMessage
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime Birthday { get; set; }
    public int Classroom { get; set; }
}


public record GetAllStudentMessage
{
    public static GetAllStudentMessage Instance = new ();
}

public record AllStudentMessage
{
    public List<StudentClassMessage> Students { get; set; } = [];
    public Exception? Error { get; set; } = null;
}


public record CreateStudentMessage(StudentClassMessage Student);
public record StudentCreatedMessage
{
    public int Id { get; set; }
    public Exception? Error { get; set; }
}


public record GetStudentByIdMessage(int Id);

public record StudentByIdMessage
{
    public StudentClassMessage? Student { get; set; }
    public Exception? Error { get; set; }
}


public record GetUpdateStudentMessage(StudentClassMessage Student);

public record UpdateStudentMessage
{
    public bool IsUpdated { get; set; }
    public Exception? Error { get; set; }
}


public record GetDeleteStudentMessage(int Id);

public record DeleteStudentMessage
{
    public bool IsDeleted { get; set; }
    public Exception? Error { get; set; }
}