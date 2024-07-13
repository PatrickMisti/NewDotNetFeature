using Connectivity.Messages;
using MassTransit;
using StudentRunner.Mapper;
using StudentRunner.Resources;

namespace StudentRunner.Communication;

public class GetStudentCommunication: IConsumer<GetAllStudentMessage>
{
    StudentRepository _repo = StudentRepository.Instance;
    public async Task Consume(ConsumeContext<GetAllStudentMessage> context)
    {
        try
        {
            var list = await _repo.GetAll();
            var responseList = list.Select(item => item.ToStudentClassMessage()).ToList();
            await context.RespondAsync(new AllStudentMessage
            {
                Students = responseList
            });
        }
        catch (Exception e)
        {
            await context.RespondAsync(new AllStudentMessage
            {
                Error = e
            });
        }
    }
}