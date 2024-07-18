using Connectivity.Messages;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using StudentRunner.Communication;
using StudentRunner.Mapper;
using Testing.Config;

namespace Testing.CommunicationTests;

public class MassTransitTests
{
    [Test]
    public async Task TestMockCreateStudentConsumer()
    {
        await using var provider = new ServiceCollection().AddMockTestMasstransitCreateStudent().BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var client = harness.GetRequestClient<CreateStudentMessage>();
            var studentConsumer = new CreateStudentMessage(MockData.GetListStudent[0].ToStudentClassMessage());
            var resp = await client.GetResponse<StudentCreatedMessage>(studentConsumer);

            Assert.That(resp.Message.Id, Is.EqualTo(MockData.GetListStudent[0].Id));
            Assert.That(await harness.Sent.Any<StudentCreatedMessage>(), Is.True);
            Assert.That(await harness.Consumed.Any<CreateStudentMessage>(), Is.True);

            var consumerHarness = harness.GetConsumerHarness<CreateStudentCommunication>();

            Assert.That(await consumerHarness.Consumed.Any<CreateStudentMessage>());
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task TestGetMockStudentConsumer()
    {
        await using var provider = new ServiceCollection().AddMockTestMasstransitGetStudent().BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var client = harness.GetRequestClient<GetAllStudentMessage>();
            var resp = await client.GetResponse<AllStudentMessage>(GetAllStudentMessage.Instance);

            Assert.That(resp.Message.Students.Count, Is.EqualTo(MockData.GetListStudent.Count));
            Assert.That(await harness.Sent.Any<AllStudentMessage>(), Is.True);
            Assert.That(await harness.Consumed.Any<GetAllStudentMessage>(), Is.True);

            var consumerHarness = harness.GetConsumerHarness<GetStudentCommunication>();

            Assert.That(await consumerHarness.Consumed.Any<GetAllStudentMessage>());
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    [TestCase(1, ExpectedResult = true)]
    [TestCase(3, ExpectedResult = false)]
    public async Task<bool> TestGetMockStudentByIdConsumer(int id)
    {
        await using var provider = new ServiceCollection().AddMockTestMasstransitGetStudentById().BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var client = harness.GetRequestClient<GetStudentByIdMessage>();
            var resp = await client.GetResponse<StudentByIdMessage>(new GetStudentByIdMessage(id));

            Assert.That(await harness.Sent.Any<StudentByIdMessage>(), Is.True);
            Assert.That(await harness.Consumed.Any<GetStudentByIdMessage>(), Is.True);

            var consumerHarness = harness.GetConsumerHarness<GetStudentByIdCommunication>();

            Assert.That(await consumerHarness.Consumed.Any<GetStudentByIdMessage>());
            return resp.Message.Student != null;
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task TestUpdateMockStudentConsumer()
    {
        await using var provider = new ServiceCollection().AddMockTestMasstransitUpdateStudent().BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var client = harness.GetRequestClient<GetUpdateStudentMessage>();
            var resp = await client.GetResponse<UpdateStudentMessage>(new GetUpdateStudentMessage(MockData.GetListStudent[0].ToStudentClassMessage()));

            Assert.That(resp.Message.IsUpdated, Is.True);
            Assert.That(await harness.Sent.Any<UpdateStudentMessage>(), Is.True);
            Assert.That(await harness.Consumed.Any<GetUpdateStudentMessage>(), Is.True);

            var consumerHarness = harness.GetConsumerHarness<UpdateStudentCommunication>();

            Assert.That(await consumerHarness.Consumed.Any<GetUpdateStudentMessage>());
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task TestDeleteMockStudentConsumer()
    {
        await using var provider = new ServiceCollection().AddMockTestMasstransitDeleteStudent().BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var client = harness.GetRequestClient<GetDeleteStudentMessage>();
            var resp = await client.GetResponse<DeleteStudentMessage>(new GetDeleteStudentMessage(MockData.GetListStudent[0].Id));

            Assert.That(resp.Message.IsDeleted, Is.True);
            Assert.That(await harness.Sent.Any<DeleteStudentMessage>(), Is.True);
            Assert.That(await harness.Consumed.Any<GetDeleteStudentMessage>(), Is.True);

            var consumerHarness = harness.GetConsumerHarness<DeleteStudentCommunication>();

            Assert.That(await consumerHarness.Consumed.Any<GetDeleteStudentMessage>());
        }
        finally
        {
            await harness.Stop();
        }
    }
}