# Multi-tiered architectures

## Three-tiered model
An old standard, reasonable starting place although outdated.
Consists of: 
1. Data tier - Database
2. Business tier - Logic server
3. Presentation tier - WCF Client

This helps with:
- Modularisation
	- Each tier exposes one interface and uses one interface, which means low coupling
	- Can even have multiple tier implementations
- Better security
	- Limits connection between tiers
	- Hide functionality you don't want users to have access to

### Data tier
- Mostly just a database. (interface to a db, sometimes just db but not often)
- Purpose is to serve data to the business tier.
- Tends to be a singleton (avoid race conditions)
- Doesn't consider **how** data is used, just provides interface for business tier.
```cs
public class Student
{
	public int Id {get; set;}
	public string Name {get; set;}
	...
}

public interface StudentDatabase
{
	IEnumerable<Student> GetAllStudents();
	Student GetStudentById(int id);
	...
}
```

### Business tier
- A tier that does the "business logic"
- If it's not directly related to data, and its not related to presentation, it belongs here.
	- Logins
	- Game world management
	- Data processing
- Business tier can be distributed
	- Allows for load balancing of applications
	- Can allow for specialisation of application back ends
		- Have one business tier service that handles one specific service
		- Can even use a multi-level business tier to route clients to the requisite machines
	- Can be used to act as a uniform platform access to multiple underlying systems

```cs
public class StudentManager
{
	private StudentDatabase studentDatabase;

	public StudentManager(StudentDatabase sdb)
	{
		this.studentDatabase = sdb;
	}
	public IEnumerable<Student> GetAllStudents()
	{
		// perofrm logic
		return studentDatabase.GetAllStudents();
	}

	public Student GetStudentById(int ID) 
	{
		// perform some checks
		return studentDatabase.GetStudentById(id);
	}
	...
	
}
```

### Presentation Tier
- **Inherently distributed**
	- Every **user runs their own instance** of the Presentation Tier
	- Each one connects to the **same backend**
	- Therefore, the Presentation Tier exists on **many separate devices**, naturally creating a **distributed system**

- **Can be multiple programs**
	- Use a standard API on your Biz tier and its easy

	You can have **multiple different applications** (or frontends) acting as presentation tiers, all interacting with the **same business logic**.
	
	#### ✅ Example:
	
	- A **web app** in a browser
	    
	- A **mobile app** on iOS/Android
	    
	- A **desktop app** (e.g., WPF)
	    
	- A **CLI tool** for power users
	    
	#### 🔧 How does it work?
	
	If your **Business Tier exposes a standard API** (e.g., REST, gRPC, WCF), then **any Presentation Tier can call it**, no matter what it is built in.
	
> 	🔁 **Reusability:** This is the power of abstraction — build your business logic once, reuse it everywhere.

- **Could theoretically have same user on multiple devices**
	- Just hide everything by running one object per user at the business tier
	
	To manage this "multi-device-per-user" scenario, the Business Tier can:
	
	- Create and manage a **user-specific object or session**
	    
	- Route all device requests through that **single in-memory object**
	    
	- Maintain **consistent state** and logic for that user
	    
	
	#### 🧠 Why?
	
	This hides the complexity of multiple devices — from the Business Tier's perspective, you're just "one user," even if you're using 3 devices.
	
	### 🛠️ Example in Practice:
	
	```cs
// Business Tier pseudo-code
class UserSessionManager {
    private Dictionary<string, UserSession> _sessions;

    public UserSession GetSession(string userId) {
        if (!_sessions.ContainsKey(userId)) {
            _sessions[userId] = new UserSession(userId);
        }
        return _sessions[userId];
    }
}
```
	
	- No matter how many devices make calls to the backend using the same user token, they’re all mapped to **one object** in memory.
	    
	- This allows state (e.g. shopping cart, game state, etc.) to remain **consistent** across devices.

## Four-tiered model
The fourth tier is the Display tier, which calls on the Presentation tier interface, e.g. a RESTful GET is passed to presentation tier, which returns JSON or XML, which the display tier formats for browser. The display tier is the front-end tier that talks to presentation tier.

