using Student.Dtos;

namespace Student.Services;

public interface IStudentService
{
    Task<IList<StudentDto>> GetStudentsAsync();
    Task<StudentDto?> GetStudentByIdAsync(int id);
    Task<bool> CreateStudentAsync(StudentDto student);
    Task<bool> UpdateStudentAsync(StudentDto student);
    Task<bool> DeleteStudentAsync(int id);
}