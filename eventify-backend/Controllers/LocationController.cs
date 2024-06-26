using eventify_backend.Data;
using eventify_backend.Models;
using eventify_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {

        private readonly AppDbContext _appDbContext;
        private readonly GeocodingService _geoCodingService;


        public LocationController(AppDbContext appDbContext, GeocodingService geoCodingService)
        {
            _appDbContext = appDbContext;
            _geoCodingService = geoCodingService;
        }


        // GET: api/location/service/{query}
        [HttpGet("service")]
        public async Task<IActionResult> GetServiceLocations([FromQuery] string? query)
        {
            try
            {
                IQueryable<Service> servicesQuery = _appDbContext.Services
                    .Include(s => s.Vendor)
                    .Include(s => s.VendorSRLocations)
                    .Include(s => s.VendorRSPhotos)
                    .Where(s => !s.IsSuspend);

                if (!string.IsNullOrEmpty(query))
                {
                    servicesQuery = servicesQuery.Where(s => s.VendorSRLocations!.Any(v => v.District!.StartsWith(query)));
                }

                var services = await servicesQuery.ToListAsync();

                var geocodedLocations = await _geoCodingService.GeocodeServicesAsync(services);

                var responseData = services.Select(s => new
                {
                    VendorName = s.Vendor!.CompanyName!,
                    image = s.VendorRSPhotos!.Select(x => x.Image),
                    soRid = s.SoRId,
                    Name = s.Name,
                    OverallRating = s.OverallRate,
                    Locations = geocodedLocations
                .Where(gl => gl.SoRId == s.SoRId && (string.IsNullOrEmpty(query) || gl.District!.ToUpper().StartsWith(query.ToUpper())))
                                .Select(gl => new { gl.Latitude, gl.Longitude, gl.District })
                                .ToList()
                }); ;

                return Ok(responseData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("resource")]
        public async Task<IActionResult> GetResourceLocations([FromQuery] string? query)
        {
            try
            {
                IQueryable<Resource> resourcesQuery = _appDbContext.Resources
                    .Include(s => s.Vendor)
                    .Include(s => s.VendorSRLocations)
                    .Include(s => s.VendorRSPhotos)
                    .Where(s => !s.IsSuspend);

                if (!string.IsNullOrEmpty(query))
                {
                    resourcesQuery = resourcesQuery.Where(s => s.VendorSRLocations!.Any(v => v.District!.StartsWith(query)));
                }

                var resources = await resourcesQuery.ToListAsync();

                var geocodedLocations = await _geoCodingService.GeocodeResourcesAsync(resources);

                var responseData = resources.Select(s => new
                {
                    VendorName = s.Vendor!.CompanyName!,
                    image = s.VendorRSPhotos!.Select(x => x.Image),
                    soRid = s.SoRId,
                    Name = s.Name,
                    OverallRating = s.OverallRate,
                    Locations = geocodedLocations
                .Where(gl => gl.SoRId == s.SoRId && (string.IsNullOrEmpty(query) || gl.District!.ToUpper().StartsWith(query.ToUpper())))
                                .Select(gl => new { gl.Latitude, gl.Longitude, gl.District })
                                .ToList()
                }); ;

                return Ok(responseData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

