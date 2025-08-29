using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int amount = int.Parse(Console.ReadLine());

            List<Student> students = StudentList.Students(amount);
            foreach (Student student in students)
            {
                Console.WriteLine(student);
            }

            Console.ReadLine();
            
        }
    }
}
