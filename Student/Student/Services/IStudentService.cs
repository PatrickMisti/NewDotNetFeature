namespace Student.Services;

public interface IStudentService
{
    Task<IList<Models.Student>> GetStudentsAsync();
    Task<Models.Student?> GetStudentByIdAsync(int id);
    Task<bool> CreateStudentAsync(Models.Student student);
    Task<bool> UpdateStudentAsync(Models.Student  student);
    Task<bool> DeleteStudentAsync(int id);
}