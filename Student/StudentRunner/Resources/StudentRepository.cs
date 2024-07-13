using StudentRunner.Model;

namespace StudentRunner.Resources;

public class StudentRepository(Database db): Repository<Student>(db)
{
}