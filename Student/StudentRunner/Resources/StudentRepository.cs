using StudentRunner.Model;

namespace StudentRunner.Resources;

internal class StudentRepository: Repository<Student>
{
    private static Lazy<StudentRepository> _instance = new (() => new StudentRepository(new Database()));
    public static StudentRepository Instance => _instance.Value;

    private Database _repo;
    private StudentRepository(Database db): base(db)
    {
        _repo = db;
    }
}