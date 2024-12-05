namespace Student.Services;

public interface IStudentService
{
    Task<IList<Connection.Models.Student>> GetStudentsAsync();
    Task<Connection.Models.Student?> GetStudentByIdAsync(int id);
    Task<bool> CreateStudentAsync(Connection.Models.Student student);
    Task<bool> UpdateStudentAsync(Connection.Models.Student  student);
    Task<bool> DeleteStudentAsync(int id);
}