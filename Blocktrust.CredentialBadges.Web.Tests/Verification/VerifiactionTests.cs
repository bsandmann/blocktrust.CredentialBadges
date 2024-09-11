using Blocktrust.CredentialBadges.Core.Commands.CheckPrismDIDSignature;
using Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;
using Blocktrust.CredentialBadges.Core.Commands.CheckSignature;
using Blocktrust.CredentialBadges.Core.Commands.CheckTrustRegistry;
using Blocktrust.CredentialBadges.Core.Common;
using Blocktrust.CredentialBadges.Core.Crypto;
using Blocktrust.CredentialBadges.Core.Services.DIDPrism;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Blocktrust.CredentialBadges.Web.Tests.TestCredentials;

namespace Blocktrust.CredentialBadges.Web.Tests.Verification;

public class VerificationTests
{
    private readonly IMediator _mediator;
    private readonly CheckSignatureHandler _checkSignatureHandler;
    private readonly CheckRevocationStatusHandler _checkRevocationStatusHandler;
    private readonly CheckTrustRegistryHandler _checkTrustRegistryHandler;

    public VerificationTests()
    {
        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CheckSignatureHandler).Assembly));
        services.AddSingleton<EcServiceBouncyCastle>();
        services.AddSingleton<ExtractPrismPubKeyFromLongFormDid>();
        services.AddTransient<CheckPrismDIDSignatureHandler>();

        var serviceProvider = services.BuildServiceProvider();

        _mediator = serviceProvider.GetRequiredService<IMediator>();
        _checkSignatureHandler = new CheckSignatureHandler(_mediator);
        // _checkRevocationStatusHandler = new CheckRevocationStatusHandler();
        _checkTrustRegistryHandler = new CheckTrustRegistryHandler();
    }

    [Fact]
    public async Task CheckSignatureHandler_Should_Return_Valid_For_Valid_Signature()
    {
        // Arrange
        var parserResult = CredentialParser.Parse(PrismCredentials.LatestCredential);
        parserResult.IsSuccess.Should().BeTrue();

        var request = new CheckSignatureRequest(parserResult.Value);

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

        // Tamper with the credential to make the signature invalid
        parserResult.Value.Jwt.Signature = "Invalid Signature";

        var request = new CheckSignatureRequest(parserResult.Value);

        // Act
        var result = await _checkSignatureHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        // result.Value.Should().Be(ECheckSignatureResponse.Invalid);
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