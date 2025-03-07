using OneOf;
using OneOf.Types;
using Student.Dtos;

namespace Student.Services;

public interface IStudentService
{
<<<<<<< HEAD
    Task<OneOf<IList<StudentDto>, Exception>> GetStudentsAsync();
    Task<OneOf<StudentDto, NotFound , Exception>> GetStudentByIdAsync(int id);
    Task<OneOf<True, False, Exception>> CreateStudentAsync(StudentDto student);
    Task<OneOf<True,False, Exception>> UpdateStudentAsync(StudentDto student);
    Task<OneOf<True, False, Exception>> DeleteStudentAsync(int id);
=======
    Task<IList<Connection.Models.Student>> GetStudentsAsync();
    Task<Connection.Models.Student?> GetStudentByIdAsync(int id);
    Task<bool> CreateStudentAsync(Connection.Models.Student student);
    Task<bool> UpdateStudentAsync(Connection.Models.Student  student);
    Task<bool> DeleteStudentAsync(int id);
>>>>>>> origin/other
}