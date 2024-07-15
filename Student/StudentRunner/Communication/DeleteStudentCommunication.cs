using Connectivity.Messages;
using MassTransit;
using StudentRunner.Resources;
using ILogger = Serilog.ILogger;

namespace StudentRunner.Communication;

public class DeleteStudentCommunication(IStudentRepository repo, ILogger logger): IConsumer<GetDeleteStudentMessage>
{
    public async Task Consume(ConsumeContext<GetDeleteStudentMessage> context)
    {
        try
        {
            logger.Debug("Got {0} message!", nameof(GetDeleteStudentMessage));
            var id = context.Message.Id;
            var result = await repo.Delete(id);
            logger.Debug("Delete was {0}", result ? "successfully" : "failure");
            await context.RespondAsync(new DeleteStudentMessage
            {
                IsDeleted = result
            });
        }
        catch (Exception e)
        {
            logger.Error("Delete student occurs a exception!", e.Message);
            await context.RespondAsync(new DeleteStudentMessage
            {
                Error = e
            });
        }
    }
}