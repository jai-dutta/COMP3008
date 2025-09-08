namespace Web_API.Models.Student
{
    public class StudentList
    {
        
        private static readonly List<string> _firstNames = ["Adam", "Jack", "Emma", "Olivia", "Liam", "Sophia" ];
        private static readonly List<string> _lastNames  = ["Smith", "Johnson", "Brown", "Davis", "Miller", "Wilson" ];
        private static int _nextId = 1;
        public static List<Student> Students { get; } = GenerateStudents(1);
        

        public static Student? GetStudent(int id)
        {
            return Students.FirstOrDefault(s => s.Id == id);
        }

        public static void AddStudent(Student student)
        {
            student.SetId(_nextId++);
            Students.Add(student);
        }

        private static List<Student> GenerateStudents(int amount)
        {
            var students = new List<Student>();
            var random = new Random();
            for (int i = 0; i < amount; i++)
            {
                var firstNameIndex = random.Next(0, _firstNames.Count);
                var lastNameIndex = random.Next(0, _lastNames.Count);

                var student = new Student()
                {
                    Age = random.Next(17, 100),
                    Name = _firstNames[firstNameIndex] + " " +  _lastNames[lastNameIndex]
                };
                student.SetId(_nextId++);
                students.Add(student);
            }

            return students;
        }
    }
}