|Tier|Responsibility|Example Technology|
|---|---|---|
|**Display Tier**|Handles _rendering_ and _user interaction_|HTML/CSS/JavaScript, React|
|**Presentation Tier**|Provides an _API interface_ to business logic|RESTful API (Web API, WCF), gRPC|
|**Business Tier**|Handles _core logic_ and workflows|C#, Java services, .NET class libs|
|**Data Tier**|Manages _data storage and retrieval_|SQL Server, MongoDB, EF Core|

# Asynchronicity in Distributed Applications

## Overview

Asynchronicity is crucial in distributed applications to prevent blocking and improve performance. When dealing with remote calls, network latency, and multiple concurrent operations, proper asynchronous handling ensures responsive applications.

## Ways of Handling Blocking in Distributed Applications

There are three primary approaches to handle blocking in distributed systems:

1. **Don't Block** (OneWay connections)
2. **Block** (might be useful under some conditions)
3. **Block in Another Thread** (asynchronous execution)

## One Way Calls

### Definition

One Way calls are the simplest method for implementing non-blocking remote calls. They allow fire-and-forget operations where the client doesn't wait for a response.

### Characteristics

- **No Return Value**: Must be void type
- **No Output Parameters**: Cannot have `out` mode parameters
- **Fire and Forget**: Client sends data but doesn't wait for response

### Implementation



```csharp
[OperationContract(IsOneWay=true)]
void ProcessData(string data);
```

### Example Implementation



```csharp
[ServiceContract]
public interface IDataService
{
    // One-way operation - no response expected
    [OperationContract(IsOneWay = true)]
    void ProcessData(string data);
    
    // Standard request-response operation
    [OperationContract]
    string GetData(int id);
}

public class DataService : IDataService
{
    public void ProcessData(string data)
    {
        // Process data without returning anything
        Console.WriteLine($"Processing: {data}");
        // Perform background processing
    }
    
    public string GetData(int id)
    {
        return $"Data for ID: {id}";
    }
}

// Client usage
var client = new DataServiceClient();
client.ProcessData("Important data"); // Non-blocking call
// Client continues immediately without waiting
```

### Important Considerations

**Connection Still Required**:

- One Way calls still need to establish connection
- Can still timeout during connection establishment
- May still block briefly during initial connection

**No Feedback**:

- No knowledge of operation success/failure
- No return values or confirmation
- Must implement alternative feedback mechanisms if needed

## Threads - A Recap

### Purpose

Threads enable concurrent execution within a single application:

- **Shared Memory**: All threads share the same memory space
- **Independent Execution**: Each thread can execute independently
- **Concurrency**: Multiple operations can appear to run simultaneously

### Benefits

- Improved responsiveness
- Better resource utilization
- Parallel processing capabilities

## Synchronous vs Asynchronous Method Execution

### Synchronous Execution


```csharp
public string ProcessData()
{
    // This blocks the calling thread
    var result = DatabaseQuery(); // Takes 5 seconds
    return result;
}
```

### Asynchronous Execution


```csharp
public async Task<string> ProcessDataAsync()
{
    // This doesn't block the calling thread
    var result = await DatabaseQueryAsync(); // Takes 5 seconds but doesn't block
    return result;
}
```

## C# Async/Await Pattern

### The `async` Keyword

The `async` keyword marks a method as asynchronous:

- Enables use of `await` within the method
- Method can contain one or more `await` expressions
- Allows asynchronous operations without blocking current thread



```csharp
public async Task<string> GetDataAsync()
{
    // Method marked as async
    var result = await SomeAsyncOperation();
    return result;
}
```

### The `Task` Class

`Task` represents an asynchronous operation:

- May or may not return a result
- Runs in background, freeing up calling thread
- Returns `Task` (no result) or `Task<TResult>` (with result)
- Provides completion, cancellation, and exception handling



```csharp
// Task without return value
public async Task ProcessAsync()
{
    await SomeOperation();
}

// Task with return value
public async Task<int> CalculateAsync()
{
    await SomeOperation();
    return 42;
}

// Using tasks
Task<int> task = CalculateAsync();
int result = await task; // or task.Result for blocking wait
```

