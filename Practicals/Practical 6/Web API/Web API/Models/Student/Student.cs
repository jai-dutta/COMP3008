using System.ComponentModel.DataAnnotations;

namespace Web_API.Models.Student
{
    public class Student
    {
        [Required]
        public int Id { get; private set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }

        public Student() { }

        public Student(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }

        internal void SetId(int id)
        {
            Id = id;
        }

    }
}
