using Connectivity.Messages;
using MassTransit;

namespace Student.Bussystem
{
    public class StudentCreateConsumer(ILogger<StudentCreateConsumer> logger): IConsumer<StudentCreated>
    {
        public async Task Consume(ConsumeContext<StudentCreated> context)
        {
            logger.LogInformation("Get context");
            await Task.CompletedTask;
        }
    }
}
