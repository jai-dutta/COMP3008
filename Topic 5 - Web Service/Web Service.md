# Web Service

## Services Overview

**Definition**: Collections of related functionalities grouped together as standalone components.

**Key Characteristics**:

- Stand-alone entities (not incorporated into other services)
- Internal objects cannot cross service boundaries
- Other services can use your service
- Reduce network dependency through special design rules

**Problem**: Cross-platform/framework implementation is challenging with traditional RPC infrastructure.

---

## Web Services

**Definition**: Services using HTTP/S as the RPC infrastructure instead of proprietary protocols.

**Why Use Web Services?**

- Universal connection shared between all frameworks
- Easy to use compared to raw sockets
- Operates on zero-trust networks
- Works with unknown/uncontrolled clients

---

## Network Communication Options

### 1. Raw Sockets

**Pros**: Total control, works everywhere **Cons**: Massive development effort, no built-in security, must implement on every platform

### 2. Custom Protocol

**Pros**: Can include any needed extensions **Cons**: Must build and maintain across all platforms, ensure compatibility

### 3. Borrowing Existing Protocol (HTTP)

**Pros**: No development overhead, widespread implementation **Cons**: Limited by existing implementation capabilities

**Why HTTP Won**: Everywhere, freeform, supports extensions, easy to use

---

## Data Formats

### XML (1998)

```xml
<?xml version="1.0"?>
<student>
    <name>John Doe</name>
    <grade id="math">85</grade>
    <subjects>
        <subject>Mathematics</subject>
        <subject>Physics</subject>
    </subjects>
</student>
```

**Pros**:

- Structured with DOM navigation: `document["student"]["grade"]["math"]`
- First-class data type in web languages
- Foundation of most web service systems
- Built-in C# serialization support

**Cons**:

- Extremely verbose (lots of tags and attributes)
- Poor visual structure (whitespace doesn't matter)
- Complex parsing requirements

### JSON (2000s)

```json
{
    "student": {
        "name": "John Doe",
        "grade": {"math": 85},
        "subjects": ["Mathematics", "Physics"]
    }
}
```

**Pros**:

- Concise and readable
- Native to JavaScript/web applications
- Easy interpreter development
- Clear visual structure

**Cons**:

- No attributes (unlike XML)
- Lost semantic information
- No DOM navigation
- Must manually traverse data structure

**When to Choose**:

- XML: Human-readable documents, SOAP services, existing XML infrastructure
- JSON: Modern web applications, APIs, JavaScript environments

---

## Web Service Description

### WSDL (Web Service Description Language)

**Purpose**: Describes web service interfaces since the actual code is hidden on the server.

**Usage**: Primarily for SOAP-based services, similar to Interface Definition Language (IDL) but for web services.

**Example Structure**:

```xml
<definitions>
    <types><!-- Data type definitions --></types>
    <message><!-- Message formats --></message>
    <portType><!-- Operations --></portType>
    <binding><!-- Protocol binding --></binding>
</definitions>
```

### API Documentation

**Modern Alternative**: Human-readable documentation explaining:

- Input formats
- URI locations
- Expected outputs
- Usage examples

**Why APIs Over WSDL**: Designed for human programmers rather than automated processing.

---

## Architectural Approaches

### SOA (Service Oriented Architecture)

**Focus**: The service as a complete entity **Characteristics**:

- Single URI per service
- Point-to-point protocol view
- Full collection of objects/functions
- Uses SOAP protocol
- Not browser-friendly (requires application interpretation)

**Example**: `http://company.com/CalculatorService` (handles all calculator operations)

### ROA (Resource Oriented Architecture)

**Focus**: Individual resources rather than complete services **Characteristics**:

- Multiple URIs for different functions
- Web 2.0 approach
- Browser-compatible
- Designed for high interoperability
- Uses REST methodology

**Example**:

- `http://company.com/calculator/add`
- `http://company.com/calculator/divide`

---

## SOAP vs REST

### SOAP (Simple Object Access Protocol)

**Method**: XML-based RPC tunnel through HTTP **Process**:

1. XML message sent via POST
2. Response returned as SOAP/XML document

**Example Request**:

```xml
<soap:Envelope>
    <soap:Body>
        <AddNumbers>
            <a>5</a>
            <b>10</b>
        </AddNumbers>
    </soap:Body>
</soap:Envelope>
```

**Limitations**: Not browser-friendly, requires POST requests, XML complexity

### REST (Representational State Transfer)

**Philosophy**: Strategy/approach rather than strict protocol **Goal**: Better represent the web, be easily accessible

#### REST HTTP Methods

|Method|Purpose|Example|
|---|---|---|
|GET|Retrieve data|`GET /users/123` - Get user info|
|POST|Create new resource|`POST /users` - Create new user|
|PUT|Update/create resource|`PUT /users/123` - Update user 123|
|PATCH|Partial update|`PATCH /users/123` - Update specific fields|
|DELETE|Remove resource|`DELETE /users/123` - Delete user|

#### Minimum REST Implementation

- **GET**: Retrieve information
- **POST**: Create/update/replace data
- **DELETE**: Remove data (optional - can use POST to delete function)

#### REST as Hybrid Systems

**Advantage**: Can function as both webpage and web service

- Web pages use GET/POST naturally
- Forms handle state changes via POST
- Same system serves humans (HTML) and programs (JSON/XML)
