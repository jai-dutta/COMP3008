 # Overview
## Key Technologies and Setup

### Programming Environment

- **Primary Language**: C# with Visual Studio 2022
- **Rationale**: Microsoft has dominated distributed systems for ~20 years, making it an excellent learning foundation
- **Framework**: .NET Framework with Windows Presentation Foundation (WPF)
- **Comparison**: C# vs Java usage patterns outlined for different application types

### Development Demonstrations

- Console applications with object-oriented programming (Person/Student classes)
- WPF GUI applications with event handling
- Project integration between console and WPF applications
- Access modifiers, properties, and inheritance concepts

## Distributed Computing Fundamentals

### What is Distributed Computing?

- **Definition**: Splitting applications into multiple processes across multiple machines
- **Core Requirement**: Extensive inter-process communication (IPC)
- **Not Simply**: Basic client-server architecture
- **Key Principle**: Applications must perform useful work in multiple locations

### Historical Context

- **Problem**: Single processors have speed limitations (1990s context)
- **Solution**: Leverage multiple processors across different computers
- **Evolution**: From number crunching to internet-based applications
- **Modern Reality**: Most client-server applications are now inherently distributed

## Application Architectures

### Client-Server Applications

- **Characteristics**: "Dumb" clients accessing "smart" servers
- **Examples**: SSH, FTP, plain HTTP
- **Processing**: All work performed server-side

### Peer-to-Peer Applications

- **Characteristics**: No central servers, direct client communication
- **Examples**: BitTorrent, Mesh Wi-Fi networks
- **Limitation**: No coordinated, orchestrated work

### Distributed Applications

- **Flexibility**: Can be peer-to-peer (cryptocurrencies) or client-server (MMORPGs)
- **Key Feature**: Coordinated, singular application running across all member computers

### Browser-Based Applications

- **Traditional**: Browsers and basic HTML pages are not distributed
- **Modern Examples**: Online documents, webmail clients, online games, web crypto miners

## Remote Procedure Calls (RPC)

### Core Concept

- **Purpose**: Enable function calls that execute on remote machines
- **Transparency**: Should appear identical to local function calls
- **History**: Original concept from 1976 (RFC 707), first implemented by Xerox

### Popular RPC Frameworks

- gRPC (Google RPC)
- Apache Thrift
- JSON-RPC
- XML-RPC

### RPC Implementation Process

#### Interface Definition

- Create IDL (Interface Definition Language) files
- Specify functions, parameters, and return types
- Generate client and server code in multiple programming languages

#### Server Implementation

- Implement functions defined in IDL
- Listen for incoming RPC requests
- Execute functions and return results

#### Client Implementation

- Use generated client libraries with stub functions
- Make RPC requests to server
- Process returned results

### Data Handling

- **Serialization**: Convert complex data structures to transmittable format
- **Common Formats**: JSON, XML, Protocol Buffers, MessagePack
- **Deserialization**: Reconstruct original data structures
- **Marshaling**: Convert data between representations within same environment

### RPC Advantages and Disadvantages

#### Pros

- Transparent function call interface
- Reduces network programming complexity
- Familiar programming model

#### Cons

- **Performance**: Much slower due to network latency
- **Reliability**: Can fail due to network issues
- **Debugging**: Difficult to trace execution state
- **Hidden Complexity**: Looks like local calls but behaves differently
- **Network Dependencies**: Memory references impossible over networks

### Important Considerations

- **Security**: Use secure channels (HTTPS), implement authentication/authorization
- **Testing**: Thorough testing and debugging essential
- **Scaling**: Consider load balancing and scaling strategies
- **Error Handling**: Graceful handling of network failures and server unavailability

## Study Resources

- W3Schools C# Tutorial
- Microsoft Learn .NET/C# Documentation
- .NET Learning Platform

## Next Topics

- Components
- .NET Remoting
- Java RMI
- Service Oriented Architectures