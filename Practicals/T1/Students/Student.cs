using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Students
{
    public class Student : Person
    {
        private int id;
        private string uni;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Uni
        { 
            get { return uni; } 
            set {  uni = value; } 
        }

        public override string ToString()
        {
            string info = 
                        "Name: " + Name + "\n" +
                        "ID: " + id + "\n" +
                        "University: " + uni;

            return info;
        }
    }
}
