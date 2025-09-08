# Essential Guide to Asynchronous Communication in .NET

## 1. Core Concepts

**Synchronous vs Asynchronous:**

- **Synchronous**: Thread waits until operation completes
- **Asynchronous**: Control returns immediately, operation runs concurrently

**When to Use Async:**

- I/O operations (file, network, database)
- Long-running operations
- GUI applications requiring responsiveness
- Operations involving waiting

## 2. Modern Async/Await with Tasks (Recommended)

### Basic Task Examples

```csharp
public class ModernAsyncCalculator
{
    // Async method returning a value
    public async Task<int> AddAsync(int a, int b)
    {
        await Task.Delay(1000); // Simulate async work
        return a + b;
    }
    
    // Async method with cancellation
    public async Task<string> ProcessDataAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(2000, cancellationToken);
        return "Processing complete";
    }
    
    // Parallel execution
    public async Task<int[]> CalculateParallelAsync()
    {
        Task<int> task1 = AddAsync(1, 2);
        Task<int> task2 = AddAsync(3, 4);
        
        return await Task.WhenAll(task1, task2); // [3, 7]
    }
}
```

### Progress Reporting with Tasks

```csharp
public class ProgressService
{
    public async Task ProcessWithProgressAsync(
        IProgress<string> progress = null,
        CancellationToken cancellationToken = default)
    {
        for (int i = 1; i <= 10; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await Task.Delay(500, cancellationToken);
            progress?.Report($"Step {i}/10 completed");
        }
    }
}

// Usage
public async Task Example()
{
    var progress = new Progress<string>(message => Console.WriteLine(message));
    var service = new ProgressService();
    
    await service.ProcessWithProgressAsync(progress);
}
```

### GUI Thread Safety with Tasks

```csharp
public partial class MainWindow : Window
{
    private async void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            loadButton.IsEnabled = false;
            
            // Automatically returns to UI thread after await
            string data = await FetchDataAsync();
            
            // Safe to update UI here
            dataTextBox.Text = data;
            statusLabel.Content = "Loaded successfully";
        }
        catch (Exception ex)
        {
            statusLabel.Content = $"Error: {ex.Message}";
        }
        finally
        {
            loadButton.IsEnabled = true;
        }
    }
    
    private async Task<string> FetchDataAsync()
    {
        using var httpClient = new HttpClient();
        return await httpClient.GetStringAsync("https://api.example.com/data");
    }
}
```

## 3. Delegate-Based Async Pattern (Legacy)

### Basic Delegates

```csharp
public delegate int BinaryOperation(int operand1, int operand2);

public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
}

public class DelegateExample
{
    public static void BasicUsage()
    {
        var calculator = new Calculator();
        BinaryOperation operation = calculator.Add;
        
        int result = operation(5, 3); // Calls Add method
        Console.WriteLine($"Result: {result}"); // Output: 8
    }
    
    public static void AsyncDelegateUsage()
    {
        var calculator = new Calculator();
        BinaryOperation operation = calculator.Add;
        
        // Start async operation
        IAsyncResult asyncResult = operation.BeginInvoke(1, 2, null, null);
        
        // Do other work
        Console.WriteLine("Calculation started...");
        
        // Get result (blocks until complete)
        int result = operation.EndInvoke(asyncResult);
        Console.WriteLine($"Result: {result}");
        
        asyncResult.AsyncWaitHandle.Close();
    }
}
```

### Async Delegates with Callbacks

```csharp
public class CallbackExample
{
    public static void AsyncWithCallback()
    {
        var calculator = new Calculator();
        BinaryOperation operation = calculator.Add;
        
        // Start async with callback
        operation.BeginInvoke(5, 10, OnCalculationComplete, operation);
        
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
    
    private static void OnCalculationComplete(IAsyncResult asyncResult)
    {
        var operation = (BinaryOperation)asyncResult.AsyncState;
        int result = operation.EndInvoke(asyncResult);
        
        Console.WriteLine($"Callback result: {result}");
        asyncResult.AsyncWaitHandle.Close();
    }
}
```

### Modern Delegate Types (Action/Func)

```csharp
public class ModernDelegates
{
    public static void Examples()
    {
        // Action - no return value
        Action<string> logMessage = message => Console.WriteLine($"Log: {message}");
        logMessage("Hello World");
        
        // Func - with return value
        Func<int, int, int> add = (a, b) => a + b;
        int result = add(3, 4);
        
        // Async Action/Func
        Func<Task<string>> fetchDataAsync = async () =>
        {
            await Task.Delay(1000);
            return "Data fetched";
        };
        
        // Usage
        Task.Run(async () =>
        {
            string data = await fetchDataAsync();
            Console.WriteLine(data);
        });
    }
}
```

## 4. WCF Duplex Communication

### Service Contracts

```csharp
// Callback interface - server calls client
[ServiceContract]
public interface IServerCallback
{
    [OperationContract(IsOneWay = true)]
    void ProgressUpdate(string message, int percentage);
}

// Main service interface
[ServiceContract(CallbackContract = typeof(IServerCallback))]
public interface IAsyncCalculatorService
{
    [OperationContract]
    void StartLongCalculation(int iterations);
    
    [OperationContract(IsOneWay = true)]
    void CancelOperation();
}
```

