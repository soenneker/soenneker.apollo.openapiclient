[![](https://img.shields.io/nuget/v/soenneker.apollo.openapiclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.apollo.openapiclient/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.apollo.openapiclient/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.apollo.openapiclient/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.apollo.openapiclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.apollo.openapiclient/)

# Soenneker.Apollo.OpenApiClient

A Kiota-generated .NET client and model set for Apollo's REST API.

The package is generated from Apollo's OpenAPI document and exposes typed request builders for the operations in that document. It intentionally does not decide how your application stores credentials or manages `HttpClient` lifetime.

For dependency injection, authentication, and client caching out of the box, use [`Soenneker.Apollo.OpenApiClientUtil`](https://www.nuget.org/packages/Soenneker.Apollo.OpenApiClientUtil).

## Installation

```bash
dotnet add package Soenneker.Apollo.OpenApiClient
```

## Create a client manually

Configure a reusable `HttpClient`, attach Apollo's API key, and give it to a Kiota request adapter:

```csharp
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Apollo.OpenApiClient;

var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://api.apollo.io/api/v1")
};

httpClient.DefaultRequestHeaders.Add("x-api-key", apolloApiKey);

var authenticationProvider = new AnonymousAuthenticationProvider();
var requestAdapter = new HttpClientRequestAdapter(
    authenticationProvider,
    httpClient: httpClient);

var client = new ApolloOpenApiClient(requestAdapter);
```

`AnonymousAuthenticationProvider` is appropriate in this example because authentication is already applied to the `HttpClient`. Applications can instead provide another Kiota `IAuthenticationProvider` that adds the required header.

Do not create and dispose a new `HttpClient` for every API call. Reuse the client stack or let a DI-managed utility own it.

## Make a request

Request builders follow Apollo's URL hierarchy. For example, retrieve the authenticated user profile and request credit usage:

```csharp
using Soenneker.Apollo.OpenApiClient.Models;

GetCurrentUserProfile200Response? profile =
    await client.Users.Api_profile.GetAsync(
        request =>
        {
            request.QueryParameters.IncludeCreditUsage = true;
        },
        cancellationToken);
```

Endpoint methods accept a request-configuration callback for query parameters, headers, and Kiota request options. They also accept a `CancellationToken`.

The root client includes builders for areas such as:

- accounts, account stages, contacts, and contact stages;
- people and organization enrichment/search;
- conversations, phone calls, notes, and tasks;
- opportunities and opportunity stages;
- sequences, email accounts, campaigns, messages, and schedules;
- reports, usage statistics, user profiles, and webhook results.

Use IntelliSense from `ApolloOpenApiClient` to find the generated path and method for a particular endpoint.

## Errors and generated models

Kiota maps documented non-success responses to generated exception classes. A profile request, for example, can throw `GetCurrentUserProfile200Response401Error`. Other endpoints may map 401, 403, 422, 429, or additional responses from Apollo's specification.

Generated response and request types implement Kiota serialization contracts. Unknown JSON fields are retained in `AdditionalData`, which helps clients tolerate additive API responses.

## Regeneration considerations

This repository's API surface is generated and can change when Apollo updates its OpenAPI document:

- request-builder and model names reflect the source specification;
- endpoint documentation and error mappings come from that specification;
- generated files should not be edited as the durable way to customize behavior;
- application code should isolate important Apollo workflows behind its own services and models.

The default base URL embedded in the generated client is `https://api.apollo.io/api/v1`. A custom `IRequestAdapter` can supply a different `BaseUrl` for a proxy or test environment.

## Recommended application setup

Applications using Microsoft dependency injection generally need only the higher-level utility:

```csharp
using Soenneker.Apollo.OpenApiClientUtil.Registrars;

builder.Services.AddApolloOpenApiClientUtilAsSingleton();
```

See the [OpenApiClientUtil package](https://www.nuget.org/packages/Soenneker.Apollo.OpenApiClientUtil) for configuration and injection examples.
