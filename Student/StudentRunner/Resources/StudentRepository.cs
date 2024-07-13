using StudentRunner.Model;

namespace StudentRunner.Resources;

public class StudentRepository: Repository<Student>
{
    public StudentRepository(Database db):base(db)
    {
        db.InitDb();
    }
}