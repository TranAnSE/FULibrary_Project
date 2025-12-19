using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibraryManagementClient.Services;

public class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiService> _logger;
    private string? _authToken;

    public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<ApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private HttpClient GetClient()
    {
        var client = _httpClientFactory.CreateClient("FULibraryAPI");
        
        // Try to get token from multiple sources
        var token = _authToken ?? 
                   _httpContextAccessor.HttpContext?.Session.GetString("AuthToken") ??
                   _httpContextAccessor.HttpContext?.User?.FindFirst("jwt_token")?.Value;
        
        if (!string.IsNullOrEmpty(token))
        {
            _logger.LogInformation("Using auth token for API request");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _logger.LogWarning("No auth token found in session, service, or claims");
        }
        
        return client;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var client = GetClient();
            _logger.LogInformation("GET Request: {Endpoint}", endpoint);
            
            var response = await client.GetAsync(endpoint);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("GET {Endpoint} failed with {StatusCode}: {Error}", endpoint, response.StatusCode, error);
                
                // If we get 401, clear the token as it might be invalid
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Received 401 Unauthorized, clearing auth token");
                    ClearAuthToken();
                }
                
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GET {Endpoint}", endpoint);
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

            _logger.LogInformation("POST Request: {Endpoint}", endpoint);
            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("POST {Endpoint} failed with {StatusCode}: {Error}", endpoint, response.StatusCode, error);

                // If we get 401, clear the token as it might be invalid
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Received 401 Unauthorized, clearing auth token");
                    ClearAuthToken();
                }

                // Try to extract error message from response
                try
                {
                    var errorObj = JsonSerializer.Deserialize<Dictionary<string, object>>(error, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (errorObj != null && errorObj.ContainsKey("message"))
                    {
                        throw new HttpRequestException(errorObj["message"].ToString());
                    }
                }
                catch (JsonException)
                {
                    // If JSON parsing fails, throw the raw error
                }

                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {error}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in POST {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task PostAsync(string endpoint, object data)
    {
        try
        {
            var client = GetClient();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("POST Request: {Endpoint}", endpoint);
            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("POST {Endpoint} failed with {StatusCode}: {Error}", endpoint, response.StatusCode, error);

                // If we get 401, clear the token as it might be invalid
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Received 401 Unauthorized, clearing auth token");
                    ClearAuthToken();
                }

                // Try to extract error message from response
                try
                {
                    var errorObj = JsonSerializer.Deserialize<Dictionary<string, object>>(error, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (errorObj != null && errorObj.ContainsKey("message"))
                    {
                        throw new HttpRequestException(errorObj["message"].ToString());
                    }
                }
                catch (JsonException)
                {
                    // If JSON parsing fails, throw the raw error
                }

                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {error}");
            }
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in POST {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint)
    {
        try
        {
            var client = GetClient();
            var response = await client.PostAsync(endpoint, null);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("POST {Endpoint} failed with {StatusCode}: {Error}", endpoint, response.StatusCode, error);
                
                // If we get 401, clear the token as it might be invalid
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Received 401 Unauthorized, clearing auth token");
                    ClearAuthToken();
                }
                
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in POST {Endpoint}", endpoint);
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
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("PUT {Endpoint} failed with {StatusCode}: {Error}", endpoint, response.StatusCode, error);
                
                // If we get 401, clear the token as it might be invalid
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Received 401 Unauthorized, clearing auth token");
                    ClearAuthToken();
                }
                
                return default;
            }

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
