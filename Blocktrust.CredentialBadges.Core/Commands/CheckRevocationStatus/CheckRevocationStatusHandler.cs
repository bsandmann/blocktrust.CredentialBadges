using System.IO.Compression;
using System.Text.Json;
using Blocktrust.CredentialBadges.OpenBadges;
using MediatR;


namespace Blocktrust.CredentialBadges.Core.Commands.CheckRevocationStatus;

public class CheckRevocationStatusHandler : IRequestHandler<CheckRevocationStatusRequest, CheckRevocationStatusResponse>
{
    private readonly HttpClient _httpClient;

    public CheckRevocationStatusHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CheckRevocationStatusResponse> Handle(CheckRevocationStatusRequest request, CancellationToken cancellationToken)
    {
        var statusListCredential = await FetchStatusListCredential(request.CredentialStatus.StatusListCredential, cancellationToken);
        var isRevoked = CheckRevocationStatus(statusListCredential, request.CredentialStatus.StatusListIndex);

        return new CheckRevocationStatusResponse
        {
            IsRevoked = isRevoked,
            CredentialId = request.CredentialStatus.Id.ToString()
        };
    }

    private async Task<StatusList2021Credential> FetchStatusListCredential(string url, CancellationToken cancellationToken)
    {
        // Development environment URL modification
        url = url.Replace("http://10.10.50.105:8000/", "http://212.124.51.147:35412/");
        try
        {
        var response = await _httpClient.GetStringAsync(url, cancellationToken);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<StatusList2021Credential>(response, options);
        
        }
        catch (Exception e)
        {
            throw new Exception("Error during fetching status list credential", e);
        }
    }

    private bool CheckRevocationStatus(StatusList2021Credential statusListCredential, int? statusListIndex)
    {
        
        var decodedList = DecodeBase64Url(statusListCredential.CredentialSubject.EncodedList);
        var decompressedList = Decompress(decodedList);

        int byteIndex = (int)(statusListIndex / 8)!;
        int bitIndex = (int)(statusListIndex % 8)!;

        return (decompressedList[byteIndex] & (1 << (7 - bitIndex))) != 0;
    }

    private byte[] DecodeBase64Url(string input)
    {
        string padded = input.Length % 4 == 0
            ? input
            : input + "====".Substring(input.Length % 4);
        string converted = padded.Replace('_', '/').Replace('-', '+');
        return Convert.FromBase64String(converted);
    }
    
    private byte[] Decompress(byte[] compressedData)
    {
        try
        {
            using var compressedStream = new MemoryStream(compressedData);
            using var decompressStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var resultStream = new MemoryStream();
            decompressStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
        catch (InvalidDataException)
        {
            // If decompression fails, it might not be compressed
            return compressedData;
        }
    }}
