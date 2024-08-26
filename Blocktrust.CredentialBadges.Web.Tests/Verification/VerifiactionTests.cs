using Blocktrust.CredentialBadges.Core.Commands.CheckPrismDIDSignature;
using Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Commands.CheckTrustRegistry;
using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using Blocktrust.CredentialBadges.Core.Common;
using Blocktrust.CredentialBadges.Core.Crypto;
using Blocktrust.CredentialBadges.Core.Prism;
using FluentAssertions;
using FluentResults;
using MediatR;
using Moq;
using Blocktrust.CredentialBadges.Web.Tests.TestCredentials;

namespace Blocktrust.CredentialBadges.Web.Tests.Verification;

public class VerificationTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CheckSignatureHandler _checkSignatureHandler;
    private readonly CheckRevocationStatusHandler _checkRevocationStatusHandler;
    private readonly CheckTrustRegistryHandler _checkTrustRegistryHandler;

    public VerificationTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _checkSignatureHandler = new CheckSignatureHandler(_mediatorMock.Object);
        _checkRevocationStatusHandler = new CheckRevocationStatusHandler();
        _checkTrustRegistryHandler = new CheckTrustRegistryHandler();

        _mediatorMock.Setup(p => p.Send(It.IsAny<CheckSignatureRequest>(), It.IsAny<CancellationToken>()))
            .Returns((CheckSignatureRequest request, CancellationToken token) => _checkSignatureHandler.Handle(request, token));

        _mediatorMock.Setup(p => p.Send(It.IsAny<CheckRevocationStatusRequest>(), It.IsAny<CancellationToken>()))
            .Returns((CheckRevocationStatusRequest request, CancellationToken token) => _checkRevocationStatusHandler.Handle(request, token));

        _mediatorMock.Setup(p => p.Send(It.IsAny<CheckTrustRegistryRequest>(), It.IsAny<CancellationToken>()))
            .Returns((CheckTrustRegistryRequest request, CancellationToken token) => _checkTrustRegistryHandler.Handle(request, token));
    }

    [Fact]
    public async Task Simple_VerificationTest_1()
    {
        // Arrange
        var parserResult = CredentialParser.Parse(PrismCredentials.LatestCredential);
        parserResult.IsSuccess.Should().BeTrue();

        var verifyOpenBadgeRequest = new VerifyOpenBadgeRequest(parserResult.Value);

        // Mock the CheckPrismDIDSignatureHandler to return Valid
        _mediatorMock.Setup(p => p.Send(It.IsAny<CheckPrismDIDSignatureRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ECheckSignatureResponse.Valid));

        // Act
        var handler = new VerifyOpenBadgeHandler(_mediatorMock.Object);
        var verificationResult = await handler.Handle(verifyOpenBadgeRequest, CancellationToken.None);

        // Assert
        verificationResult.IsSuccess.Should().BeTrue();
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
        validation.Should().BeTrue();
    }

    [Fact]
    public async Task CheckSignatureHandler_Should_Return_Valid_For_Valid_Signature()
    {
        // Arrange
        var parserResult = CredentialParser.Parse(PrismCredentials.LatestCredential);
        parserResult.IsSuccess.Should().BeTrue();

        var request = new CheckSignatureRequest(parserResult.Value);

        // Mock the CheckPrismDIDSignatureHandler to return Valid
        _mediatorMock.Setup(p => p.Send(It.IsAny<CheckPrismDIDSignatureRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ECheckSignatureResponse.Valid));

        // Act
        var result = await _checkSignatureHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ECheckSignatureResponse.Valid);
    }

    [Fact]
    public async Task CheckSignatureHandler_Should_Return_Invalid_For_Invalid_Signature()
    {
        // Arrange
        var parserResult = CredentialParser.Parse(PrismCredentials.LatestCredential);
        parserResult.IsSuccess.Should().BeTrue();

        var request = new CheckSignatureRequest(parserResult.Value);

        // Mock the CheckPrismDIDSignatureHandler to return Invalid
        _mediatorMock.Setup(p => p.Send(It.IsAny<CheckPrismDIDSignatureRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(ECheckSignatureResponse.Invalid));

        // Act
        var result = await _checkSignatureHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ECheckSignatureResponse.Invalid);
    }

    [Fact]
    public async Task CheckSignatureHandler_Should_Return_UnsupportedDidMethod_For_Unsupported_DID()
    {
        // Arrange
        var parserResult = CredentialParser.Parse(PrismCredentials.LatestCredential);
        parserResult.IsSuccess.Should().BeTrue();

        // Modify the DID to an unsupported method
        parserResult.Value.Issuer.Id = new Uri("did:unsupported:example");

        var request = new CheckSignatureRequest(parserResult.Value);

        // Act
        var result = await _checkSignatureHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ECheckSignatureResponse.UnsupportedDidMethod);
    }
}