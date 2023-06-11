using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Threading;

namespace Microsoft.eShopWeb.Infrastructure.Services;
public class RabbitServerFactory : IRabbitServerFactory
{

    private IRepository<CatalogType> _IRepository;

    public RabbitServerFactory(IRepository<CatalogType> iRepository)
    {
        this._IRepository = iRepository;

    }



    public void startServer(CancellationToken stoppingToken)
    {

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
                    //var message = Encoding.UTF8.GetString(body);
                    //CatalogType CType = JsonSerializer.Deserialize<CatalogType>(message);
                    //var res = await _IRepository.AddAsync(new CatalogType(CType.Type));
                    // response = res.ToString();
                    response = Fib(10).ToString();
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
            var i = 4;
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
