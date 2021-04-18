# domain-service-proto is a sandbox project to explore implementation patterns of a web service world

The center of this sandbox project is a minimal domain service providing CRUD operations to a single entity.
Starting there I'm exploring opinions and assumptions how to structure the code best to achieve non-functional goals like testability.
Also the service is surrounded by other software components like user interfaces or services.

**Design Goal 1: The controllers of the web service must have technical responsibilities only**

The web-controllers ony translate the http protocol to the call to the internal business service which implements the actual domain functions.
To avoid unnecessary mappings the business service receives its parameters and returns its responses in simple classes which can be uses as DTOs by the web controllers.

**Design Goal 2: The domain service provides client libraries implementing the service contract**

For ease of use the domain service project provides two client libraries which allow access to the service using http+json and Grpc.
Both clients implement the very same interface contract as the business service does.
To allow this the domain service contract is moved to a 'contract project' containing the interface and simple data classes which serve as DTOs where possible,
Grpc messages are mapped to these parameter classes.

**Design Goal 3: The implementation of the interface contract must show identical behavior in client and business service**

To enforce same behavior of the implementation of the domain service and the domain clients the tests share main portions of the code.

**Design Goal 4: Even with client libraries the behavior of the controllers should be conform with the http standard**

Since the client library hides the details how the HTTP protocol is used to communicate to the web service I won't exclude other HTTP client implementations.
These other clients expect the interface to follow HTTP standards like meaningful HTTP status codes etc.
To make sure the messaging between client and server still follows general HTTP rules a test project is introduced to validate this.

Also an OpenId description and a swagger UI is provided by the service.

**Design Goal 5: A powershell client should be available allowing to interact with the service for inspection or automation scenarios**

A powershell client is helpful in scenarios for test, maintenance and enterprise integration.
The powershell already provides commands to interact with web services like Invoke-RestMethod which will relay already to the standard conform HTTP behavior described in design goal 4.
This project will provide such a client including tests.

**Design Goal 6: Behavior of all components should be observable with logfile wich is easily queriable**

Logging done using structured logging which creates JSON formatted log files.
