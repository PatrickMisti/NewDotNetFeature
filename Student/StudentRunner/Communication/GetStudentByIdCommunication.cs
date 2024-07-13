using Connectivity.Messages;
using MassTransit;
using StudentRunner.Mapper;
using StudentRunner.Resources;
using ILogger = Serilog.ILogger;

namespace StudentRunner.Communication;

public class GetStudentByIdCommunication(StudentRepository repo, ILogger logger): IConsumer<GetStudentByIdMessage>
{
    public async Task Consume(ConsumeContext<GetStudentByIdMessage> context)
    {
        try
        {
            logger.Debug("Got {0} message!", nameof(GetStudentByIdMessage));
            var id = context.Message.Id;
            var result = await repo.FindById(id);
            logger.Debug("Could find a student: {0}", result == null ? "no" : "yes");
            await context.RespondAsync(new StudentByIdMessage
            {
                Student = result?.ToStudentClassMessage()
            });
        }
        catch (Exception e)
        {
            logger.Error("Get student by id occurs a exception!", e.Message);
            await context.RespondAsync(new StudentByIdMessage
            {
                Error = e
            });
        }
    }
}