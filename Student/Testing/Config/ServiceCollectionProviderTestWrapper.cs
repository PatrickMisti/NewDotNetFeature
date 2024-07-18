using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StudentRunner.Communication;
using StudentRunner.Resources;
using Testing.CommunicationTests;

namespace Testing.Config;

public static class ServiceCollectionProviderTestWrapper
{
    private static void AddMockStudentService(this IServiceCollection cfg)
    {
        cfg.AddSingleton(Log.Logger);
        cfg.AddSingleton<IStudentRepository, MockStudentRepo>();
    }

    public static IServiceCollection AddMockTestMasstransitCreateStudent(this IServiceCollection cfg)
    {
        cfg.AddMockStudentService();
        cfg.AddMassTransitTestHarness(ctx =>
        {
            ctx.AddConsumer<CreateStudentCommunication>();
        });
        return cfg;
    }

    public static IServiceCollection AddMockTestMasstransitGetStudent(this IServiceCollection cfg)
    {
        cfg.AddMockStudentService();
        cfg.AddMassTransitTestHarness(ctx =>
        {
            ctx.AddConsumer<GetStudentCommunication>();
        });
        return cfg;
    }

    public static IServiceCollection AddMockTestMasstransitGetStudentById(this IServiceCollection cfg)
    {
        cfg.AddMockStudentService();
        cfg.AddMassTransitTestHarness(ctx =>
        {
            ctx.AddConsumer<GetStudentByIdCommunication>();
        });
        return cfg;
    }

    public static IServiceCollection AddMockTestMasstransitUpdateStudent(this IServiceCollection cfg)
    {
        cfg.AddMockStudentService();
        cfg.AddMassTransitTestHarness(ctx =>
        {
            ctx.AddConsumer<UpdateStudentCommunication>();
        });
        return cfg;
    }

    public static IServiceCollection AddMockTestMasstransitDeleteStudent(this IServiceCollection cfg)
    {
        cfg.AddMockStudentService();
        cfg.AddMassTransitTestHarness(ctx =>
        {
            ctx.AddConsumer<DeleteStudentCommunication>();
        });
        return cfg;
    }
}