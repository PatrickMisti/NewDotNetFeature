using Connectivity.Messages;
using MassTransit;
using StudentRunner.Mapper;
using StudentRunner.Resources;
using ILogger = Serilog.ILogger;

namespace StudentRunner.Communication;

public class CreateStudentCommunication(IStudentRepository repo, ILogger logger): IConsumer<CreateStudentMessage>
{
    public async Task Consume(ConsumeContext<CreateStudentMessage> context)
    {
        try
        {
            logger.Debug("Got {0} message!", nameof(CreateStudentMessage));
            var response = context.Message.Student;
            var id = await repo.Create(response.ToStudent());
            logger.Debug("Create student was {0}", id > 0 ? "successfully" : "failure");
            await context.RespondAsync(new StudentCreatedMessage
            {
                Id = id
            });
        }
        catch (Exception e)
        {
            logger.Error("Create student occurs a exception!", e.Message);
            await context.RespondAsync(new StudentCreatedMessage
            {
                Error = e
            });
        }
    }
}