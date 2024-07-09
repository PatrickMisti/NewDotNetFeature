namespace Connectivity.Messages;


public interface IStudentMessage
{
        
}
public class GetAllStudentMessage: IStudentMessage
{
}

[Serializable]
public record CreateStudentMessage : IStudentMessage
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthday { get; set; }
    public int Classroom { get; set; }
}