<<<<<<<< HEAD:Student/StudentRunner/Model/Student.cs
﻿namespace StudentRunner.Model;

public class Student: BaseEntity
========
﻿namespace Connection.Models;

public class Student : BaseEntity
>>>>>>>> origin/other:Student/Connection/Models/Student.cs
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDay { get; set; }
    public int Classroom { get; set; }
}