using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.PublicApi.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.Services;

public class CatalogService : ICatalogService
{
    private IRabitMQRepository _IRabitMQRepository;



    public CatalogService(IRabitMQRepository iIRabitMQRepository)
    {
        this._IRabitMQRepository = iIRabitMQRepository;
    }




    public async Task<string> SaveItemAsync(CatalogType item,CancellationToken cancellationToken = default)
    {
          return  await _IRabitMQRepository.SendMessage<CatalogType>(item);

    }
}
