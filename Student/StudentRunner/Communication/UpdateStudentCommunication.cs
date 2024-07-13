using Connectivity.Messages;
using MassTransit;
using StudentRunner.Mapper;
using StudentRunner.Resources;
using ILogger = Serilog.ILogger;

namespace StudentRunner.Communication;

public class UpdateStudentCommunication(ILogger logger): IConsumer<GetUpdateStudentMessage>
{
    StudentRepository repo = StudentRepository.Instance;
    public async Task Consume(ConsumeContext<GetUpdateStudentMessage> context)
    {
        try
        {
            logger.Debug("Got {0} message!", nameof(GetUpdateStudentMessage));
            var student = context.Message.Student;
            var result = await repo.Update(student.ToStudent());
            logger.Debug("Update was {0}", result ? "successfully" : "failure");
            await context.RespondAsync(new UpdateStudentMessage
            {
                IsUpdated = result
            });
        }
        catch (Exception ex)
        {
            logger.Error("Update student occurs a exception!", ex.Message);
            await context.RespondAsync(new UpdateStudentMessage
            {
                Error = ex
            });
        }
    }
}