using Connectivity.Messages;
using MassTransit;
using StudentRunner.Mapper;
using StudentRunner.Resources;

namespace StudentRunner.Communication;

public class CreateStudentCommunication: IConsumer<CreateStudentMessage>
{
    StudentRepository repo = StudentRepository.Instance;
    public async Task Consume(ConsumeContext<CreateStudentMessage> context)
    {
        try
        {
            var response = context.Message.Student;
            var id = await repo.Create(response.ToStudent());
            await context.RespondAsync(new StudentCreatedMessage
            {
                Id = id
            });
        }
        catch (Exception e)
        {
            await context.RespondAsync(new StudentCreatedMessage
            {
                Error = e
            });
        }
    }
}