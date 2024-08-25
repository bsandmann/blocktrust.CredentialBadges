namespace Blocktrust.CredentialBadges.Web.Tests.Verification;

using Core.Commands.CheckRevocationStatus;
using Core.Commands.CheckSignature;
using Core.Commands.CheckTrustRegistry;
using Core.Commands.VerifyOpenBadge;
using Core.Common;
using Core.Crypto;
using Core.Prism;
using Xunit;
using FluentResults.Extensions.FluentAssertions;
using MediatR;
using Moq;
using TestCredentials;

public partial class TestSetup
{
    private Mock<IMediator> _mediatorMock = new();
    private CheckSignatureHandler checkSignatureHandler = new();
    private CheckRevocationStatusHandler checkRevocationStatusHandler = new();
    private CheckTrustRegistryHandler checkchTrustRegistryHandler = new();

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
        var parserResult = CredentialParser.Parse(PrismCredentials.LatestCredential);
        parserResult.Should().BeSuccess();

        var verifiyOpenBadgeRequest = new VerifyOpenBadgeRequest(parserResult.Value);

        // Act
        var handler = new VerifyOpenBadgeHandler(_mediatorMock.Object);
        var verifivationResult = await handler.Handle(verifiyOpenBadgeRequest, CancellationToken.None);

        // Assert
        verifivationResult.Should().BeSuccess();
    }
    
    [Fact]
    public void BouncyCastle_is_able_to_sign_and_verify_message_as_expected()
    {
        // Arrange
        var privateKeyHex = "c1edddfe6b0dfdcd7dd5af7bac2c44bd5361b6e9516985d5fd1d971d91520628";
        var publicKeyHex = "04b07372dd8077224e8cdede04908627dbc7749c8c148e21204ab5863bce9fa0a5063fd7e55baee446ee97c979f37507ae3ce21b4c261075398d54382e3ada2e2f";
        var testMessage = "abcdefgh123456";
        var testMessageBytes = PrismEncoding.Utf8StringToByteArray(testMessage);
        var ecService = new EcServiceBouncyCastle();

        // Act
        var signature = ecService.SignData(testMessageBytes, PrismEncoding.HexToByteArray(privateKeyHex));
        var validation = ecService.VerifyData(testMessageBytes, signature, PrismEncoding.HexToByteArray(publicKeyHex));

        // Assert
        Assert.True(validation);
    }
}