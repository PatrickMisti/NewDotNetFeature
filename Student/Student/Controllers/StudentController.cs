<<<<<<< HEAD
﻿using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Student.Dtos;
=======
﻿using Microsoft.AspNetCore.Mvc;
>>>>>>> origin/other
using Student.Services;
using ILogger = Serilog.ILogger;

namespace Student.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController(IStudentService repo, ILogger logger)
{
    [HttpGet]
    [ProducesResponseType<List<StudentDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Route("all")]
<<<<<<< HEAD
    public async Task<IResult> GetAll()
=======
    public async Task<IList<Connection.Models.Student>> GetAll()
>>>>>>> origin/other
    {
        var result = await repo.GetStudentsAsync();
        
        var response = result.Match(
            completed => Results.Ok(completed),
            err =>
            {
                logger.Error("HttpError : ",err.Message);
                return Results.NoContent();
            });


        return response;
    }

    [HttpPut]
<<<<<<< HEAD
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<StudentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Route("byId/{id}")]
    public async Task<IResult> UpdateStudent(int id)
=======
    [Route("create")]
    public async Task<bool> AddStudent(Connection.Models.Student student)
>>>>>>> origin/other
    {
        var result = await repo.GetStudentByIdAsync(id);

        var response = result.Match(
            right => Results.Ok(right),
            failed => Results.BadRequest(),
            err =>
            {
                logger.Error("HttpError : ", err.Message);
                return Results.Conflict();
            });

        return response;
    }

    [HttpPost]
<<<<<<< HEAD
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Route("create")]
    public async Task<IResult> AddStudent(StudentDto student)
=======
    [Route("update")]
    public async Task<bool> UpdateStudent(Connection.Models.Student student)
>>>>>>> origin/other
    {
        var result = await repo.CreateStudentAsync(student);

        var response = result.Match(
            right => Results.Created(), 
            failed => Results.BadRequest(),
            err => 
            {
                logger.Error("HttpError : ", err.Message);
                return Results.Conflict();
            });

        return response;
    }

    [HttpPut]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Route("update")]
    public async Task<IResult> UpdateStudent(StudentDto student)
    {
        var result = await repo.UpdateStudentAsync(student);

        var response = result.Match(
            right => Results.Accepted(),
            failed => Results.BadRequest(),
            err => 
            {
                logger.Error("HttpError : ", err.Message);
                return Results.Conflict();
            });

        return response;
    }

    [HttpDelete]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Route("delete/{id}")]
    public async Task<IResult> DeleteStudent(int id)
    {
        var result = await repo.DeleteStudentAsync(id);

        var response = result.Match(
            right => Results.Ok(),
            failed => Results.BadRequest(),
            err => 
            {
                logger.Error("HttpError : ", err.Message);
                return Results.Conflict();
            });

        return response;
    }
}