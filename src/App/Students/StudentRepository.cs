using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Students
{
    public sealed class StudentRepository
    {
        private readonly SchoolContext _schoolContext;

        public StudentRepository(SchoolContext schoolContext)
        {
            _schoolContext = schoolContext;
        }

        public Student GetById(long id)
        {
            //Student student = _schoolContext.Students
            //    .Include(x => x.Enrollments)
            //    .SingleOrDefault(x => x.Id == studentId);

            //vlad recomends this way even 2 trips to db it is simple pro is that identy map works better find method
            Student student = _schoolContext.Students.Find(id);

            if (student == null)
                return null;

            _schoolContext.Entry(student).Collection(x => x.Enrollments).Load();

            return student;
        }

        public void Save(Student student)
        {
            _schoolContext.Students.Attach(student);

            //throws exception when using cached objects. it gives entity state as "Added" for the esisting object
            //_schoolContext.Students.Add(student); //When adding new object avoid Add and Update use attach
        }
    }
}
