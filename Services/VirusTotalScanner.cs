using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SecureFileUploadDemo.Services;
using System.Net.Http.Headers;

public class VirusTotalScanner : IAntivirusScanner
{
    private readonly string apiKey = "b5a23702a1585cd58d7a604826366eecc2f514bd27d7eae02c1d1515d9fe0f0d";

    public async Task<bool> ScanAsync(IFormFile file)
    {
        using var http = new HttpClient();

        using var form = new MultipartFormDataContent();
        var streamContent = new StreamContent(file.OpenReadStream());
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

        form.Add(streamContent, "file", file.FileName);

        var response = await http.PostAsync(
            "https://www.virustotal.com/api/v3/files",
            form
        );

        var result = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(result);

        int maliciousCount =
            json["data"]?["attributes"]?["last_analysis_stats"]?["malicious"]?.Value<int>() ?? 0;

        return maliciousCount == 0; // Safe if zero
    }
}
