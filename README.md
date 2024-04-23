# .NET 8 Elasticsearch Class Library

This repository contains a .NET 8 Class Library for integrating Elasticsearch using the `Elastic.Clients.Elasticsearch` package. The library encapsulates various asynchronous repository methods to facilitate seamless interactions with Elasticsearch for complex querying, indexing, and document management tasks.

## Prerequisites

Before you start, ensure you have:
- An active Elasticsearch server.
- .NET 8 SDK installed on your development machine.

## Configuration

### Setting Up Dependency Injection

The library is designed to integrate with the .NET Core dependency injection framework. Add the Elasticsearch services to your application's service collection using the `AddElasticsearch` extension method, which is configured to utilize the `Elastic.Clients.Elasticsearch` package.

```csharp
// Inside your ConfigureServices method in Startup.cs or Program.cs
builder.Services.AddElasticsearch(builder.Configuration);
```
Make sure to configure the Elasticsearch client in the AddElasticsearch method properly by providing the connection settings and any specific configurations required by Elastic.Clients.Elasticsearch.

## Repository Methods
The class library includes a generic repository with the following asynchronous methods to interact with your Elasticsearch indices:

### CRUD Operations
**GetAllAsync():** Retrieves all documents of a particular type.

**GetByIdAsync(id):** Fetches a document by its unique identifier.

**InsertAsync(document):** Adds a new document to the Elasticsearch index.

**UpdateAsync(document):** Updates an existing document in the index.

**DeleteAsync(id):** Deletes a document by its identifier.

### Advanced Query Operations

**DeleteByQueryAsync(querySelector):** Deletes documents based on a query.

**BulkInsertAsync(documents):** Performs a bulk insertion of multiple documents.

**GetByTermQueryAsync(querySelector):** Retrieves documents matching a term query.

**GetByQueryAsync(querySelector):** Fetches documents based on a complex query.

**GetByQuerySearchAsync(querySelector):** Executes a search based on a query and returns a search response.

**ExistByTermQueryAsync(querySelector):** Checks if documents exist that match a term query.

**GetMappingInfoAsync(propertyName):** Obtains mapping information for a specific property.

## Usage
After the library is configured, you can use these repository methods to manage and query documents in your Elasticsearch instance efficiently. These operations support asynchronous execution to enhance performance and scalability of your application.

## Contributing
We welcome contributions to improve the library. Please fork this repository and submit a pull request with your changes.
