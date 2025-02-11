using Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;
using Blocktrust.CredentialBadges.OpenBadges;
using FluentAssertions;

namespace Blocktrust.CredentialBadges.Core.Tests.Commands;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class CheckRevocationTests
{
    private readonly HttpClient _httpClient;

    public CheckRevocationTests()
    {
        _httpClient = new HttpClient();
    }

    [Fact]
    public async Task CheckRevocation_ActualRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var handler = new CheckRevocationStatusHandler(_httpClient);
        var request = new CheckRevocationStatusRequest(CreateCredentialStatus(3));

        // Act
        var response = await handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.CredentialId.Should().Contain("/credential-status/");
    }



    [Fact]
    public async Task CheckRevocation_NotRevokedDccCredential_ReturnsNotRevokedStatus()
    {
        // Arrange
        var handler = new CheckRevocationStatusHandler(_httpClient);
        var request = new CheckRevocationStatusRequest(CreateDccCredentialStatus(2));

        // Act
        var response = await handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.CredentialId.Should().Contain("https://digitalcredentials.github.io/credential-status-playground/JWZM3H8WKU#2");
        response.IsRevoked.Should().BeFalse("Credential with status list index 2 should not be revoked");
    }



    private CredentialStatus CreateCredentialStatus(int statusListIndex)
    {
        return new CredentialStatus
        {
            StatusPurpose = "Revocation",
            StatusListIndex = statusListIndex,
            Id = new Uri($"http://10.10.50.105:8000/cloud-agent/credential-status/b9b6bb1e-6864-4074-b8ac-12b3a0b30f0c#{statusListIndex}"),
            Type = "StatusList2021Entry",
            StatusListCredential = "http://10.10.50.105:8000/cloud-agent/credential-status/b9b6bb1e-6864-4074-b8ac-12b3a0b30f0c"
        };
    }

    private CredentialStatus CreateDccCredentialStatus(int statusListIndex)
    {
        return new CredentialStatus
        {
            StatusPurpose = "revocation",
            StatusListIndex = statusListIndex,
            Id = new Uri($"https://digitalcredentials.github.io/credential-status-playground/JWZM3H8WKU#{statusListIndex}"),
            Type = "StatusList2021Entry",
            StatusListCredential = "https://digitalcredentials.github.io/credential-status-playground/JWZM3H8WKU"
        };
    }
}