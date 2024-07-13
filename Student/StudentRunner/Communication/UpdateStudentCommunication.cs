using Connectivity.Messages;
using MassTransit;
using StudentRunner.Mapper;
using StudentRunner.Resources;

namespace StudentRunner.Communication;

public class UpdateStudentCommunication: IConsumer<GetUpdateStudentMessage>
{
    StudentRepository repo = StudentRepository.Instance;
    public async Task Consume(ConsumeContext<GetUpdateStudentMessage> context)
    {
        try
        {
            var student = context.Message.Student;
            var result = await repo.Update(student.ToStudent());
            await context.RespondAsync(new UpdateStudentMessage
            {
                IsUpdated = result
            });
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new UpdateStudentMessage
            {
                Error = ex
            });
        }
    }
}