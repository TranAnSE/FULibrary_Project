namespace LibraryManagementClient.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task PostAsync(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
    Task<T?> PostAsync<T>(string endpoint);
    void SetAuthToken(string token);
    void ClearAuthToken();
}
