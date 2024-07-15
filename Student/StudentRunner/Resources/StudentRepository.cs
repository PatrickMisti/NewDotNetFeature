using StudentRunner.Model;

namespace StudentRunner.Resources;

public class StudentRepository: Repository<Student>, IStudentRepository
{
    public StudentRepository(Database db):base(db)
    {
        db.InitDb();
    }
}