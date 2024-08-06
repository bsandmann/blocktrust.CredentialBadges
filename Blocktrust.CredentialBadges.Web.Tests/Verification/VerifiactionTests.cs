namespace Blocktrust.CredentialBadges.Web.Tests.Verification;

using Core.Commands.CheckRevocationStatus;
using Core.Commands.CheckSignature;
using Core.Commands.CheckTrustRegistry;
using Core.Commands.VerifyOpenBadge;
using Core.Common;
using FluentResults;
using Xunit;
using FluentResults.Extensions.FluentAssertions;
using MediatR;
using Moq;
using OpenBadges;
using Org.BouncyCastle.Asn1.Ocsp;
using TestCredentials;
using Result = FluentResults.Result;

public partial class TestSetup
{
    private Mock<IMediator> _mediatorMock = new Mock<IMediator>();
    private CheckSignatureHandler checkSignatureHandler = new CheckSignatureHandler();
    private CheckRevocationStatusHandler checkRevocationStatusHandler = new CheckRevocationStatusHandler();
    private CheckTrustRegistryHandler checkchTrustRegistryHandler = new CheckTrustRegistryHandler();

    public TestSetup()
    {
        _mediatorMock.Setup(p => p.Send(It.IsAny<CheckSignatureRequest>(), It.IsAny<CancellationToken>()))
            .Returns(async (CheckSignatureRequest Request, CancellationToken token) => await checkSignatureHandler.Handle(Request, token));

        _mediatorMock.Setup(p => p.Send(It.IsAny<CheckRevocationStatusRequest>(), It.IsAny<CancellationToken>()))
            .Returns(async (CheckRevocationStatusRequest Request, CancellationToken token) => await checkRevocationStatusHandler.Handle(Request, token));

        _mediatorMock.Setup(p => p.Send(It.IsAny<CheckTrustRegistryRequest>(), It.IsAny<CancellationToken>()))
            .Returns(async (CheckTrustRegistryRequest Request, CancellationToken token) => await checkchTrustRegistryHandler.Handle(Request, token));
    }

    [Fact]
    public async Task Simple_VerifiactionTest_1()
    {
        var parserResult = CredentialParser.Parse(PrismCredentials.PrismCredentialJohnWorking);
        parserResult.Should().BeSuccess();

        var verifiyOpenBadgeRequest = new VerifyOpenBadgeRequest(parserResult.Value);

        // Act
        var handler = new VerifyOpenBadgeHandler(_mediatorMock.Object);
        var verifivationResult = await handler.Handle(verifiyOpenBadgeRequest, CancellationToken.None);

        // Assert
        verifivationResult.Should().BeSuccess();
    }
}