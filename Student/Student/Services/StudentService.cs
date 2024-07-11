using Connectivity.Messages;
using Student.Bussystem;
using Student.Dtos;

namespace Student.Services;

public class StudentService(ILogger<StudentService> logger, BusController endpoint) : IStudentService
{
    public async Task<IList<StudentDto>> GetStudentsAsync()
    {
        logger.LogDebug("Grab all Students");
        return new List<StudentDto>();
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        return null;
    }

    public async Task<bool> CreateStudentAsync(StudentDto student)
    {
        var a = await endpoint.PubSubAsync<CreateStudentMessage, StudentCreated>(student.MapToMessage());
        return true;
    }

    public async Task<bool> UpdateStudentAsync(StudentDto student)
    {
        return true;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        return true;
    }
}