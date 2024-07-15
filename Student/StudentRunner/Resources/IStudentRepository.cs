using StudentRunner.Model;

namespace StudentRunner.Resources;

public interface IStudentRepository
{
    Task<IList<Student>> GetAll();

    Task<Student?> FindById(int id);

    Task<int> Create(Student entity);

    Task<bool> Update(Student entity);

    Task<bool> Delete(int id);
}