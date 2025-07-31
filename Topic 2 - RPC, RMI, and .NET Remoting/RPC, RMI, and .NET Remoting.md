# RPC, RMI, and .NET Remoting

## Components
A component is a set of functions that work together.
It is also the smallest chunk of an application that can be moved to a remote server.
Essentially provides "a service" to other components.

Components are defined by the **interfaces** it exposes to external clients. This is very similar to how **objects** are defined by their exposed interfaces to other objects.

**Components & Objects** both:
- Define public interfaces for clioents
- Can inherit interfaces from other objects / components
- Encapsulate data
- Define a cohesive set of actions and variables that can fulfil a role in the application
**Differences**: 
- Objects
	- Technically specific
	- Are actually source code in the app.
	- Internal, only seen by other objects
	- Compile time linking
	- Implementation is specific and reusable
- Components
	- Architecturally specific
	- Independent of actual code
	- Exposed to other applications
	- Use run time linking
	- Implementation details are obtuse

**Components** are coarser-grained, self-contained units that encapsulate functionality and communicate through well-defined interfaces. They typically represent significant business capabilities or services. A component might be an entire web service, a database module, or a payment processing system. Components emphasize loose coupling and can be deployed, scaled, and maintained independently. They often communicate through messaging protocols, REST APIs, or other network-based interfaces. The component model aligns well with microservices architecture, where each component can be developed by different teams using different technologies.

**Objects** in distributed computing extend the traditional object-oriented programming model across network boundaries. Distributed objects appear as regular objects to client code but actually execute on remote machines. Technologies like CORBA, Java RMI, and .NET Remoting exemplify this approach. Objects are typically finer-grained than components, representing individual entities or small units of functionality. They maintain state and provide methods that can be invoked remotely, often with location transparency - the client doesn't need to know where the object physically resides.

## Components and RPC
Put these together and you have the basis for distributed computing.
This, however, introduces problems - network is not stable like RAM.
Consequences of spreading state of a program over network.

## Services
**Services** in distributed computing are network-accessible software units that provide specific business functionality through standardized interfaces (like REST APIs).

Key traits:

- **Autonomous**: Run independently, own their data
- **Stateless**: Each request is self-contained
- **Composable**: Multiple services work together to build applications
- **Technology-agnostic**: Can use different languages/frameworks

Examples: authentication service, payment service, inventory service.

Services communicate over networks rather than direct function calls, enabling independent development, deployment, and scaling. This is the foundation of microservices and SOA architectures.

## Distribution
**Java RMI (Remote Method Invocation)**

Java RMI enables distributed objects where clients invoke methods on remote Java objects as if they were local. The server object inherits from `UnicastRemoteObject` and clients use downloaded stub code.

Example:
```java
// Server interface
public interface Calculator extends Remote {
    int add(int a, int b) throws RemoteException;
}

// Server implementation
public class CalculatorImpl extends UnicastRemoteObject 
    implements Calculator {
    public int add(int a, int b) { return a + b; }
}

// Client usage
Calculator calc = (Calculator) Naming.lookup("//server/Calculator");
int result = calc.add(5, 3); // Remote method call
```

**CORBA (Common Object Request Broker Architecture)**

CORBA uses Object Request Brokers (ORBs) as middleware to enable cross-language, cross-platform distributed objects. Uses Interface Definition Language (IDL) for language-independent interfaces.

Example IDL:

```idl
interface Calculator {
    long add(in long a, in long b);
};
```

**WCF (Windows Communication Foundation)**
WCF is Microsoft's .NET framework for building service-oriented applications with contracts, bindings, and endpoints.

**Key Components:**

- **Service Contract**: Interface defining available operations
- **Operation Contract**: Individual service methods
- **Bindings**: Communication protocols (HTTP, TCP, etc.)
- **Endpoints**: Address + Contract + Binding

**Example:**

```csharp
// Service contract
[ServiceContract]
public interface ICalculator {
    [OperationContract]
    int Add(int a, int b);
}

// Service implementation
[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
public class Calculator : ICalculator {
    public int Add(int a, int b) { return a + b; }
}

// Client using ChannelFactory
var binding = new NetTcpBinding();
var factory = new ChannelFactory<ICalculator>(binding, "net.tcp://localhost:8100/calc");
var proxy = factory.CreateChannel();
int result = proxy.Add(5, 3);
```

**Advantages:**

- Multiple transport protocols (HTTP, TCP, named pipes)
- Built-in security and transactions
- Interoperable with non-.NET services
- Flexible configuration

**Limitations:**

- .NET ecosystem only
- Complex configuration
- Microsoft-specific technology

WCF succeeded where CORBA failed by providing simpler configuration while maintaining enterprise features.
## Architecture Components

- **Database library** (shared code/DLL)
- **Server** (hosts the service)
- **Client** (consumes the service)

## Example: Student Information System

### 1. Dynamic Link Library (DLL)

**What is a DLL?**

- Dynamic Link Library - stores code/data for multiple programs
- **Code Reusability**: Multiple apps use same DLL functions
- **Dynamic Linking**: Links at runtime, not compile time
- **Memory Efficiency**: Loaded once, shared by multiple processes

**Building the DLL:**

csharp

```csharp
// Student class and Person class go in the DLL
// This becomes "StudentDLL" project
```

### 2. Building the Server (Console Application)

**Setup:**

- Console application project
- Add reference to "StudentDLL"
- Add reference to `System.ServiceModel` (from Assemblies)
- Create `StudentList` as database

**Service Contract:**

csharp

```csharp
[ServiceContract]
public interface DatabaseInterface {
    [OperationContract]
    void GetValuesForEntry(int index, out string name, out int id, out string university);
    
    [OperationContract]
    int GetNumEntries();
}
```

**Key Points:**

- `[ServiceContract]` defines the interface exposed to clients
- `[OperationContract]` marks individual service methods
- Note the "out" keyword for returning multiple values

**Service Implementation:**

csharp

```csharp
[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
public class DatabaseInterfaceImpl : DatabaseInterface {
    // Implementation of the contract methods
}
```

### 3. Service Behavior Configuration

**ServiceBehavior Attribute Options:**

**ConcurrencyMode:**

- `Single`: One request at a time (no concurrency)
- `Multiple`: Multiple requests concurrently using multiple threads

**IncludeExceptionDetailInFaults:**

- `true`: Include exception details in fault messages to client

**UseSynchronizationContext:**

- `true`: Enable synchronization with current SynchronizationContext (for UI apps)