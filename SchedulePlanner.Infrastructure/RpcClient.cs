using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SchedulePlanner.Domain.Interfaces;

namespace SchedulePlanner.Infrastructure;

public class RpcClient : IRpcClient
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _replyQueueName;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new();
    private const string RpcQueueName = "rpc_queue";

    public RpcClient(IConfiguration configuration)
    {
        var factory = new ConnectionFactory() { HostName = configuration["RabbitMQ:HostName"] ?? "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _replyQueueName = _channel.QueueDeclare().QueueName;
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                return;
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            tcs.TrySetResult(response);
        };

        _channel.BasicConsume(
            consumer: consumer,
            queue: _replyQueueName,
            autoAck: true);
    }

    public Task<string> CallAsync(string message, CancellationToken cancellationToken = default)
    {
        var props = _channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = _replyQueueName;
        var messageBytes = Encoding.UTF8.GetBytes(message);

        var tcs = new TaskCompletionSource<string>();
        _callbackMapper.TryAdd(correlationId, tcs);

        _channel.BasicPublish(
            exchange: "",
            routingKey: RpcQueueName,
            basicProperties: props,
            body: messageBytes);
        
        cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out _));
        return tcs.Task;
    }

    public void Dispose()
    {
        _connection.Close();
    }
}
