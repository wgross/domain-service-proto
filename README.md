# domain-service-proto is a sandbox project to explore implementation patterns of a web service world

The center of this sandbox project is a minimal domain service providing CRUD operations to a single entity. 
With starting there I'm exploring opinions and assumptions how to structure the code best to achieve non-functional goals like testability.
Also the service is surrounded by other software components like user interfaces or services.

## Opinion 1: The controllers of the web service must have technical responsibilities only

The web-controllers ony translate the http protoicol to the call to the internal business service which implements the actual domain functions.
To avoid unnecessary mappings the business service receives its parameters and returns its responses in simple classes which can be uses as DTOs by the web controllers. 
Theses class are moved to a shared project which provides an intercace contract which is referenced by the web-controllers and implemented by the business service.

## Opinion 2: The domain service provides client libraries following a service contract

For ease of use the domain service project provides two client libraries which allow access to the service using http+json and Grpc. 
Both clients implement the very same interface contract as the business service does. 

## Opinion 3: The implementation of the interfac contract must show identical behavor in cluent and business service

To achieve this goal the test cases for clients an business service share the code of the Act and Assert phases of the tests. 
They might differ in the Arrange phase deoedning on the test subject.

## Opinion 4: Even with client libraries the behavior of the controllers should be conform with the http standard

An additional controller test which isn't using the client libraries but the controller directly ensures that public behaviour of the controllers doesn't surprise a client which is not using the provided client libraries.
This affects the internal working of the Json clien especiallyt. Instead of just relying on a propriatrary contract for status reporting it is required that the usual http status codes are used by the controller to communicate properly.

Also an OpenId description and a swagger UI is provided by the service.

## Opinion 5: A powershell client should be available allowing to interact with the service for inspection or automation scenarios

It is easy to achieve that with any web service by implementation a script module using the Invoke-RestMethod commandlet. 
But supporting more technically complex calling scenarios (ex. event streaming) requires some custom implementation in C#. 

## Opinon 6: Behavior of all components should be observable with logfile wich is easily queriable

=> log entries are compressed JSON, one entry in each line of the logfile.
