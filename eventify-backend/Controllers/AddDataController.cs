using eventify_backend.Data;
using eventify_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddDataController : ControllerBase
    {

        private readonly AppDbContext _appDbContext;
        public AddDataController(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }


        [HttpPost("Service")]
        public async Task<ActionResult<Service>> PostService(Service service)
        {
            _appDbContext.services.Add(service);
            await _appDbContext.SaveChangesAsync();

            return CreatedAtAction("GetService", new { id = service.SoRId }, service);
        }

        // POST: api/Data/ServiceCategory
        [HttpPost("ServiceCategory")]
        public async Task<ActionResult<ServiceCategory>> PostServiceCategory(ServiceCategory serviceCategory)
        {
            _appDbContext.ServiceCategories.Add(serviceCategory);
            await _appDbContext.SaveChangesAsync();

            return CreatedAtAction("GetServiceCategory", new { id = serviceCategory.CategoryId }, serviceCategory);
        }

        // POST: api/Data/ServiceAndResource
        [HttpPost("ServiceAndResource")]
        public async Task<ActionResult<ServiceAndResource>> PostServiceAndResource(ServiceAndResource serviceAndResource)
        {
            _appDbContext.ServiceAndResources.Add(serviceAndResource);
            await _appDbContext.SaveChangesAsync();

            return CreatedAtAction("GetServiceAndResource", new { id = serviceAndResource.SoRId }, serviceAndResource);
        }

        // POST: /Api/Vendor
        [HttpPost("Vendor")]
        public async Task<IActionResult> AddVendor(Vendor vendor)
        {
            try
            {
                _appDbContext.Vendors.Add(vendor);
                await _appDbContext.SaveChangesAsync();
                return Ok(vendor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the vendor: {ex.Message}");
            }
        }

        // POST: /Api/Vendor
        [HttpPost("Client")]
        public async Task<IActionResult> AddClient(Client client)
        {
            try
            {
                _appDbContext.Clients.Add(client);
                await _appDbContext.SaveChangesAsync();
                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the client: {ex.Message}");
            }
        }
    }
}
