using StudentRunner.Resources;

namespace Testing.DatabaseTests;

public class DbTests
{
    private Database? _db;
    private StudentRepository? _studentRepository;

    [SetUp]
    public void SetUp()
    {
        var connectionString = Database.GetTestTableConnectionString();
        _db = new Database(connectionString.Options);
        _studentRepository = new StudentRepository(_db);
    }

    [TearDown]
    public void TearDown()
    {
        _db?.Database.EnsureDeleted();
        _db?.Dispose();
    }


    [Test]
    public async Task CreateStudent_ShouldAddStudentToDatabase()
    {
        // Arrange
        var student = new StudentRunner.Model.Student
        {
            Id = default,
            FirstName = "John",
            LastName = "Doe",
            BirthDay = DateTime.UtcNow,
            Classroom = 1
        };

        // Act
        await _studentRepository!.Create(student);

        await Task.Delay(50);
        // Assert
        var savedStudent = await _studentRepository.GetAll();
        Assert.That(savedStudent, Has.Member(student));
        Assert.That(savedStudent.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetStudentById_ShouldReturnCorrectStudent()
    {
        // Arrange
        var student = new StudentRunner.Model.Student
        {
            Id = default,
            FirstName = "John",
            LastName = "Doe",
            BirthDay = DateTime.UtcNow,
            Classroom = 1
        };
        await _studentRepository!.Create(student);
        await Task.Delay(50);

        // Act
        var retrievedStudent = await _studentRepository.FindById(student.Id);

        // Assert
        Assert.That(retrievedStudent, Is.EqualTo(student));
    }

    [Test]
    public async Task UpdateStudent_ShouldModifyStudentInDatabase()
    {
        // Arrange
        var student = new StudentRunner.Model.Student
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDay = DateTime.UtcNow,
            Classroom = 1
        };
        await _studentRepository!.Create(student);

        var newRawStudent = new StudentRunner.Model.Student
        {
            Id = student.Id,
            BirthDay = DateTime.UtcNow.AddDays(-2),
            Classroom = 1,
            FirstName = "Patrick",
            LastName = "Misti"
        };

        // Act
        bool isCompete = await _studentRepository.Update(newRawStudent);
        Console.WriteLine("Update student :"+ isCompete);
        await Task.Delay(50);

        // Assert
        var updatedStudent = await _studentRepository.GetAll();
        var newStudent = updatedStudent.FirstOrDefault(item => item.FirstName.Contains(newRawStudent.FirstName));
        Assert.That(updatedStudent, Has.Member(newRawStudent));
        Assert.That(newStudent?.BirthDay, Is.EqualTo(newRawStudent.BirthDay));
    }

    [Test]
    public async Task DeleteStudent_ShouldRemoveStudentFromDatabase()
    {
        // Arrange
        var student = new StudentRunner.Model.Student()
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDay = DateTime.UtcNow,
            Classroom = 1
        };
        await _studentRepository!.Create(student);

        await Task.Delay(250);
        // Act
        await _studentRepository.Delete(student.Id);

        // Assert
        var deletedStudent = await _studentRepository.FindById(student.Id);
        Assert.That(deletedStudent, Is.Null);
    }

    [Test]
    public async Task GetAllStudents_ShouldReturnAllStudents()
    {
        // Arrange
        var students = new List<StudentRunner.Model.Student>
        {
            new () 
            {
                Id = default, 
                FirstName = "John", 
                LastName = "Doe", 
                BirthDay = DateTime.UtcNow, 
                Classroom = 1
                },
            new()
            { 
                Id = default, 
                FirstName = "Patrick", 
                LastName = "Doe", 
                BirthDay = DateTime.UtcNow,
                Classroom = 3
            }
        };
        foreach (var student in students)
        {
            student.Id = await _studentRepository!.Create(student);
        }
        await Task.Delay(50);
        // Act
        var allStudents = await _studentRepository!.GetAll();

        // Assert
        Assert.That(2, Is.EqualTo(allStudents.Count));
        Assert.That(allStudents, Has.Member(students[0]));
        Assert.That(allStudents, Has.Member(students[1]));
    }


}