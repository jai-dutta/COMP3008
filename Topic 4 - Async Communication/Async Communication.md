
## 1. Introduction to Asynchronous Communication

### Key Concepts

**Synchronous vs Asynchronous Calls:**

- **Synchronous (Blocking)**: Caller waits until function completes and returns
- **Asynchronous (Non-blocking)**: Control returns immediately to caller, function runs in background

**Why Use Asynchronous Calls?**

- Maintain GUI responsiveness
- Utilize caller resources more efficiently
- Continue other work during long-running operations
- Better resource management in distributed systems

### When to Use Asynchronous Calls

- Expected long-running operations (heavy processing, disk I/O)
- GUI applications requiring responsiveness
- **Avoid** making every RPC call async (increases complexity unnecessarily)

## 2. .NET Delegates

### What are Delegates?

Delegates are like function pointers in C/C++ - they allow you to:

- Define a function prototype
- Create variables that point to functions
- Pass functions as parameters

### Basic Delegate Example

```csharp
// Define the Calculator class
namespace MyExample {
    public class Calculator {
        public int Add(int num1, int num2) { return num1 + num2; }
        public int Subtract(int num1, int num2) { return num1 - num2; }
        public int Multiply(int num1, int num2) { return num1 * num2; }
    }
}

// Define delegate type
public delegate int BinaryOperation(int operand1, int operand2);

// Use delegates
public class DelegateTester {
    public static void Main() {
        BinaryOperation binOp;          // Declare delegate variable
        Calculator calc = new Calculator();
        
        binOp = calc.Add;               // Point to Add method
        Console.WriteLine("1 + 2 = " + binOp(1, 2));  // Output: 3
        
        binOp = calc.Subtract;          // Point to Subtract method
        Console.WriteLine("1 - 2 = " + binOp(1, 2));  // Output: -1
    }
}
```

## 3. Asynchronous Call Methods in .NET

### 3.1 Blocking/Polling Method

**Key Methods:**

- `BeginInvoke()`: Starts asynchronous call on new thread
- `EndInvoke()`: Retrieves results (blocks if not complete)
- `IsCompleted`: Check if call completed (for polling)

**Worked Example: Parallel Processing**

```csharp
public delegate int BinaryOperation(int operand1, int operand2);

public class CalcClient {
    public static void Main() {
        BinaryOperation addDel;
        Calculator calc = new Calculator();
        IAsyncResult asyncObj1, asyncObj2;
        int result1, result2;
        
        addDel = calc.Add;
        
        // Start two parallel async calls
        asyncObj1 = addDel.BeginInvoke(1, 2, null, null);
        asyncObj2 = addDel.BeginInvoke(3, 4, null, null);
        
        // Block waiting for results
        result1 = addDel.EndInvoke(asyncObj1);  // Gets 3
        result2 = addDel.EndInvoke(asyncObj2);  // Gets 7
        
        // Clean up
        asyncObj1.AsyncWaitHandle.Close();
        asyncObj2.AsyncWaitHandle.Close();
        
        Console.WriteLine($"1 + 2 = {result1}");
        Console.WriteLine($"3 + 4 = {result2}");
    }
}
```

**Use Cases for Blocking/Polling:**

- Fire off multiple calls, wait for all to complete
- Simple parallel task execution
- No need for complex thread synchronization

### 3.2 Completion Callbacks Method

**Concept:** Define a callback function that .NET calls when async operation completes.

**Thread Flow:**

1. Main thread calls `BeginInvoke()` with callback delegate
2. Worker thread starts executing the function
3. Main thread continues other work
4. When function completes, .NET calls callback on worker thread
5. Use `EndInvoke()` in callback to get results

**Worked Example: Async with Callback**