### Server Implementation

```csharp
[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
public class AsyncCalculatorService : IAsyncCalculatorService
{
    private volatile bool _cancelled = false;
    
    public void StartLongCalculation(int iterations)
    {
        // Get callback channel to client
        var callback = OperationContext.Current
            .GetCallbackChannel<IServerCallback>();
        
        _cancelled = false;
        
        for (int i = 1; i <= iterations && !_cancelled; i++)
        {
            // Simulate work
            Thread.Sleep(200);
            
            // Calculate progress and notify client
            int percentage = (i * 100) / iterations;
            callback.ProgressUpdate($"Processing step {i}/{iterations}", percentage);
        }
        
        if (!_cancelled)
        {
            callback.ProgressUpdate("Calculation completed!", 100);
        }
    }
    
    public void CancelOperation()
    {
        _cancelled = true;
    }
}
```

### Client Implementation

```csharp
[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
public class CalculatorClient : IServerCallback
{
    private DuplexChannelFactory<IAsyncCalculatorService> _factory;
    private IAsyncCalculatorService _calculator;
    
    public void ConnectToService()
    {
        var binding = new NetTcpBinding();
        var endpoint = new EndpointAddress("net.tcp://localhost:8080/Calculator");
        
        _factory = new DuplexChannelFactory<IAsyncCalculatorService>(
            this, binding, endpoint);
        _calculator = _factory.CreateChannel();
    }
    
    public void StartCalculation()
    {
        Console.WriteLine("Starting calculation...");
        
        // This will trigger callbacks as work progresses
        Task.Run(() => _calculator.StartLongCalculation(20));
    }
    
    // Callback method - called by server
    public void ProgressUpdate(string message, int percentage)
    {
        Console.WriteLine($"[{percentage}%] {message}");
        
        // Update UI on proper thread if needed
        if (Application.Current != null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Update progress bar, labels, etc.
            });
        }
    }
    
    public void Disconnect()
    {
        _calculator?.CancelOperation();
        _factory?.Close();
    }
}
```

### Complete Duplex Example Usage

```csharp
public class DuplexExample
{
    public static void RunExample()
    {
        // Client setup
        var client = new CalculatorClient();
        client.ConnectToService();
        
        // Start long-running operation
        client.StartCalculation();
        
        // Keep client alive to receive callbacks
        Console.WriteLine("Press 'q' to quit or any key to cancel operation");
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.KeyChar == 'q')
                break;
            
            // Cancel current operation
            client.Disconnect();
            Console.WriteLine("Operation cancelled");
            break;
        }
        
        client.Disconnect();
    }
}
```

## 5. Thread Safety Essentials

### Thread-Safe Operations

```csharp
public class ThreadSafeCounter
{
    private int _count = 0;
    private readonly object _lock = new object();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    // Traditional synchronous lock
    public void IncrementSync()
    {
        lock (_lock)
        {
            _count++;
        }
    }
    
    // Async-compatible lock
    public async Task IncrementAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _count++;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    // Atomic operation - no lock needed
    public void IncrementAtomic()
    {
        Interlocked.Increment(ref _count);
    }
    
    public int GetCount() => _count;
}
```

## 6. Best Practices Summary

### Do's and Don'ts

```csharp
public class BestPractices
{
    // ✅ Good: Use async/await consistently
    public async Task<string> GoodAsync()
    {
        var result = await SomeAsyncOperation();
        return result.ToUpper();
    }
    
    // ❌ Bad: Don't block on async methods
    public string BadBlocking()
    {
        return SomeAsyncOperation().Result; // Can deadlock!
    }
    
    // ✅ Good: Use ConfigureAwait(false) in libraries
    public async Task<string> LibraryMethodAsync()
    {
        var result = await SomeAsyncOperation().ConfigureAwait(false);
        return result;
    }
    
    // ✅ Good: Return Task directly when just awaiting
    public Task<string> PassThroughAsync()
    {
        return SomeAsyncOperation(); // No async/await needed
    }
    
    // ✅ Good: Handle exceptions properly
    public async Task<string> SafeOperationAsync()
    {
        try
        {
            return await RiskyOperationAsync();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Network error: {ex.Message}");
            throw;
        }
    }
    
    private async Task<string> SomeAsyncOperation() => 
        await Task.FromResult("result");
    
    private async Task<string> RiskyOperationAsync() => 
        await Task.FromResult("risky result");
}
```

## Summary

**Modern Approach (Preferred):**

- Use `async/await` with `Task<T>` for all new code
- Automatic UI thread marshalling
- Built-in cancellation and progress support
- Exception handling with try/catch

**Delegates:**

- Function pointers for callback patterns
- Legacy `BeginInvoke/EndInvoke` for older codebases
- Modern `Action/Func` for simple callbacks

**Duplex Communication:**

- Two-way communication between client/server
- Server can call back to client during operations
- Essential for real-time updates and progress reporting
- Requires careful thread safety consideration

**Key Takeaways:**

- Prefer `async/await` over legacy patterns
- Use `CancellationToken` for cancellable operations
- Implement `IProgress<T>` for progress updates
- Never block on async methods with `.Result` or `.Wait()`
- Use appropriate synchronization for shared state