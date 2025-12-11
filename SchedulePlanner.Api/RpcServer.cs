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
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string RpcQueueName = "rpc_queue";

    public RpcServer(IConfiguration configuration, ILogger<RpcServer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // Инициализация соединения перенесена в ExecuteAsync, так как она теперь асинхронная
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost"
        };

        // 1. Создаем соединение и канал асинхронно
        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync();

        // 2. Объявляем очередь (QueueDeclareAsync)
        await _channel.QueueDeclareAsync(queue: RpcQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        
        // 3. QoS (BasicQosAsync)
        await _channel.BasicQosAsync(0, 1, false);

        _logger.LogInformation("Awaiting RPC requests on queue '{Queue}'", RpcQueueName);

        // 4. Используем AsyncEventingBasicConsumer вместо EventingBasicConsumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            string response = string.Empty;
            var body = ea.Body.ToArray();
            
            // Получаем свойства входящего сообщения
            var props = ea.BasicProperties;
            var replyTo = props.ReplyTo;
            var correlationId = props.CorrelationId;

            // Создаем свойства для ответа
            var replyProps = new BasicProperties
            {
                CorrelationId = correlationId
            };

            try
            {
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received RPC request: '{Message}'", message);

                // Логика обработки
                response = $"Responding to '{message}' at {DateTime.UtcNow:O}";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing RPC request");
                response = "Error: " + e.Message;
            }
            finally
            {
                if (!string.IsNullOrEmpty(replyTo))
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    
                    // 5. Публикуем ответ асинхронно (BasicPublishAsync)
                    await _channel.BasicPublishAsync(exchange: "", routingKey: replyTo, mandatory: false, basicProperties: replyProps, body: responseBytes);
                    
                    // 6. Подтверждаем обработку (BasicAckAsync)
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            }
        };

        // 7. Подписываемся на очередь
        await _channel.BasicConsumeAsync(queue: RpcQueueName, autoAck: false, consumer: consumer);

        // Ждем остановки приложения
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {

        base.Dispose();
    }
}