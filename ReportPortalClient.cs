using System.Text.Json;

namespace SeleniumNUnitDemo;

/// <summary>
/// Simple ReportPortal API client to send test results
/// </summary>
public class ReportPortalClient
{
    private readonly string _baseUrl;
    private readonly string _projectName;
    private readonly string _username;
    private readonly string _password;
    private readonly HttpClient _httpClient;

    public ReportPortalClient(string baseUrl, string projectName, string username, string password)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _projectName = projectName;
        _username = username;
        _password = password;
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Start a test launch in ReportPortal
    /// </summary>
    public async Task<string?> StartLaunch(string launchName, string description = "")
    {
        try
        {
            var payload = new
            {
                name = launchName,
                description = description,
                start_time = DateTime.UtcNow.ToString("O"),
                mode = "DEFAULT"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, 
                $"{_baseUrl}/api/v1/{_projectName}/launch")
            {
                Content = content
            };

            var auth = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            request.Headers.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("id", out var idElement))
                {
                    return idElement.GetString();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting launch: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Log test result to ReportPortal
    /// </summary>
    public async Task LogTestResult(string launchId, string testName, string status, string message = "")
    {
        try
        {
            var payload = new
            {
                launch_uuid = launchId,
                time = DateTime.UtcNow.ToString("O"),
                message = message,
                level = status.ToUpper() == "PASSED" ? "INFO" : "ERROR",
                item_type = "STEP"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{_baseUrl}/api/v1/{_projectName}/log")
            {
                Content = content
            };

            var auth = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            await _httpClient.SendAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error logging result: {ex.Message}");
        }
    }

    /// <summary>
    /// Finish a launch in ReportPortal
    /// </summary>
    public async Task FinishLaunch(string launchId, string status = "PASSED")
    {
        try
        {
            var payload = new
            {
                end_time = DateTime.UtcNow.ToString("O"),
                status = status
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                $"{_baseUrl}/api/v1/{_projectName}/launch/{launchId}/finish")
            {
                Content = content
            };

            var auth = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            await _httpClient.SendAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error finishing launch: {ex.Message}");
        }
    }
}
