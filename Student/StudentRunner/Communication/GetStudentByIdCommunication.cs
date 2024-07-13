using Connectivity.Messages;
using MassTransit;
using StudentRunner.Mapper;
using StudentRunner.Resources;

namespace StudentRunner.Communication;

public class GetStudentByIdCommunication: IConsumer<GetStudentByIdMessage>
{
    StudentRepository repo = StudentRepository.Instance;
    public async Task Consume(ConsumeContext<GetStudentByIdMessage> context)
    {
        try
        {
            var id = context.Message.Id;
            var result = await repo.FindById(id);
            await context.RespondAsync(new StudentByIdMessage
            {
                Student = result?.ToStudentClassMessage()
            });
        }
        catch (Exception e)
        {
            await context.RespondAsync(new StudentByIdMessage
            {
                Error = e
            });
        }
    }
}