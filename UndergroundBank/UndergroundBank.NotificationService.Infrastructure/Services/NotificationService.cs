using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EasyNetQ;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using UndergroundBank.Common.Dto.AccountService;
using UndergroundBank.Common.Dto.Notification;
using UndergroundBank.NotificationService.Domain.Entities;

public class NotificationService
{
    private readonly FirebaseConfig _firebaseConfig;
    private readonly IBus _bus;

    public NotificationService(IOptions<FirebaseConfig> firebaseOptions)
    {
        _firebaseConfig = firebaseOptions.Value;
        _bus = RabbitHutch.CreateBus("host=localhost");
    }

    public async Task SendNotificationAsync(NotificationDto dto)
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

        var user = await _bus.Rpc.RequestAsync<Guid, UserFirebaseDto>(
            Guid.Parse(dto.UserId),
            x => x.WithQueueName("firebase_UserProfileResponse")
        );

        var message = new
        {
            message = new
            {
                token = user.FirebaseId,
                notification = new { title = dto.Title, body = dto.Body },
            },
        };

        var jsonMessage = System.Text.Json.JsonSerializer.Serialize(message);

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
