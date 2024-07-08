using MassTransit;
using ILogger = Serilog.ILogger;

namespace Student.Bussystem;

public class StudentTransitPublisher
{
    private IBus _bus;
    private ILogger _logger;

    public StudentTransitPublisher( ILogger logger)
    {

    }

    public void SendPackage(string studentName)
    {
        
    }
}