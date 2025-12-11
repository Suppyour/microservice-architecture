using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SchedulePlanner.Domain.Interfaces;

namespace SchedulePlanner.Infrastructure;

public class RpcClient : IRpcClient, IDisposable
{
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _replyQueueName;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new();
    
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _isInitialized = false;

    private const string RpcQueueName = "rpc_queue";

    public RpcClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    private async Task InitializeAsync()
    {
        if (_isInitialized) return;
        
        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized) return;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            var queueDeclareResult = await _channel.QueueDeclareAsync(); 
            _replyQueueName = queueDeclareResult.QueueName;

            var consumer = new AsyncEventingBasicConsumer(_channel);
            
            consumer.ReceivedAsync += (model, ea) =>
            {
                var correlationId = ea.BasicProperties.CorrelationId;
                
                if (!_callbackMapper.TryRemove(correlationId!, out var tcs))
                {
                    return Task.CompletedTask;
                }

                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                
                tcs.TrySetResult(response);
                
                return Task.CompletedTask;
            };
            
            await _channel.BasicConsumeAsync(
                queue: _replyQueueName,
                autoAck: true,
                consumer: consumer);

            _isInitialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<string> CallAsync(string message, CancellationToken cancellationToken = default)
    {
        await InitializeAsync();

        var correlationId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        
        _callbackMapper.TryAdd(correlationId, tcs);
        
        var props = new BasicProperties
        {
            CorrelationId = correlationId,
            ReplyTo = _replyQueueName
        };

        var messageBytes = Encoding.UTF8.GetBytes(message);

        try
        {
            await _channel!.BasicPublishAsync(
                exchange: "",
                routingKey: RpcQueueName,
                mandatory: false,
                basicProperties: props,
                body: messageBytes,
                cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            _callbackMapper.TryRemove(correlationId, out _);
            throw;
        }
        
        using var registration = cancellationToken.Register(() =>
        {
            if (_callbackMapper.TryRemove(correlationId, out var removedTcs))
            {
                removedTcs.TrySetCanceled();
            }
        });

        return await tcs.Task;
    }

    public void Dispose()
    {
        try 
        {
            _channel?.CloseAsync().GetAwaiter().GetResult();
            _connection?.CloseAsync().GetAwaiter().GetResult();
        }
        catch
        {
            // ignored
        }

        _initLock.Dispose();
    }
}