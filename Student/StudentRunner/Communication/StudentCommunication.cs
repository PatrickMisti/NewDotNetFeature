using Connectivity.Messages;
using MassTransit;
using StudentRunner.Mapper;
using StudentRunner.Resources;

namespace StudentRunner.Communication;

public class StudentCommunication: IConsumer<CreateStudentMessage>
{
    StudentRepository repo = StudentRepository.Instance;
    public async Task Consume(ConsumeContext<CreateStudentMessage> context)
    {
        var i = context.Message;
        var s = await HandleCreateStudent(i);
        await context.RespondAsync(s);
    }

    public async Task<StudentCreated> HandleCreateStudent(CreateStudentMessage message)
    {
        var student = message.ToStudent();
        var i = await repo.Create(student);
        return new StudentCreated(i, student.FirstName);
    }
}