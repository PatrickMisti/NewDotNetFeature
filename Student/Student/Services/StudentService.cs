using Connectivity;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Student.Resource;

namespace Student.Services;

public class StudentService(Database db, ILogger<StudentService> logger, IPublishEndpoint bus) : IStudentService
{
    public async Task<IList<Models.Student>> GetStudentsAsync()
    {
        await bus.Publish(new CreateMessage
        {
            Message = "Hllp"
        });

        if (db.Database.EnsureCreated())
        {

        }

        logger.LogDebug("Grab all Students");
        return await db.Students.ToListAsync();
    }

    public async Task<Models.Student?> GetStudentByIdAsync(int id)
    {
        return await db.Students.FirstOrDefaultAsync(student => student.Id == id);
    }

    public async Task<bool> CreateStudentAsync(Models.Student student)
    {
        await db.Students.AddAsync(student);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateStudentAsync(Models.Student student)
    {
        var oldStudent = await GetStudentByIdAsync(student.Id);
        if (oldStudent is null) return false;

        student.Id = default;
        db.Students.Add(student);

        oldStudent.Deleted = true;
        oldStudent.TimeStamp = DateTime.UtcNow;

        db.Students.Update(oldStudent);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await GetStudentByIdAsync(id);
        if (student is null) return false;
        student.Deleted = true;
        student.TimeStamp = DateTime.Now;
        db.Students.Update(student);
        return await db.SaveChangesAsync() > 0;
    }
}