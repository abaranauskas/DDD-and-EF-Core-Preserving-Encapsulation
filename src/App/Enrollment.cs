using App.Common;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    public class Enrollment : Entity
    {
        public Enrollment()
        {
        }

        public Enrollment(Course course, Student student, Grade grade)
        {
            Course = course;
            Student = student;
            Grade = grade;
        }

        public Grade Grade { get;  }
        public virtual Course Course { get; }
        public virtual Student Student { get; }
    }

    public enum Grade
    {
        A,
        B,
        C,
        D,
        F
    }
}
