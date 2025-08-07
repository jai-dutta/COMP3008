using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students
{
    public class StudentList
    {
        public static List<Student> StudentsFromCsv(String filePath)
        {

            List<Student> students = new List<Student>();

            StreamReader sr = new StreamReader(filePath);

            while (sr.ReadLine() != null)
            {
                string[] line = sr.ReadLine().Split(",");
                Student student = new Student();

                student.Id = int.Parse(line[0]);
                student.Name = line[1];
                student.Uni = line[2];

                students.Add(student);

            }

            return students;
        }
    }
}
