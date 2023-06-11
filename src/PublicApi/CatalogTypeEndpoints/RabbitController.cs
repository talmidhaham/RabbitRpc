using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.PublicApi.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Microsoft.eShopWeb.PublicApi.CatalogTypeEndpoints;
[Route("api/[controller]")]
[ApiController]
public class RabbitController : ControllerBase
{

    private ICatalogService _ICatalogService;
    public RabbitController(ICatalogService iCatalogService)
    {
        this._ICatalogService = iCatalogService;
    }
    // GET: api/<ValuesController>
    //[HttpGet]
    //public IEnumerable<string> Get()
    //{
    //    return new string[] { "value1", "value2" };
    //}

    //// GET api/<ValuesController>/5
    //[HttpGet("{id}")]
    //public string Get(int id)
    //{
    //    return "value";
    //}

    // POST api/<ValuesController>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CatalogType value)
    {

       //var rpcResult = await _ICatalogService.SaveItemAsync(value);



        //return rpcResult.ToString();


        int timeout = 5000;
        var task = _ICatalogService.SaveItemAsync(value);
        if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
        {
            // task completed within timeout
          return Ok(await task);
        }
        else
        {
            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status408RequestTimeout, "timeout");
        }



    }

    //// PUT api/<ValuesController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<ValuesController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
