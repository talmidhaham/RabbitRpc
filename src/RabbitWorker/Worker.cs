using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.Infrastructure.Data;
using Newtonsoft.Json;

namespace RabbitWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private IRabbitServerFactory _IRabbitServerFactory;
    private IRepository<CatalogType> _IRepository;

    public Worker(ILogger<Worker> logger, IRabbitServerFactory iRabbitServerFactory, IRepository<CatalogType> iRepository)
    {
        _logger = logger;
        this._IRabbitServerFactory = iRabbitServerFactory;
        this._IRepository = iRepository;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // _IRabbitServerFactory.startServer(stoppingToken);
        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //    await Task.Delay(1000, stoppingToken);
        //}






        var factory = new ConnectionFactory { HostName = "host.docker.internal" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "rpc_queue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue: "rpc_queue",
                             autoAck: false,
                             consumer: consumer);
        Console.WriteLine(" [x] Awaiting RPC requests");

        consumer.Received += async (model, ea) =>
        {
            string response = string.Empty;

            var body = ea.Body.ToArray();
            var props = ea.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {

                var message = Encoding.UTF8.GetString(body);
                CatalogType CType = JsonConvert.DeserializeObject<CatalogType>(message);
                CatalogType? res;
                if (CType.Id == -1)
                {
                     res = await _IRepository.AddAsync(new CatalogType(CType.Type));
                }
                else
                {
                     res = await _IRepository.GetByIdAsync(CType.Id);
                }

                if (res == null)
                {
                    res = new CatalogType("Item Not Found ") { Id = -2};
                }
                
                var json = JsonConvert.SerializeObject(res);
                var ConvByte = Encoding.UTF8.GetBytes(json);
                response = Encoding.UTF8.GetString(ConvByte);
            }
            catch (Exception e)
            {
                Console.WriteLine($" [.] {e.Message}");
                response = string.Empty;
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: props.ReplyTo,
                                     basicProperties: replyProps,
                                     body: responseBytes);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }



    static int Fib(int n)
    {
        if (n is 0 or 1)
        {
            return n;
        }

        return Fib(n - 1) + Fib(n - 2);
    }
}