```csharp
public delegate int BinaryOperation(int operand1, int operand2);

public class CalcClient {
    public static void Main() {
        BinaryOperation addDel;
        AsyncCallback callbackDel;
        Calculator calc = new Calculator();
        
        addDel = calc.Add;
        callbackDel = OnAddCompletion;  // Point to callback method
        
        // Start async call with callback
        addDel.BeginInvoke(1, 2, callbackDel, null);
        
        Console.WriteLine("Waiting for completion...");
        Console.ReadLine();  // Keep main thread alive
    }
    
    // Callback method - runs on worker thread
    private static void OnAddCompletion(IAsyncResult asyncResult) {
        int result;
        BinaryOperation addDel;
        AsyncResult asyncObj = (AsyncResult)asyncResult;
        
        if (asyncObj.EndInvokeCalled == false) {
            addDel = (BinaryOperation)asyncObj.AsyncDelegate;
            result = addDel.EndInvoke(asyncObj);
            Console.WriteLine($"1 + 2 = {result}");
        }
        
        asyncObj.AsyncWaitHandle.Close();
    }
}
```

### Comparison: Blocking vs Completion Callbacks

|Aspect|Blocking/Polling|Completion Callbacks|
|---|---|---|
|**Advantages**|Simple, no callback function needed, .NET handles synchronization|Immediate notification, no wasted CPU cycles|
|**Disadvantages**|May block calling thread, polling wastes CPU|Complex, requires separate callback function, manual thread sync needed|
|**Best for**|Parallel task execution where you wait for all|Long-running tasks with immediate notification needs|

## 4. Thread Safety and Synchronization

### The Thread Safety Problem

**Race Conditions occur when:**

- Multiple threads access shared data simultaneously
- One thread's changes can overwrite another's
- Data corruption, lost updates, inconsistent state

### Thread Safety Example

```csharp
public class CalculatorEx : Calculator {
    private int m_iNumOperationsCompleted;
    
    public CalculatorEx() {
        m_iNumOperationsCompleted = 0;
    }
    
    // Read-only: no synchronization needed
    public int GetNumOperationsCompleted() {
        return m_iNumOperationsCompleted;
    }
    
    // Synchronized method - only one thread can execute at a time
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int Add(int operand1, int operand2) {
        m_iNumOperationsCompleted++;  // Safe to modify shared data
        return operand1 + operand2;
    }
    
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int Subtract(int operand1, int operand2) {
        m_iNumOperationsCompleted++;  // Safe to modify shared data
        return operand1 - operand2;
    }
}
```

### Manual Synchronization with Lock

```csharp
public int Add(int operand1, int operand2) {
    lock(this) {  // Lock entire object
        m_iNumOperationsCompleted++;
    }
    return operand1 + operand2;
}
```

### Thread Safety Best Practices

1. **Automatic vs Manual Synchronization**
    
    - Automatic: Easy but can be inefficient
    - Manual: More control but error-prone
2. **Guidelines:**
    
    - Never underestimate thread complexity
    - Minimize shared variables
    - Better safe than sorry - err on side of inefficiency
    - Don't synchronize everything (causes bottlenecks)
    - Lock read-only accesses usually not needed

## 5. Oneway Calls

### Concept

- Fire off message to remote end without waiting for response
- Client blocks only until server ACKs the call started
- Much simpler than full async calls
- No return values allowed

### Oneway Call Example

```csharp
[ServiceContract]
public interface INotificationService {
    [OperationContract(IsOneWay=true)]
    void SendNotification(string message);
    
    [OperationContract(IsOneWay=true)]
    void LogEvent(string eventData);
}
```

**Oneway vs Async Comparison:**

- **Oneway**: Blocks until server ACKs, then returns immediately
- **Async**: Returns before RPC call is even made, uses separate thread

## 6. Remote Callbacks

### Concept

Server calls back to client during long-running operations. Common uses:

- Progress updates
- Publisher/subscriber systems
- Peer-to-peer applications

### WCF Duplex Channels

**Server-Side Implementation:**

