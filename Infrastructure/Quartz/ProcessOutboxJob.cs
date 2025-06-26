using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Infrastructure.Database.Abstractions;
using Core.Domain;


namespace Infrastructure.Quartz;

[DisallowConcurrentExecution]
public class ProcessOutboxJob : IJob
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessOutboxJob> _logger;

    public ProcessOutboxJob(ISqlConnectionFactory sqlConnectionFactory, IMediator mediator, ILogger<ProcessOutboxJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var connection = _sqlConnectionFactory.CreateOpenConnection();

        string sql = @"SELECT Id, Type, Data 
                       FROM SystemZarzadzania.OutboxMessages 
                       WHERE ProcessedDate IS NULL";

        var messages = await connection.QueryAsync<OutboxMessageDto>(sql);

        foreach (var message in messages)
        {
            try
            {
                var type = Type.GetType(message.Type);
                if (type == null)
                {
                    _logger.LogError($"Type not found: {message.Type}");
                    continue;
                }

                var notification = JsonConvert.DeserializeObject(message.Data, type, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                }) as INotification;

                if (notification == null)
                {
                    _logger.LogError($"Deserialized message does not implement INotification: {message.Type}");
                    continue;
                }

                await _mediator.Publish(notification);

                string updateSql = "UPDATE SystemZarzadzania.OutboxMessages SET ProcessedDate = @Date WHERE Id = @Id";
                await connection.ExecuteAsync(updateSql, new { Date = DateTime.UtcNow, Id = message.Id });

                _logger.LogInformation($"Processed outbox message {message.Id} of type {message.Type}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing outbox message {message.Id}");
            }
        }
    }
}

public class OutboxMessageDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
}
