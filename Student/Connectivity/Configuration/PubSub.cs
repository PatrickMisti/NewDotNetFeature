using MassTransit;

namespace Connectivity.Configuration;

public class PubSub(IBus bus)
{
    public async Task<Response<TRes>> PubSubAsync<TReq, TRes>(TReq request) where TReq : class where TRes : class
    {
        var factory = bus.CreateClientFactory();
        var requestClient = factory.CreateRequestClient<TReq>();
        return await requestClient.GetResponse<TRes>(request);
    }

}