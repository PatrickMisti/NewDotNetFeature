using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connectivity.Messages;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using StudentRunner.Communication;

namespace Testing.CommunicationTests;

public class MassTransitTests
{
    [Test]
    public async Task TestConsumer()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<CreateStudentCommunication>();
                cfg.UsingRabbitMq((context, config) =>
                {
                    config.ConfigureEndpoints(context);
                    config.Host("localhost", "/", c =>
                    {
                        c.Username("guest");
                        c.Password("guest");
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<StudentCreatedMessage>();

        await client.GetResponse<CreateStudentMessage>(new CreateStudentMessage(
            new StudentClassMessage 
            { 
                Id = 0, 
                FirstName = "Herbert", 
                LastName = "Stuetz", 
                Birthday = DateTime.UtcNow, 
                Classroom = 1
            }));

        Assert.That(await harness.Sent.Any<CreateStudentMessage>(), Is.True);
        Assert.That(await harness.Consumed.Any<StudentCreatedMessage>(), Is.True);

        var consumerHarness = harness.GetConsumerHarness<CreateStudentCommunication>();

        Assert.That(await consumerHarness.Consumed.Any<CreateStudentMessage>());
    }
}