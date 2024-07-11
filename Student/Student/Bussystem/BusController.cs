using MassTransit;
using MassTransit.Clients;
using MassTransit.DependencyInjection;

namespace Student.Bussystem
{
    public class BusController: BusInstance<IBus>
    {
        private IBusControl _control;
        public BusController(IBusControl busControl) : base(busControl)
        {
            _control = busControl;
        }

        public async Task<Response<TResponse>> PubSubAsync<TRequest,TResponse>(TRequest request) where TRequest: class where TResponse : class 
        {
            var i = _control.CreateRequestClient<TRequest>();
            return await i.GetResponse<TResponse>(request);
        }
    }
}
