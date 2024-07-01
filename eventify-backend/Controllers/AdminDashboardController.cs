using eventify_backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public AdminDashboardController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet("clients")]
        [Authorize]

        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var clients = await _appDbContext.Clients.Select(c => new { c.FirstName, c.LastName, c.Email }).ToListAsync();

                if (clients == null || !clients.Any())
                {
                    return NotFound("No clients found.");
                }

                return Ok(clients);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("vendors")]
        [Authorize]

        public async Task<IActionResult> GetAllVendors()
        {
            try
            {
                await UpdateAllVendorsAverageRating();

                var vendors = await _appDbContext.Vendors.Select(c => new { c.CompanyName, c.rate, c.Email }).ToListAsync();

                if (vendors == null || !vendors.Any())
                {
                    return NotFound("No vendors found.");
                }

                return Ok(vendors);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task UpdateAllVendorsAverageRating()
        {
            try
            {
                var vendors = await _appDbContext.Vendors
                    .Include(v => v.ServiceAndResources)
                    .ToListAsync();

                foreach (var vendor in vendors)
                {
                    if (vendor.ServiceAndResources != null && vendor.ServiceAndResources.Count > 0)
                    {
                        float averageRating = vendor.ServiceAndResources.Average(s => s.OverallRate) ?? 0.0f;
                        vendor.rate = (float)Math.Round(averageRating, 1);
                    }
                    else
                    {
                        vendor.rate = 0;
                    }

                    _appDbContext.Vendors.Update(vendor);
                }

                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }

}
