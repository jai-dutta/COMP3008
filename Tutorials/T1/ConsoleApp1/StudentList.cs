using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class StudentList
    {
        public static List<Student> Students(int amount)
        {
            List<Student> students = new List<Student>();
            Random rand = new Random();

            for(int i = 0; i<amount; i++)
            {
                Student student = new Student();
                student.Id = 1 + i;
                student.Name = "Student #" + i.ToString();
                student.Uni = "University # " + rand.Next(100);
                students.Add(student);
            }
            return students;
        }
    }
}
