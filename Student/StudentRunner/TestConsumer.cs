using Connectivity;
using MassTransit;

namespace StudentRunner
{
    public class TestConsumer: IConsumer<CreateMessage>
    {
        public Task Consume(ConsumeContext<CreateMessage> context)
        {
            Console.WriteLine("Message respond: "+ context.Message);

            return Task.CompletedTask;
        }
    }
}
