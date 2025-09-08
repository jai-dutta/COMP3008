using Microsoft.AspNetCore.Mvc;
using Web_API.Models.Student;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        // GET api/student/
        [HttpGet]
        public IEnumerable<Student> GetAll()
        {
            return StudentList.Students;
        }
        // GET api/student/{id}
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var student = StudentList.GetStudent(id);
            return student == null ? NotFound() : Ok(student);
        }

        // POST /api/student
        [HttpPost]
        public IActionResult Create([FromBody] Student newStudent)
        {
            if (newStudent == null)
            {
                return BadRequest();
            }
            StudentList.AddStudent(newStudent);
            return CreatedAtAction(nameof(Get), new { id = newStudent.Id }, newStudent);
        }

    }
}
