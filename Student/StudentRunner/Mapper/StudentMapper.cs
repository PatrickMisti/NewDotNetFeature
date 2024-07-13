using Connectivity.Messages;
using StudentRunner.Model;

namespace StudentRunner.Mapper
{
    public static class StudentMapper
    {
        public static Student ToStudent(this StudentClassMessage message)
        {
            return new Student
            {
                Id = message.Id,
                FirstName = message.FirstName,
                LastName = message.LastName,
                BirthDay = message.Birthday.ToUniversalTime(),
                Classroom = message.Classroom
            };
        }

        public static StudentClassMessage ToStudentClassMessage(this Student student)
        {
            return new StudentClassMessage
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Birthday = student.BirthDay,
                Classroom = student.Classroom
            };
        }
    }
}
