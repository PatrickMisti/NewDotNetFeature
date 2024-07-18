using StudentRunner.Resources;
using Testing.Config;

namespace Testing.CommunicationTests
{
    public class MockStudentRepo : IStudentRepository
    {
        public Task<IList<StudentRunner.Model.Student>> GetAll()
        {
            return Task.FromResult(MockData.GetListStudent);
        }

        public Task<StudentRunner.Model.Student?> FindById(int id)
        {
            StudentRunner.Model.Student? student = MockData.GetListStudent.FirstOrDefault(item => item.Id == id);
            return Task.FromResult(student);
        }

        public Task<int> Create(StudentRunner.Model.Student entity)
        {
            return Task.FromResult(1);
        }

        public Task<bool> Update(StudentRunner.Model.Student entity)
        {
            return Task.FromResult(true);
        }

        public Task<bool> Delete(int id)
        {
            return Task.FromResult(true);
        }
    }
}
