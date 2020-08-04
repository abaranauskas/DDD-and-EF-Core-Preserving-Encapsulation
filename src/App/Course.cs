using App.Common;
using System.Linq;

namespace App
{
    public class Course : Entity
    {
        //Enumeration pattern
        public static readonly Course Calculus = new Course(1, "Calculus");
        public static readonly Course Chemistry = new Course(2, "Chemistry");

        public static readonly Course[] AllCourses = { Calculus, Chemistry };

        protected Course()
        {
        }

        private Course(long id, string name)
            : base(id)
        {
            Name = name;
        }

        public string Name { get; set; }

        public static Course FromId(long id)
        {
            return AllCourses.SingleOrDefault(x => x.Id == id);
        }
    }
}
