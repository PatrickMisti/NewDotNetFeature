using MassTransit;
using Serilog;

namespace Connectivity.Configuration;

public class PubSub(IBus bus, ILogger logger)
{
    public async Task<Response<TRes>> PubSubAsync<TReq, TRes>(TReq request) where TReq : class where TRes : class
    {
        logger.Information("Send PubSub request {0} and get response {1}",typeof(TReq).Name, typeof(TRes).Name);
        var factory = bus.CreateClientFactory();
        var requestClient = factory.CreateRequestClient<TReq>();
        return await requestClient.GetResponse<TRes>(request);
    }
}