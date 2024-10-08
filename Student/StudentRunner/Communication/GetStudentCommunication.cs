﻿using Connectivity.Messages;
using MassTransit;
using StudentRunner.Mapper;
using StudentRunner.Resources;
using ILogger = Serilog.ILogger;

namespace StudentRunner.Communication;

public class GetStudentCommunication(IStudentRepository repo, ILogger logger): IConsumer<GetAllStudentMessage>
{
    public async Task Consume(ConsumeContext<GetAllStudentMessage> context)
    {
        try
        {
            logger.Debug("Got {0} message!", nameof(GetAllStudentMessage));
            var list = await repo.GetAll();
            var responseList = list.Select(item => item.ToStudentClassMessage()).ToList();
            logger.Debug("Get all student! Count is {0}", responseList.Count);
            await context.RespondAsync(new AllStudentMessage
            {
                Students = responseList
            });
        }
        catch (Exception e)
        {
            logger.Error("Get all student occurs a exception!", e.Message);
            await context.RespondAsync(new AllStudentMessage
            {
                Error = e
            });
        }
    }
}