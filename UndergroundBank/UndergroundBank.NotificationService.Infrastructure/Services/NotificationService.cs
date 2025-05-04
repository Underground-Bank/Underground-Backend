using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using UndergroundBank.NotificationService.Domain.Entities;

public class NotificationService
{
    private readonly FirebaseConfig _firebaseConfig;

    public NotificationService(IOptions<FirebaseConfig> firebaseOptions)
    {
        _firebaseConfig = firebaseOptions.Value;
    }

    public async Task SendNotificationAsync(string deviceToken, string title, string body)
    {
        var credential = GoogleCredential
            .FromFile(_firebaseConfig.CredentialsPath)
            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );

        var message = new
        {
            message = new
            {
                token = deviceToken,
                notification = new { title = title, body = body },
            },
        };

        var jsonMessage = JsonSerializer.Serialize(message);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://fcm.googleapis.com/v1/projects/{_firebaseConfig.ProjectId}/messages:send"
        )
        {
            Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json"),
        };

        var response = await httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Error sending FCM notification: {response.StatusCode} - {responseContent}"
            );
        }
    }
}
