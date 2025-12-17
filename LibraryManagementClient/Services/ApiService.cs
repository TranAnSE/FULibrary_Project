using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LibraryManagementClient.Services;

public class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _authToken;

    public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpClient GetClient()
    {
        var client = _httpClientFactory.CreateClient("FULibraryAPI");
        
        // Try to get token from session
        var token = _authToken ?? _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        return client;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var client = GetClient();
            var response = await client.GetAsync(endpoint);
            
            if (!response.IsSuccessStatusCode)
                return default;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
        }
        catch
        {
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var client = GetClient();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync(endpoint, content);
            
            if (!response.IsSuccessStatusCode)
                return default;

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
        }
        catch
        {
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint)
    {
        try
        {
            var client = GetClient();
            var response = await client.PostAsync(endpoint, null);
            
            if (!response.IsSuccessStatusCode)
                return default;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
        }
        catch
        {
            return default;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var client = GetClient();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PutAsync(endpoint, content);
            
            if (!response.IsSuccessStatusCode)
                return default;

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
        }
        catch
        {
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var client = GetClient();
            var response = await client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public void SetAuthToken(string token)
    {
        _authToken = token;
        _httpContextAccessor.HttpContext?.Session.SetString("AuthToken", token);
    }

    public void ClearAuthToken()
    {
        _authToken = null;
        _httpContextAccessor.HttpContext?.Session.Remove("AuthToken");
    }
}
