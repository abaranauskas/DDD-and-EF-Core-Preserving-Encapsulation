using App.Common;
using CSharpFunctionalExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace App
{
    public class Student : Entity
    {
        protected Student() //concession of using ORm in this case EF
        {
        }

        public Student(Name name, Email email, Course favoriteCourse, Grade favoriteCourseGrade)
        {
            Name = name;
            Email = email;
            FavoriteCourse = favoriteCourse;

            EnrollIn(favoriteCourse, favoriteCourseGrade);
        }

        public virtual Name Name { get; private set; }
        public Email Email { get; private set; }
        public virtual Course FavoriteCourse { get; private set; }
        //navigation virtual required cuz of lazy lauding and not sealed clas and has to has atleats protected paramaterles ctor

        private List<Enrollment> _enrollments = new List<Enrollment>();
        public virtual IReadOnlyList<Enrollment> Enrollments => _enrollments.ToList();
        //ToList enasures that original collection stays the same evan if client performes manual cats

        internal Result CanEnroll(Course course)
        {
            if (_enrollments.Any(x => x.Course == course))
                return Result.Failure($"Already enrolled in course '{course.Name}'");

            return Result.Success();
        }

        internal void EnrollIn(Course course, Grade grade)
        {
            if (CanEnroll(course).IsFailure)
                throw new Exception();

            if (course == null)
                throw new ArgumentNullException(nameof(course));

            _enrollments.Add(new Enrollment(course, this, grade));
        }

        internal Result CanDisenroll(Course course)
        {
            if (!_enrollments.Any(x => x.Course == course))
                return Result.Failure($"There is no student in '{course.Name}' course");

            return Result.Success();
        }

        internal void Disenroll(Course course)
        {
            if (CanDisenroll(course).IsFailure)
                throw new Exception();

            var enrollmentToRemove = _enrollments.FirstOrDefault(x => x.Course == course);

            _enrollments.Remove(enrollmentToRemove);
        }

        internal void EditPersonalInfo(Name name, Email email, Course favoriteCourse)
        {
            if (name == null || email == null || favoriteCourse == null)
                throw new ArgumentNullException();

            if (Email != email)
                RaiseEvent(new StudentEmailChangedEvent(Id, email));

            Name = name;
            Email = email;
            FavoriteCourse = favoriteCourse;
        }
    }
}
