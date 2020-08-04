using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace App
{
    public class Program
    {
        public static void Main()
        {
            //in APS.net
            //no need to use
            //service.AddDbContext<SchoolContext>(opt=>opt...) cuz it only accept optbuilder instead

            //service.AddScope(_ => new SchoolContext(GetConnectionString(), true))  

            var result4 = Execute(x => x.EditPersonalInfo(3, "Carl 2", "Carson 2", 1, "carl1@gmail.com", 1));
            //var result4 = Execute(x => x.RegisterStudent("Carl","carl@gmail.com", 2, Grade.C));
            //var result3 = Execute(x => x.DisnrollStudent(1, 2));
            //var result2 = Execute(x => x.CheckStudentFavoriteCourse(1, 2));
            //var result1 = Execute(x => x.EnrollStudent(1, 2, Grade.A));
        }

        private static string Execute(Func<StudentController, string> func)
        {
            string connectionString = GetConnectionString();
            IBus bus = new Bus();
            var messagerBus = new MessageBus(bus);
            var dispacher = new EventDispacher(messagerBus);

            using (var context = new SchoolContext(connectionString, true, dispacher))
            {
                var controller = new StudentController(context);
                return func(controller);
            }
        }

        private static string GetConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration["ConnectionString"];
        }
    }
}
