<<<<<<< HEAD
﻿using Connectivity.Configuration;
using Connectivity.Messages;
using OneOf;
using OneOf.Types;
using Student.Dtos;
using ILogger = Serilog.ILogger;
=======
﻿using Connection.Services;
using Connectivity;
using MassTransit;
>>>>>>> origin/other


namespace Student.Services;

<<<<<<< HEAD
public class StudentService(ILogger logger, PubSub endpoint) : IStudentService
{
    public async Task<OneOf<IList<StudentDto>, Exception>> GetStudentsAsync()
    {
        logger.Debug("Send all student info to runner {0}", nameof(GetAllStudentMessage));
        var response = await endpoint.PubSubAsync<GetAllStudentMessage, AllStudentMessage>(GetAllStudentMessage.Instance);

        if (response.Message.Error != null)
            return response.Message.Error;

        var result = response.Message.Students.Select(StudentDto.ToStudentDto).ToList();
        logger.Debug("Get student successfully! Count: {0}", result.Count);
        return result;
    }

    public async Task<OneOf<StudentDto, NotFound, Exception>> GetStudentByIdAsync(int id)
    {
        logger.Debug("Send id: {0} to runner {1}",id,nameof(GetStudentByIdMessage));
        var response =
            await endpoint.PubSubAsync<GetStudentByIdMessage, StudentByIdMessage>(new GetStudentByIdMessage(id));

        if (response.Message.Error != null) return response.Message.Error;

        if (response.Message.Student == null) return new NotFound();

        var result = StudentDto.ToStudentDto(response.Message.Student);
        logger.Debug("Get student by id {0} !", result.Id);
        return result;
    }

    public async Task<OneOf<True, False, Exception>> CreateStudentAsync(StudentDto student)
    {
        logger.Debug("Send student to runner {0}", nameof(CreateStudentMessage));
        var response = await endpoint.PubSubAsync<CreateStudentMessage, StudentCreatedMessage>(new CreateStudentMessage(student.MapToMessage()));

        if (response.Message.Error != null)
            return response.Message.Error;

        logger.Debug("Student created successfully!");
        return response.Message.Id > 0 ? new True() : new False();
    }

    public async Task<OneOf<True, False, Exception>> UpdateStudentAsync(StudentDto student)
    {
        logger.Debug("Update send student to runner {0}", nameof(GetUpdateStudentMessage));
        var response = await endpoint.PubSubAsync<GetUpdateStudentMessage, UpdateStudentMessage>(new GetUpdateStudentMessage(student.MapToMessage()));

        if (response.Message.Error != null)
            return response.Message.Error;

        logger.Debug("Updated student {0}!",response.Message.IsUpdated ? " successfully": "failed");
        return response.Message.IsUpdated ? new True() : new False();
=======
public class StudentService : IStudentService
{
    ILogger<StudentService> _logger;
    private IPublishEndpoint _publishEndpoint;
    private StudentDbContext db;


    public StudentService(ILogger<StudentService> logger, IPublishEndpoint bus)
    {
        _logger = logger;
        _publishEndpoint = bus;
        db = StudentDbContext.Instance;
    }

    public async Task<IList<Connection.Models.Student>> GetStudentsAsync()
    {
        await _publishEndpoint.Publish(new CreateMessage
        {
            Message = "Hllp"
        });

        _logger.LogDebug("Grab all Students");
        return await db.GetAllAsync();
    }

    public async Task<Connection.Models.Student?> GetStudentByIdAsync(int id)
    {
        return await db.GetById(id);
    }

    public async Task<bool> CreateStudentAsync(Connection.Models.Student student)
    {
        return (await db.Create(student)) is not null;
    }

    public async Task<bool> UpdateStudentAsync(Connection.Models.Student student)
    {
        return await db.Update(student);
>>>>>>> origin/other
    }

    public async Task<OneOf<True, False, Exception>> DeleteStudentAsync(int id)
    {
<<<<<<< HEAD
        logger.Debug("Delete send student to runner {0}", nameof(GetDeleteStudentMessage));
        var response = await endpoint.PubSubAsync<GetDeleteStudentMessage, DeleteStudentMessage>(new GetDeleteStudentMessage(id));

        if (response.Message.Error != null)
            return response.Message.Error;

        logger.Debug("Deleted student {0}!", response.Message.IsDeleted ? " successfully" : "failed");
        return response.Message.IsDeleted ? new True() : new False();
=======
        return await db.Delete(id);
>>>>>>> origin/other
    }
}