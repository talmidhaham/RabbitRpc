using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Microsoft.eShopWeb.Infrastructure.Data;
public class RabitMQRepository : IRabitMQRepository
{
    private  IRabbitClientFactory _IRabbitClientFactory ;

    public RabitMQRepository(IRabbitClientFactory iRabbitClientFactory)
    {
        this._IRabbitClientFactory = iRabbitClientFactory;
    }

    public async Task<string> SendMessage<T>(T message)
    {
        var json = JsonConvert.SerializeObject(message);
        var ConvByte = Encoding.UTF8.GetBytes(json);
        var body = Encoding.UTF8.GetString(ConvByte);
        var response = await _IRabbitClientFactory.CallAsync(body);
        return response;
    }
}
