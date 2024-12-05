using Microsoft.AspNetCore.Mvc;
using Student.Services;

namespace Student.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController(IStudentService repo)
{
    [HttpGet]
    [Route("all")]
    public async Task<IList<Connection.Models.Student>> GetAll()
    {
        return await repo.GetStudentsAsync();
    }

    [HttpPut]
    [Route("create")]
    public async Task<bool> AddStudent(Connection.Models.Student student)
    {
        return await repo.CreateStudentAsync(student);
    }

    [HttpPost]
    [Route("update")]
    public async Task<bool> UpdateStudent(Connection.Models.Student student)
    {
        return await repo.UpdateStudentAsync(student);
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<bool> DeleteStudent(int id)
    {
        return await repo.DeleteStudentAsync(id);
    }
}