using Connectivity.Configuration;
using Connectivity.Messages;
using Student.Dtos;
using ILogger = Serilog.ILogger;


namespace Student.Services;

public class StudentService(ILogger logger, PubSub endpoint) : IStudentService
{
    public async Task<IList<StudentDto>> GetStudentsAsync()
    {
        logger.Debug("Send all student info to runner {0}", nameof(GetAllStudentMessage));
        var response = await endpoint.PubSubAsync<GetAllStudentMessage, AllStudentMessage>(GetAllStudentMessage.Instance);

        if (response.Message.Error != null)
            return new List<StudentDto>();

        var result = response.Message.Students.Select(StudentDto.ToStudentDto).ToList();
        logger.Debug("Get student successfully! Count: {0}", result.Count);
        return result;
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        logger.Debug("Send id: {0} to runner {1}",id,nameof(GetStudentByIdMessage));
        var response =
            await endpoint.PubSubAsync<GetStudentByIdMessage, StudentByIdMessage>(new GetStudentByIdMessage(id));

        if (response.Message.Error != null || response.Message.Student == null) return null;

        var result = StudentDto.ToStudentDto(response.Message.Student);
        logger.Debug("Get student by id {0} !", result.Id);
        return result;
    }

    public async Task<bool> CreateStudentAsync(StudentDto student)
    {
        logger.Debug("Send student to runner {0}", nameof(CreateStudentMessage));
        var response = await endpoint.PubSubAsync<CreateStudentMessage, StudentCreatedMessage>(new CreateStudentMessage(student.MapToMessage()));

        if (response.Message.Error != null)
            return false;

        logger.Debug("Student created successfully!");
        return true;
    }

    public async Task<bool> UpdateStudentAsync(StudentDto student)
    {
        logger.Debug("Update send student to runner {0}", nameof(GetUpdateStudentMessage));
        var response = await endpoint.PubSubAsync<GetUpdateStudentMessage, UpdateStudentMessage>(new GetUpdateStudentMessage(student.MapToMessage()));

        if (response.Message.Error != null)
            return false;

        logger.Debug("Updated student {0}!",response.Message.IsUpdated ? " successfully": "failed");
        return response.Message.IsUpdated;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        logger.Debug("Delete send student to runner {0}", nameof(GetDeleteStudentMessage));
        var response = await endpoint.PubSubAsync<GetDeleteStudentMessage, DeleteStudentMessage>(new GetDeleteStudentMessage(id));

        if (response.Message.Error != null)
            return false;

        logger.Debug("Deleted student {0}!", response.Message.IsDeleted ? " successfully" : "failed");
        return response.Message.IsDeleted;
    }
}