namespace Blocktrust.CredentialBadges.Web.Tests;

using System;
using MediatR;
using Moq;
using Xunit;

[Collection("TransactionalTests")]
public partial class TestSetup : IDisposable
{
    private TransactionalTestDatabaseFixture Fixture { get; }

    private readonly ApplicationDbContext _context;
    private readonly Mock<IMediator> _mediatorMock;

    public TestSetup(TransactionalTestDatabaseFixture fixture)
    {
        this.Fixture = fixture;
        this._context = this.Fixture.CreateContext();
        this._mediatorMock = new Mock<IMediator>();
    }

    public void Dispose()
    {
        this.Fixture.Cleanup();
    }
}