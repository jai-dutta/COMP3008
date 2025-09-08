
## **1. Models in ASP.NET Core**

**Models** represent **data and business logic** in your application. They are the backbone of MVC/Web API apps.

### **1.1 Simple Model Example**

```csharp
public class Student
{
    public int Id { get; set; }                   // Unique identifier
    public string Name { get; set; }              // Student's name
    public string Address { get; set; }           // Optional address
}
```

- Properties in C# use **PascalCase** by convention.
    
- The model can map directly to a database table if using **Entity Framework Core**.
    

---

### **1.2 Model with Data Annotations**

ASP.NET Core provides **validation attributes**:

```csharp
using System.ComponentModel.DataAnnotations;

public class Student
{
    [Key]  // Primary key
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be 2-50 chars.")]
    public string Name { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
    public int Age { get; set; }
}
```

- `Required` → Cannot be null or empty.
    
- `StringLength` → Limits length.
    
- `Range` → Ensures numeric values fall within a range.
    
- Validation is enforced when **model binding** happens in controllers.
    

---

### **1.3 Complex Models**

Models can **contain nested objects**:

```csharp
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }

    // Nested model
    public Teacher Instructor { get; set; }
}

public class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

- ASP.NET Core **binds nested JSON automatically**.
    

**Example JSON:**

```json
{
  "Id": 101,
  "Title": "Computer Science",
  "Instructor": { "Id": 1, "Name": "Dr. Smith" }
}
```

---

### **1.4 DTOs (Data Transfer Objects)**

- Models often represent database entities.
    
- Use **DTOs** to define what is exposed via API.
    
- Example: hiding internal properties:
    

```csharp
public class StudentDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

- Convert model → DTO in controller:
    

```csharp
var dto = students.Select(s => new StudentDTO { Id = s.Id, Name = s.Name });
```

---

## **2. Controllers in ASP.NET Core**

Controllers **handle incoming HTTP requests**, interact with models/services, and return responses.

### **2.1 API Controller (JSON)**

```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private static List<Student> students = new List<Student>
    {
        new Student { Id = 1, Name = "Alice", Age = 21, Email = "alice@example.com" }
    };

    [HttpGet] // GET api/students
    public IEnumerable<Student> GetAll() => students;

    [HttpGet("{id}")] // GET api/students/1
    public IActionResult Get(int id)
    {
        var student = students.FirstOrDefault(s => s.Id == id);
        return student == null ? NotFound() : Ok(student);
    }

    [HttpPost] // POST api/students
    public IActionResult Create([FromBody] Student s)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        students.Add(s);
        return CreatedAtAction(nameof(Get), new { id = s.Id }, s);
    }

    [HttpPut("{id}")] // PUT api/students/1
    public IActionResult Update(int id, [FromBody] Student s)
    {
        var existing = students.FirstOrDefault(st => st.Id == id);
        if (existing == null) return NotFound();

        // Update fields
        existing.Name = s.Name;
        existing.Age = s.Age;
        existing.Email = s.Email;

        return Ok(existing);
    }

    [HttpDelete("{id}")] // DELETE api/students/1
    public IActionResult Delete(int id)
    {
        var removed = students.RemoveAll(s => s.Id == id);
        return removed == 0 ? NotFound() : NoContent();
    }
}
```

---

### **2.2 MVC Controller (with Views)**

```csharp
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger) => _logger = logger;

    public IActionResult Index() => View(); // returns Views/Home/Index.cshtml

    public IActionResult Privacy() => View();
}
```

- Returns Razor views instead of JSON.
    
- Typically used in **server-side rendered apps**.
    

---

### **2.3 Dependency Injection in Controllers**

```csharp
public interface IStudentService { IEnumerable<Student> GetAll(); }
public class StudentService : IStudentService
{
    private static List<Student> students = new List<Student> { new Student { Id=1, Name="Alice" } };
    public IEnumerable<Student> GetAll() => students;
}

// Register service
builder.Services.AddScoped<IStudentService, StudentService>();

// Inject in controller
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    public StudentsController(IStudentService service) => _service = service;

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAll());
}
```

- Keeps **controller thin**; business logic lives in service.
---

## **3. Routing**

### **3.1 Convention-Based**

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
```

- `/Home/Index` → `HomeController.Index()`
    
- Defaults: controller = Home, action = Index, id = optional
    

### **3.2 Attribute Routing**

```csharp
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    [HttpGet("add")]
    public int Add(int x, int y) => x + y;
}
```

- URL: `/api/calculator/add?x=5&y=10`
    

---

## **4. Model Binding & Validation**

- **Model Binding** maps HTTP request data to controller parameters.
    
- Supports:
    
    - Route parameters: `[HttpGet("{id}")]`
        
    - Query string: `?x=5&y=10`
        
    - JSON body: `[FromBody] Student s`
        
    - Form data: `[FromForm]`
        
    - Headers: `[FromHeader]`
        
- Validation occurs automatically for `[ApiController]` + data annotations.
    

---

## **5. Filters**

- **AuthorizationFilter** – Check user permissions
    
- **ActionFilter** – Pre/post processing
    
- **ExceptionFilter** – Catch exceptions and format response
    
- **ResultFilter** – Modify result before sending to client
    

**Example Action Filter:**

```csharp
public class LogActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) => Console.WriteLine("Before action");
    public void OnActionExecuted(ActionExecutedContext context) => Console.WriteLine("After action");
}
```

---

## **6. Error Handling**

- Use **`ProblemDetails`** for standard JSON error format:
    

```csharp
[HttpGet("{id}")]
public IActionResult Get(int id)
{
    var student = students.FirstOrDefault(s => s.Id == id);
    if (student == null)
        return NotFound(new { message = "Student not found", code = 404 });

    return Ok(student);
}
```

- Global exception handling via middleware:
    

```csharp
app.UseExceptionHandler(a => a.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
    var result = JsonSerializer.Serialize(new { error = exception.Message });
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(result);
}));
```

---

## **7. Advanced Models & Relationships**

### **7.1 One-to-Many Example**

```csharp
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Course> Courses { get; set; } = new();
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }
}
```

- EF Core automatically maps relationships.
    
- Use **Include** to load related data:
    

```csharp
var student = dbContext.Students.Include(s => s.Courses).FirstOrDefault(s => s.Id == id);
```

---

### **7.2 DTOs for Nested Models**

```csharp
public class StudentDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<string> CourseTitles { get; set; }
}

// Mapping
var dto = students.Select(s => new StudentDTO
{
    Id = s.Id,
    Name = s.Name,
    CourseTitles = s.Courses.Select(c => c.Title).ToList()
});
```

---

## **8. Client Integration (React Example)**

```javascript
// Fetch all students
fetch("https://localhost:5001/api/students")
  .then(res => res.json())
  .then(data => console.log(data));

// Post new student
fetch("https://localhost:5001/api/students", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({ Id: 105, Name: "David", Age: 22, Email: "david@example.com" })
});
```

- Use **CORS middleware** in ASP.NET Core to allow React dev server:
    

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});
app.UseCors("AllowReact");
```

---

## **9. Summary**

- **Models:** Represent data; use validation & DTOs for API.
    
- **Controllers:** Handle HTTP requests; thin controllers + services.
    
- **Routing:** Convention-based or attribute-based; flexible for APIs.
    
- **Middleware:** Core to request pipeline; handles logging, error handling, auth.
    
- **Filters:** Pre/post-processing hooks for actions.
    
- **Client Integration:** React, WPF, or mobile apps call JSON endpoints.
    
- **Best Practices:** DTOs, proper status codes, stateless APIs, async programming.
    