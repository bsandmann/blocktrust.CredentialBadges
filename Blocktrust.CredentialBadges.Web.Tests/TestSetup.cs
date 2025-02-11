namespace Blocktrust.CredentialBadges.Web.Tests;

using System;
using MediatR;
using Moq;
using Xunit;

[Collection("TransactionalTests")]
public partial class TestSetup : IDisposable
{
    protected TransactionalTestDatabaseFixture Fixture { get; }
    protected Mock<IMediator> MediatorMock { get; }

    public TestSetup(TransactionalTestDatabaseFixture fixture)
    {
        Fixture = fixture;
        MediatorMock = new Mock<IMediator>();
    }

    public void Dispose()
    {
        // Clean up logic after each test
        Fixture.Cleanup();
    }
}