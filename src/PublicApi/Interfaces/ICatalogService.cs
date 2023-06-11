using System.Threading.Tasks;
using System.Threading;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.PublicApi.Interfaces;

public interface ICatalogService
{
    public Task<string> SaveItemAsync(CatalogType item, CancellationToken cancellationToken = default);
}
