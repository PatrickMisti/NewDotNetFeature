using OneOf;
using OneOf.Types;
using Student.Dtos;

namespace Student.Services;

public interface IStudentService
{
    Task<OneOf<IList<StudentDto>, Exception>> GetStudentsAsync();
    Task<OneOf<StudentDto, NotFound , Exception>> GetStudentByIdAsync(int id);
    Task<OneOf<True, False, Exception>> CreateStudentAsync(StudentDto student);
    Task<OneOf<True,False, Exception>> UpdateStudentAsync(StudentDto student);
    Task<OneOf<True, False, Exception>> DeleteStudentAsync(int id);
}