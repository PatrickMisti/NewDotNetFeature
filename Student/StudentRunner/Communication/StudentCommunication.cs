using System.Text.Json;
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
        //var i = JsonSerializer.Deserialize<IStudentMessage>(context.Message)!;
        var i = context.Message;
        if (i is null) return;
        await HandleCreateStudent(i);
    }

    public async Task HandleCreateStudent(CreateStudentMessage message)
    {
        var student = message.ToStudent();
        await repo.Create(student);
    }
}