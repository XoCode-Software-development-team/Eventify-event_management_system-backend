using eventify_backend.Data;
using eventify_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ServiceController(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;

        }



        [HttpGet("/Api/[Controller]/Categories")]
        public async Task<IActionResult> GetAllServiceCategories()
        {
            var categories = await _appDbContext.ServiceCategories
                .Select(x => new { x.CategoryId, x.ServiceCategoryName })
                .ToListAsync();

            if (categories == null || categories.Count == 0)
            {
                return NotFound(false);
            }
            return Ok(categories);
        }

        [HttpGet("/Api/[Controller]/{categoryId}")]

        public async Task<IActionResult> GetServiceByCategory([FromRoute] int categoryId)
        {

            var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);

            if (serviceCategory == null)
            {
                return NotFound();
            }


            var servicesWithCategories = await _appDbContext.services
                .Include(s => s.ServiceCategory)
                .Where(s => s.ServiceCategoryId == categoryId)
                .Select(s => new
                {
                    SoRId = s.SoRId,
                    Service = s.Name,
                    Rating = s.OverallRate,
                    IsSuspend = s.IsSuspend,
                })
                .ToListAsync();

            return Ok(servicesWithCategories);
        }

        [HttpPut("/Api/[Controller]/{SoRId}")]
        public async Task<IActionResult> ChangeSuspendState([FromRoute] int SORId)
        {
            var service = await _appDbContext.ServiceAndResources.FindAsync(SORId);
            if (service == null)
            {
                return NotFound();
            }

            service.IsSuspend = !service.IsSuspend;

            await _appDbContext.SaveChangesAsync();

            var categoryId = await _appDbContext.services
                .Where(s => s.SoRId == service.SoRId)
                .Select(s => s.ServiceCategoryId)
                .FirstOrDefaultAsync();


            return Ok(categoryId);

        }

        [HttpDelete("/Api/[Controller]/{Id}")]
        public async Task<IActionResult> DeleteService([FromRoute] int Id)
        {
            var service = await _appDbContext.services.FindAsync(Id);

            if (service == null)
            {
                return NotFound(Id);
            }

            var deletedCategoryId = service.ServiceCategoryId; // Save the category before deletion

            _appDbContext.services.Remove(service);
            await _appDbContext.SaveChangesAsync();

            return Ok(deletedCategoryId);
        }

        [HttpGet("/Api/deleteRequestServices")]
        public async Task<IActionResult> GetCategoriesWithRequestToDelete()
        {
            var categoriesWithRequestToDelete = await _appDbContext.ServiceCategories
                .Join(_appDbContext.services.Where(s => s.IsRequestToDelete),
                    category => category.CategoryId,
                    service => service.ServiceCategoryId,
                    (category, service) => new { category.CategoryId, category.ServiceCategoryName })
                .Distinct()
                .ToListAsync();

            if (categoriesWithRequestToDelete.Count == 0)
            {
                return Ok(categoriesWithRequestToDelete);
            }

            return Ok(categoriesWithRequestToDelete);
        }

        [HttpGet("/Api/deleteRequestServices/{categoryId}")]

        public async Task<IActionResult> DeleteRequestServices([FromRoute] int categoryId)
        {
            var services = await _appDbContext.services.Where(s => (s.ServiceCategoryId == categoryId && s.IsRequestToDelete == true))
                .Select(s => new
                {
                    SoRId = s.SoRId,
                    ServiceName = s.Name,
                    Rating = s.OverallRate,
                })
                .ToListAsync();

            if (services == null || services.Count == 0)
            {
                return Ok(services);
            }
            return Ok(services);
        }

        [HttpPut("/Api/deleteRequestServices/{Id}")]
        public async Task<IActionResult> ChangeDeleteRequestState([FromRoute] int Id)
        {
            var service = await _appDbContext.services.FindAsync(Id);
            if (service == null)
            {
                return NotFound();
            }

            service.IsRequestToDelete = false;

            await _appDbContext.SaveChangesAsync();

            var remainingCount = await _appDbContext.services.CountAsync(s => s.ServiceCategoryId == service.ServiceCategoryId && s.IsRequestToDelete == true);

            return Ok(new { DeletedServiceCategoryId = service.ServiceCategoryId, RemainingCount = remainingCount });

        }

        [HttpDelete("/Api/deleteRequestServices/{Id}")]
        public async Task<IActionResult> ApproveVendorDeleteRequest([FromRoute] int Id)
        {
            var service = await _appDbContext.services.FindAsync(Id);

            if (service == null)
            {
                return NotFound(Id);
            }

            var deletedCategory = service.ServiceCategoryId;

            _appDbContext.services.Remove(service);
            await _appDbContext.SaveChangesAsync();

            var remainingCount = await _appDbContext.services.CountAsync(s => s.ServiceCategoryId == deletedCategory && s.IsRequestToDelete == true);

            return Ok(new { DeletedServiceCategoryId = service.ServiceCategoryId, RemainingCount = remainingCount });
        }

        [HttpGet("/Api/[Controller]/Categories/{Id}")]
        public async Task<IActionResult> GetAllServiceCategoriesOfVendor(Guid Id)
        {
            var categories = await _appDbContext.ServiceCategories
                .Where(sc => sc.Services != null && sc.Services.Any(s => s.VendorId == Id))
                .Select(x => new { x.CategoryId, x.ServiceCategoryName })
                .ToListAsync();


            if (categories == null || categories.Count == 0)
            {
                return NotFound(false);
            }

            return Ok(categories);
        }

        [HttpPut("/Api/[Controller]/deleteRequest/{SoRId}")]
        public async Task<IActionResult> RequestToDelete([FromRoute] int SORId)
        {
            var service = await _appDbContext.services.FindAsync(SORId);
            if (service == null)
            {
                return NotFound();
            }

            service.IsRequestToDelete = !service.IsRequestToDelete;

            await _appDbContext.SaveChangesAsync();

            return Ok(service.ServiceCategoryId);
        }


        [HttpGet("/Api/vendorService/{categoryId}/{vendorId}")]

        public async Task<IActionResult> GetVendorServiceByCategory([FromRoute] int categoryId, Guid vendorId)
        {

            var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);

            var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

            if (serviceCategory == null || vendor == null)
            {
                return NotFound();
            }


            var servicesWithCategories = await _appDbContext.services
                .Where(s => s.ServiceCategoryId == categoryId && s.VendorId == vendorId)
                .Select(s => new
                {
                    SoRId = s.SoRId,
                    Service = s.Name,
                    Rating = s.OverallRate,
                    IsRequestToDelete = s.IsRequestToDelete,
                })
                .ToListAsync();

            return Ok(servicesWithCategories);
        }

        [HttpGet("/Api/bookedService/Categories/{Id}")]
        public async Task<IActionResult> GetServiceCategoriesOfBookedServices(Guid Id)
        {
            try
            {
                var currentDate = DateTime.Now;

                var categories = await _appDbContext.ServiceCategories
                    .Where(sc => sc.Services != null && sc.Services.Any(s => s.EventSRs != null &&
                        s.EventSRs.Any(esr => esr.Event != null && esr.ServiceAndResource != null &&
                            esr.Event.EndDateTime > currentDate && esr.ServiceAndResource.VendorId == Id)))
                    .Select(x => new {x.CategoryId, x.ServiceCategoryName})
                    .ToListAsync();


                return Ok(categories);



            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/Api/bookedService/{categoryId}/{vendorId}")]
        public async Task<IActionResult> GetBookedServicesOfVendor(int categoryId, Guid vendorId)
        {
            try
            {
                var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);

                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (serviceCategory == null || vendor == null)
                {
                    return NotFound();
                }

                var currentDate = DateTime.Now;

                var services = await _appDbContext.services
                    .Where(s => s.ServiceCategoryId == categoryId &&
                                s.VendorId == vendorId &&
                                s.EventSRs != null &&
                                s.EventSRs.Any(e => e.Event != null && e.Event.EndDateTime > currentDate))
                    .Select(x => new
                    {
                        soRid = x.SoRId,
                        service = x.Name,
                        eventDate = x.EventSRs != null ? x.EventSRs.Select(e => e.Event != null ? e.Event.StartDateTime.Date.ToString("yyyy-MM-dd") : DateTime.MinValue.ToString("yyyy-MM-dd")).ToList() : null,
                        endDate = x.EventSRs != null ? x.EventSRs.Select(e => e.Event != null ? e.Event.EndDateTime.Date.ToString("yyyy-MM-dd") : DateTime.MinValue.ToString("yyyy-MM-dd")).ToList() : null
                    })
                    .ToListAsync();



                return Ok(services);



            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
