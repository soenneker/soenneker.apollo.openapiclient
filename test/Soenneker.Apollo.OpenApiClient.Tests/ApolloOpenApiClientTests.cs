using Soenneker.Tests.HostedUnit;

namespace Soenneker.Apollo.OpenApiClient.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class ApolloOpenApiClientTests : HostedUnitTest
{
    public ApolloOpenApiClientTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