```csharp
// Define callback interface
[ServiceContract]
public interface IServerCallback {
    [OperationContract(IsOneWay=true)]
    void ProgressUpdate(int iteration);
}

// Define server interface with callback
[ServiceContract(CallbackContract=typeof(IServerCallback))]
public interface IServer {
    [OperationContract]
    void LongRunningJob();
}

// Server implementation
[ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple,
                UseSynchronizationContext=false)]
internal class ServerImpl : IServer {
    public void LongRunningJob() {
        // Get callback reference
        IServerCallback callback = OperationContext.Current
            .GetCallbackChannel<IServerCallback>();
        
        // Simulate long-running work with progress updates
        int iteration = 0;
        while (DoProcessing()) {
            iteration++;
            callback.ProgressUpdate(iteration);  // Callback to client
        }
    }
    
    private bool DoProcessing() {
        // Simulate work
        Thread.Sleep(1000);
        return iteration < 10;  // Stop after 10 iterations
    }
}
```

**Client-Side Implementation:**

```csharp
// Callback implementation
[CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Multiple,
                 UseSynchronizationContext=false)]
internal class ClientObj : IServerCallback {
    public void ConnectToServer() {
        DuplexChannelFactory<IServer> serverFactory;
        IServer theServer;
        NetTcpBinding tcpBinding = new NetTcpBinding();
        string url = "net.tcp://localhost:8005/Server";
        
        // Create duplex channel with callback handler
        serverFactory = new DuplexChannelFactory<IServer>(
            this,           // Callback handler
            tcpBinding, 
            url
        );
        
        theServer = serverFactory.CreateChannel();
        theServer.LongRunningJob();  // Will trigger callbacks
    }
    
    // Callback method - called by server
    public void ProgressUpdate(int iteration) {
        Console.WriteLine($"Progress: Iteration {iteration}");
        // Update progress bar, etc.
    }
}

// Main client
public class TheClient {
    public static void Main() {
        ClientObj client = new ClientObj();
        client.ConnectToServer();
    }
}
```

### Remote Callback Flow

```
1. Client calls server method
2. Server gets callback reference
3. Server performs work
4. Server calls callback method (runs on new client thread)
5. Client callback method executes
6. Server method completes and returns
```

### Remote vs Completion Callbacks

|Type|Purpose|When Called|Thread|
|---|---|---|---|
|**Remote Callback**|Server notifies client during execution|During server method execution|New thread on client|
|**Completion Callback**|.NET notifies when async call finishes|After async method completes|Worker thread|

## 7. GUI Thread Safety

### Cross-Thread Method Invocation

**Problem:** GUI controls must be updated only on the GUI event thread.

**Solution:** Use `Dispatcher.Invoke()` to marshal calls to GUI thread.

### Traditional Approach

```csharp
partial public class CalculatorGUI : Window {
    protected delegate void SetStatusDelg(string text);
    
    void SetStatus(string text) {
        ((StatusBarItem)stbStatus.Items[0]).Content = text;
    }
    
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void SetMessage(string msg) {
        SetStatusDelg setSts = SetStatus;
        object[] prms = new object[1];
        prms[0] = msg;
        
        // Marshal to GUI thread
        Application.Current.Dispatcher.Invoke(setSts, prms);
    }
}
```

### Modern Approach with Anonymous Methods

```csharp
partial public class CalculatorGUI : Window {
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void SetMessage(string msg) {
        Action action = delegate() {
            ((StatusBarItem)stbStatus.Items[0]).Content = msg;  // Uses closure
        };
        Application.Current.Dispatcher.Invoke(action);
    }
}
```

### Lambda Expression Approach

```csharp
partial public class CalculatorGUI : Window {
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void SetMessage(string msg) {
        Action action = () => {
            ((StatusBarItem)stbStatus.Items[0]).Content = msg;  // Uses closure
        };
        Application.Current.Dispatcher.Invoke(action);
    }
}
```

