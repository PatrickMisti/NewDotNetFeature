using System.Runtime.Serialization;

namespace Connectivity.Messages;


public interface IStudentMessage
{
        
}
public class GetAllStudentMessage: IStudentMessage
{
}

[Serializable]
[DataContract]
public record CreateStudentMessage : IStudentMessage
{
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string LastName { get; set; }
    [DataMember]
    public DateTime Birthday { get; set; }
    [DataMember]
    public int Classroom { get; set; }
}

public record StudentCreated(int Id, string FirstName);