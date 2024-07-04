using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace Offices.Infrastructure.HttpClients;

public class DocumentsServiceHttpClient
{
    public HttpClient HttpClient { get; }

    public DocumentsServiceHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://localhost:7208/api/");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "InnoClinic.OfficesService");

        HttpClient = httpClient;
    }

    public async Task<string> SaveFile(IFormFile officePhoto)
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(officePhoto.OpenReadStream());

        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(officePhoto.ContentType);

        content.Add(fileContent, "file", officePhoto.FileName);

        var response = await HttpClient.PostAsync("documents", content);

        return await response.Content.ReadAsStringAsync();
    }
}
