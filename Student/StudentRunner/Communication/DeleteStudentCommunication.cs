using Connectivity.Messages;
using MassTransit;
using StudentRunner.Resources;

namespace StudentRunner.Communication;

public class DeleteStudentCommunication: IConsumer<GetDeleteStudentMessage>
{
    StudentRepository repo = StudentRepository.Instance;
    public async Task Consume(ConsumeContext<GetDeleteStudentMessage> context)
    {
        try
        {
            var id = context.Message.Id;
            var result = await repo.Delete(id);
            await context.RespondAsync(new DeleteStudentMessage
            {
                IsDeleted = result
            });
        }
        catch (Exception e)
        {
            await context.RespondAsync(new DeleteStudentMessage
            {
                Error = e
            });
        }
    }
}