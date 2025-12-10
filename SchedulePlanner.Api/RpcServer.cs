using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SchedulePlanner.Api;

public class RpcServer : BackgroundService
{
    private readonly ILogger<RpcServer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string RpcQueueName = "rpc_queue";

    public RpcServer(IConfiguration configuration, ILogger<RpcServer> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory() { HostName = configuration["RabbitMQ:HostName"] ?? "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: RpcQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.BasicQos(0, 1, false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        _channel.BasicConsume(queue: RpcQueueName, autoAck: false, consumer: consumer);
        
        _logger.LogInformation("Awaiting RPC requests on queue '{Queue}'", RpcQueueName);

        consumer.Received += (model, ea) =>
        {
            string response = null;
            var body = ea.Body.ToArray();
            var props = ea.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received RPC request: '{Message}'", message);
                // For demonstration, we just return a simple message.
                // In a real application, you would do some work here.
                response = $"Responding to '{message}' at {DateTime.UtcNow:O}";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing RPC request");
                response = "Error: " + e.Message;
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
