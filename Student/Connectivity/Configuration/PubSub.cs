using MassTransit;

namespace Connectivity.Configuration
{
    public class PubSub<TResponse>(IPublishEndpoint bus): IConsumer<TResponse> where TResponse : class
    {
        private static Action<TResponse>? _func;
        public Task Consume(ConsumeContext<TResponse> context)
        {
            _func?.Invoke(context.Message);
            return Task.CompletedTask;
        }

        public async Task<TResponse> PublishSubscribeAsync<TRequest>(TRequest message)
        {

            var tcs = new TaskCompletionSource<TResponse>();
            var task = tcs.Task;

            _func = response =>
            {
                tcs.SetResult(response);
            };

            await bus.Publish(message!);

            var i = await task;

            return i;
        }

    }
}