### The `await` Keyword

`await` suspends method execution without blocking the thread:

- Waits for `Task` or `Task<TResult>` completion
- Suspends current method and returns control to caller
- Resumes execution after awaited task completes
- Re-throws exceptions from awaited tasks



```csharp
public async Task<string> ProcessFileAsync(string filePath)
{
    try
    {
        string content = await File.ReadAllTextAsync(filePath);
        // Method resumes here after file reading completes
        return content.ToUpper();
    }
    catch (Exception ex)
    {
        // Catches exceptions from the awaited operation
        Console.WriteLine($"Error reading file: {ex.Message}");
        return string.Empty;
    }
}
```

## Practical Examples

### Reading File Asynchronously


```csharp
public async Task<string> ReadFileAsync(string filePath)
{
    using (var reader = new StreamReader(filePath))
    {
        // await ensures non-blocking file read
        string content = await reader.ReadToEndAsync();
        return content;
    }
}

// Usage
public async Task ProcessFiles()
{
    string content = await ReadFileAsync("data.txt");
    // Main thread remains free during file reading
    Console.WriteLine($"File content length: {content.Length}");
}
```

### Parallel Execution Example



```csharp
public async Task<int> AddNumbersAsync(int a, int b)
{
    // Simulate time-consuming operation
    await Task.Delay(1000);
    return a + b;
}

public async Task ParallelAdditionExample()
{
    // Start multiple operations in parallel
    Task<int> task1 = AddNumbersAsync(5, 10);
    Task<int> task2 = AddNumbersAsync(15, 20);
    Task<int> task3 = AddNumbersAsync(25, 30);
    
    // Wait for all to complete
    int[] results = await Task.WhenAll(task1, task2, task3);
    
    Console.WriteLine($"Results: {results[0]}, {results[1]}, {results[2]}");
    // Output: Results: 15, 35, 55
}
```

### HTTP Client Async Example


```csharp
public async Task<string> FetchDataAsync(string url)
{
    using (var client = new HttpClient())
    {
        try
        {
            // Non-blocking HTTP request
            HttpResponseMessage response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
            return $"Error: {response.StatusCode}";
        }
        catch (HttpRequestException ex)
        {
            return $"Network error: {ex.Message}";
        }
    }
}
```

### Error Handling in Async Operations

```csharp
public async Task<string> RobustAsyncOperation()
{
    try
    {
        var task1 = FetchDataAsync("https://api.example.com/data1");
        var task2 = FetchDataAsync("https://api.example.com/data2");
        
        // Wait for first successful completion
        var completedTask = await Task.WhenAny(task1, task2);
        
        if (completedTask.IsCompletedSuccessfully)
        {
            return await completedTask;
        }
        
        throw new InvalidOperationException("All operations failed");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Operation failed: {ex.Message}");
        return "Fallback data";
    }
}
```

## Best Practices

1. **Use async/await consistently** throughout the call chain
2. **Avoid blocking** on async operations with `.Result` or `.Wait()`
3. **Configure await behavior** with `ConfigureAwait(false)` in library code
4. **Handle exceptions properly** in async methods
5. **Use CancellationToken** for long-running operations


```csharp
public async Task<string> BestPracticeExample(CancellationToken cancellationToken = default)
{
    using var client = new HttpClient();
    
    try
    {
        var response = await client.GetAsync("https://api.example.com", cancellationToken)
            .ConfigureAwait(false);
        
        return await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);
    }
    catch (OperationCanceledException)
    {
        return "Operation was cancelled";
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"HTTP error: {ex.Message}");
        throw;
    }
}
```

## Summary

Asynchronicity in distributed applications is essential for:

- **Performance**: Non-blocking operations improve responsiveness
- **Scalability**: Better resource utilization with async patterns
- **User Experience**: Applications remain responsive during long operations
- **Resource Management**: Threads aren't blocked waiting for I/O operations

The combination of One Way calls, proper threading, and C#'s async/await pattern provides powerful tools for building efficient distributed applications.