## 8. Advanced Concepts

### Built-in .NET Delegate Types

```csharp
// No parameters, no return value
Action action = () => Console.WriteLine("Hello");

// One parameter, no return value  
Action<string> actionWithParam = (msg) => Console.WriteLine(msg);

// Two parameters, no return value
Action<string, int> actionWithTwoParams = (msg, num) => 
    Console.WriteLine($"{msg}: {num}");

// No parameters, with return value
Func<int> funcWithReturn = () => 42;

// One parameter, with return value
Func<int, string> funcWithParamAndReturn = (num) => $"Number: {num}";
```

### Closures

Closures allow inner functions to access variables from outer scope:

```csharp
public void ExampleClosure() {
    string outerVariable = "Hello from outer scope";
    int counter = 0;
    
    Action innerAction = () => {
        Console.WriteLine(outerVariable);  // Accesses outer variable
        counter++;                         // Modifies outer variable
        Console.WriteLine($"Counter: {counter}");
    };
    
    innerAction();  // Prints: "Hello from outer scope" and "Counter: 1"
    innerAction();  // Prints: "Hello from outer scope" and "Counter: 2"
}
```

## 9. Complete Working Example: Async Calculator with Progress

Here's a comprehensive example combining multiple concepts:

```csharp
// Service contracts
[ServiceContract]
public interface ICalculatorCallback {
    [OperationContract(IsOneWay=true)]
    void ProgressUpdate(string message, int percentage);
}

[ServiceContract(CallbackContract=typeof(ICalculatorCallback))]
public interface IAsyncCalculator {
    [OperationContract]
    void PerformLongCalculation(int iterations);
}

// Server implementation
[ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple)]
public class AsyncCalculatorService : IAsyncCalculator {
    public void PerformLongCalculation(int iterations) {
        var callback = OperationContext.Current
            .GetCallbackChannel<ICalculatorCallback>();
        
        for (int i = 1; i <= iterations; i++) {
            // Simulate work
            Thread.Sleep(100);
            
            // Calculate progress
            int percentage = (i * 100) / iterations;
            
            // Update client
            callback.ProgressUpdate($"Processing iteration {i}", percentage);
        }
    }
}

// Client implementation
[CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Multiple)]
public partial class CalculatorClient : Window, ICalculatorCallback {
    private DuplexChannelFactory<IAsyncCalculator> factory;
    private IAsyncCalculator calculator;
    
    public CalculatorClient() {
        InitializeComponent();
        ConnectToService();
    }
    
    private void ConnectToService() {
        var binding = new NetTcpBinding();
        var endpoint = new EndpointAddress("net.tcp://localhost:8080/Calculator");
        
        factory = new DuplexChannelFactory<IAsyncCalculator>(this, binding, endpoint);
        calculator = factory.CreateChannel();
    }
    
    private void StartButton_Click(object sender, RoutedEventArgs e) {
        // Start async calculation (non-blocking)
        Task.Run(() => calculator.PerformLongCalculation(50));
    }
    
    // Callback method - called by server
    public void ProgressUpdate(string message, int percentage) {
        // Marshal to GUI thread using lambda
        Dispatcher.Invoke(() => {
            statusLabel.Content = message;
            progressBar.Value = percentage;
        });
    }
}
```

## Summary

Asynchronous communication in distributed computing provides:

1. **Better Resource Utilization**: Non-blocking calls allow continued processing
2. **Improved Responsiveness**: Especially important for GUI applications
3. **Flexible Communication Patterns**: Callbacks enable server-to-client communication
4. **Scalability**: Systems can handle more concurrent operations

**Key Takeaways:**

- Choose the right async pattern for your needs
- Always consider thread safety when using shared data
- Use appropriate synchronization mechanisms
- Be cautious with thread complexity - it's easy to introduce bugs
- Leverage .NET's built-in async capabilities rather than manual threading