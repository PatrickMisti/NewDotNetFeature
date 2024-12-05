using Connection.Services;
using Connectivity;
using MassTransit;


namespace Student.Services;

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
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        return await db.Delete(id);
    }
}