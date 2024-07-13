using Connectivity.Configuration;
using Connectivity.Messages;
using Student.Dtos;

namespace Student.Services;

public class StudentService(ILogger<StudentService> logger, PubSub endpoint) : IStudentService
{
    public async Task<IList<StudentDto>> GetStudentsAsync()
    {
        logger.LogDebug("Send all student info to runner {0}", nameof(GetAllStudentMessage));
        var response = await endpoint.PubSubAsync<GetAllStudentMessage, AllStudentMessage>(GetAllStudentMessage.Instance);

        if (response.Message.Error != null)
            return new List<StudentDto>();

        var result = response.Message.Students.Select(StudentDto.ToStudentDto).ToList();
        logger.LogDebug("Get student successfully! Count: {0}", result.Count);
        return result;
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        logger.LogDebug("Send id: {0} to runner {1}",id,nameof(GetStudentByIdMessage));
        var response =
            await endpoint.PubSubAsync<GetStudentByIdMessage, StudentByIdMessage>(new GetStudentByIdMessage(id));

        if (response.Message.Error != null || response.Message.Student == null) return null;

        var result = StudentDto.ToStudentDto(response.Message.Student);
        logger.LogDebug("Get student by id {0} !", result.Id);
        return result;
    }

    public async Task<bool> CreateStudentAsync(StudentDto student)
    {
        logger.LogDebug("Send student to runner {0}", nameof(CreateStudentMessage));
        var response = await endpoint.PubSubAsync<CreateStudentMessage, StudentCreatedMessage>(new CreateStudentMessage(student.MapToMessage()));

        if (response.Message.Error != null)
            return false;

        logger.LogDebug("Student created successfully!");
        return true;
    }

    public async Task<bool> UpdateStudentAsync(StudentDto student)
    {
        logger.LogDebug("Update send student to runner {0}", nameof(GetUpdateStudentMessage));
        var response = await endpoint.PubSubAsync<GetUpdateStudentMessage, UpdateStudentMessage>(new GetUpdateStudentMessage(student.MapToMessage()));

        if (response.Message.Error != null)
            return false;

        logger.LogDebug("Updated student {0}!",response.Message.IsUpdated ? " successfully": "failed");
        return response.Message.IsUpdated;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        logger.LogDebug("Delete send student to runner {0}", nameof(GetDeleteStudentMessage));
        var response = await endpoint.PubSubAsync<GetDeleteStudentMessage, DeleteStudentMessage>(new GetDeleteStudentMessage(id));

        if (response.Message.Error != null)
            return false;

        logger.LogDebug("Deleted student {0}!", response.Message.IsDeleted ? " successfully" : "failed");
        return response.Message.IsDeleted;
    }
}