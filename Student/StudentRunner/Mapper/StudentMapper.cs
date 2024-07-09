using Connectivity.Messages;
using StudentRunner.Model;

namespace StudentRunner.Mapper
{
    public static class StudentMapper
    {
        public static Student ToStudent(this CreateStudentMessage message)
        {
            return new Student
            {
                FirstName = message.FirstName,
                LastName = message.LastName,
                BirthDay = message.Birthday.ToUniversalTime(),
                Classroom = message.Classroom
            };
        }
    }
}
