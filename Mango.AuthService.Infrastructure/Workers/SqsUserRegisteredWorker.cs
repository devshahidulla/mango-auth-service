using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Mango.AuthService.Application.Interfaces;
using System.Text.Json;
using Mango.AuthService.Domain.Events;
using Microsoft.Extensions.Configuration;
using Mango.AuthService.Domain.Entities;

namespace Mango.AuthService.Infrastructure.Workers;

public class SqsUserRegisteredWorker : BackgroundService
{
  private readonly IAmazonSQS _sqs;
  private readonly ILogger<SqsUserRegisteredWorker> _logger;
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly IConfiguration _config;

  private string? _queueUrl;

  public SqsUserRegisteredWorker(
      IAmazonSQS sqs,
      IServiceScopeFactory scopeFactory,
      ILogger<SqsUserRegisteredWorker> logger,
      IConfiguration config)
  {
    _sqs = sqs;
    _scopeFactory = scopeFactory;
    _logger = logger;
    _config = config;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _queueUrl = _config["AWS:SqsUserRegisteredQueueUrl"]; // e.g., https://sqs.us-east-1.amazonaws.com/123456789012/mango-user-registered

    if (string.IsNullOrWhiteSpace(_queueUrl))
    {
      _logger.LogError("SQS Queue URL is not configured.");
      return;
    }

    _logger.LogInformation("SqsUserRegisteredWorker started. Listening to queue: {QueueUrl}", _queueUrl);

    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
        {
          QueueUrl = _queueUrl,
          MaxNumberOfMessages = 5,
          WaitTimeSeconds = 10
        }, stoppingToken);

        if (response.Messages.Any())
        {
          foreach (var message in response.Messages)
          {
            using var scope = _scopeFactory.CreateScope();
            var userSyncService = scope.ServiceProvider.GetRequiredService<IUserSyncService>();

            try
            {
              var envelope = JsonSerializer.Deserialize<EventBridgeEnvelope<UserRegisteredEvent>>(message.Body, new JsonSerializerOptions
              {
                PropertyNameCaseInsensitive = true
              });

              var userEvent = envelope?.Detail;//JsonSerializer.Deserialize<UserRegisteredEvent>(message.Body);
              if (userEvent != null)
              {
                var user = new User
                {
                  UserId = userEvent.UserId,
                  FullName = userEvent.FullName,
                  Email = userEvent.Email,
                  Role = userEvent.Role,
                  CreatedAt = userEvent.CreatedAt,
                  PasswordHash = userEvent.PasswordHash
                };
                await userSyncService.SaveUserAsync(user, stoppingToken);
                _logger.LogInformation("Processed user registration for: {Email}", userEvent.Email);

                // delete message from queue
                await _sqs.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
              }
              else
              {
                _logger.LogWarning("Received malformed message.");
              }
            }
            catch (Exception ex)
            {
              _logger.LogError(ex, "Failed to process message: {Message}", message.Body);
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error polling SQS queue.");
      }

      await Task.Delay(1000, stoppingToken); // small delay before next poll
    }
  }
}
