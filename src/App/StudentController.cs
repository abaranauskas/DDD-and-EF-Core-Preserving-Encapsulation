using App.Students;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace App
{
    public sealed class StudentController
    {
        private readonly SchoolContext _schoolContext;
        private readonly StudentRepository _studentRepository;

        public StudentController(SchoolContext schoolContext)
        {
            _schoolContext = schoolContext;
            _studentRepository = new StudentRepository(_schoolContext);
        }

        //not patr o domain model it is application service
        public string CheckStudentFavoriteCourse(long studentId, long courseId)
        {
            Student student = _schoolContext.Students.Find(studentId);

            if (student == null)
                return "Student not found";

            var course = Course.FromId(courseId);

            if (course == null)
                return "Cource not found";

            return student.FavoriteCourse == course ? "YES" : "NO";
        }

        public string EnrollStudent(long studentId, long courseId, Grade grade)
        {
            Student student = _studentRepository.GetById(studentId);
            if (student == null)
                return "Student not found";

            var course = Course.FromId(courseId);
            if (course == null)
                return "Cource not found";

            var canEnroll = student.CanEnroll(course);
            if (canEnroll.IsFailure)
                return canEnroll.Error;

            student.EnrollIn(course, grade);

            _schoolContext.SaveChanges();

            return "ok";
        }

        public string DisnrollStudent(long studentId, long courseId)
        {
            Student student = _studentRepository.GetById(studentId);
            if (student == null)
                return "Student not found";

            var course = Course.FromId(courseId);
            if (course == null)
                return "Cource not found";

            var canEnroll = student.CanDisenroll(course);
            if (canEnroll.IsFailure)
                return canEnroll.Error;

            student.Disenroll(course);

            _schoolContext.SaveChanges();

            return "ok";
        }

        public string RegisterStudent(string firstName, string lastName, long nameSuffixId,
            string email, long favoriteCourseId, Grade favoriteCourseGrade)
        {
            var course = Course.FromId(favoriteCourseId);
            if (course == null)
                return "Cource not found";

            var suffix = Suffix.FromId(nameSuffixId);
            if (suffix == null)
                return "Suffix not found";

            var emailEesult = Email.Create(email);
            if (emailEesult.IsFailure)
                return emailEesult.Error;

            var nameResult = Name.Create(firstName, lastName, suffix);
            if (nameResult.IsFailure)
                return nameResult.Error;

            var student = new Student(nameResult.Value, emailEesult.Value, course, favoriteCourseGrade);

            _studentRepository.Save(student);

            _schoolContext.SaveChanges();

            return "OK";
        }

        public string EditPersonalInfo(
            long studentId, string firstName, string lastName, long nameSuffixId,
            string email, long favoriteCourseId) //in real app it should be in dto
        {
            Student student = _schoolContext.Students.Find(studentId);

            if (student == null)
                return "Student not found";

            var favoriteCourse = Course.FromId(favoriteCourseId);

            if (favoriteCourse == null)
                return "Cource not found";

            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
                return emailResult.Error;

            var suffix = Suffix.FromId(nameSuffixId);
            if (suffix == null)
                return "Suffix not found";

            var nameResult = Name.Create(firstName, lastName, suffix);
            if (nameResult.IsFailure)
                return nameResult.Error;

            student.EditPersonalInfo(nameResult.Value, emailResult.Value, favoriteCourse);

            _schoolContext.SaveChanges();

            return "ok";

        }
    }
}
