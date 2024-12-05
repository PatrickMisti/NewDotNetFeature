using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connection.Models;

namespace Connection.Services;

public class StudentDbContext : Repository<Student>
{
    private static readonly Lazy<StudentDbContext> _instance =
        new (() => new StudentDbContext());

    public static StudentDbContext Instance => _instance.Value;

    private StudentDbContext() : base(new Database())
    {
        
    }


}