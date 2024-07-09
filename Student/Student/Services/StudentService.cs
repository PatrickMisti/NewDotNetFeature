using System.Text.Json;
using Connectivity;
using MassTransit;
using Student.Dtos;

namespace Student.Services;

public class StudentService(ILogger<StudentService> logger, IPublishEndpoint bus) : IStudentService
{
    public async Task<IList<StudentDto>> GetStudentsAsync()
    {
        await bus.Publish(new CreateMessage
        {
            Message = "Hllp"
        });


        logger.LogDebug("Grab all Students");
        return new List<StudentDto>();
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        return null;
    }

    public async Task<bool> CreateStudentAsync(StudentDto student)
    {
        await bus.Publish(student.MapToMessage());
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