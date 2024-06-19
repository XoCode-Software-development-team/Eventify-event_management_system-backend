using eventify_backend.Data;
using eventify_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompareController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public CompareController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet("category/{soRId}")]
        public async Task<IActionResult> GetCategoryName(int soRId)
        {
            try
            {
                if (soRId == 0)
                {
                    return BadRequest(new { Message = "Please request with Id!" });
                }

                var item = await _appDbContext.ServiceAndResources.FirstOrDefaultAsync(s => s.SoRId == soRId);

                if (item == null)
                {
                    return NotFound(new { Message = $"Service or Resource with SoRId {soRId} not found!" });
                }

                string categoryName = string.Empty;

                if (item is Service)
                {
                    var serviceEntity = item as Service;
                    var serviceCategory = await _appDbContext.ServiceCategories.FirstOrDefaultAsync(sc => sc.CategoryId == serviceEntity!.ServiceCategoryId);
                    categoryName = serviceCategory?.ServiceCategoryName ?? "Category Not Found";
                }
                else if (item is Resource)
                {
                    var resourceEntity = item as Resource;
                    var resourceCategory = await _appDbContext.ResourceCategories.FirstOrDefaultAsync(rc => rc.CategoryId == resourceEntity!.ResourceCategoryId);
                    categoryName = resourceCategory?.ResourceCategoryName ?? "Category Not Found";
                }
                else
                {
                    return BadRequest(new { Message = $"Invalid Service or Resource type for SoRId {soRId}" });
                }

                return Ok(new { CategoryName = categoryName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("overview/{soRIds}")]
        public async Task<IActionResult> GetOverview([FromRoute] string soRIds)
        {
            try
            {
                var soRIdArray = soRIds.Split(',').Select(int.Parse).ToArray();

                if (soRIdArray == null || soRIdArray.Length < 3)
                {
                    return BadRequest(new { Message = "Please request with at least three Ids!" });
                }

                var services = await _appDbContext.ServiceAndResources
                    .Where(s => soRIdArray.Contains(s.SoRId))
                    .Include(s => s.Vendor) // Include related Vendor data
                    .ToListAsync();

                if (services.Count != soRIdArray.Length)
                {
                    return NotFound(new { Message = "One or more services or resources not found!" });
                }

                var overviews = services.Select(s => new
                {
                    s.SoRId,
                    Overview = new
                    {
                        CompanyName = s.Vendor!.CompanyName,
                        AvatarUrl = s.Vendor.ProfilePic
                    }
                }).ToList();

                return Ok(overviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("features/{soRIds}")]
        public async Task<IActionResult> GetFeatures([FromRoute] string soRIds)
        {
            if (string.IsNullOrWhiteSpace(soRIds))
            {
                return BadRequest(new { Message = "Please provide valid soRIds." });
            }

            try
            {
                var soRIdArray = soRIds.Split(',').Select(id =>
                {
                    if (int.TryParse(id, out var parsedId))
                    {
                        return parsedId;
                    }
                    throw new ArgumentException($"Invalid soRId: {id}");
                }).ToArray();

                if (soRIdArray.Length < 3)
                {
                    return BadRequest(new { Message = "Please request with at least three Ids!" });
                }

                var items = await _appDbContext.ServiceAndResources
                    .Where(s => soRIdArray.Contains(s.SoRId))
                    .Include(s => s.FeaturesAndFacilities)
                    .ToListAsync();

                if (items.Count != soRIdArray.Length)
                {
                    return NotFound(new { Message = "One or more services or resources not found!" });
                }

                var features = items.Select(s => new 
                {
                    SoRId = s.SoRId,
                    Features = s.FeaturesAndFacilities!.Select(f => f.FacilityName).ToList()
                }).ToList();

                return Ok(features);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (log implementation not shown here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("images/{soRIds}")]
        public async Task<IActionResult> GetImages([FromRoute] string soRIds)
        {
            if (string.IsNullOrWhiteSpace(soRIds))
            {
                return BadRequest(new { Message = "Please provide valid soRIds." });
            }

            try
            {
                var soRIdArray = soRIds.Split(',').Select(id =>
                {
                    if (int.TryParse(id, out var parsedId))
                    {
                        return parsedId;
                    }
                    throw new ArgumentException($"Invalid soRId: {id}");
                }).ToArray();

                if (soRIdArray.Length < 3)
                {
                    return BadRequest(new { Message = "Please request with at least three Ids!" });
                }

                var items = await _appDbContext.ServiceAndResources
                    .Where(s => soRIdArray.Contains(s.SoRId))
                    .Include(s => s.VendorRSPhotos)
                    .ToListAsync();

                if (items.Count != soRIdArray.Length)
                {
                    return NotFound(new { Message = "One or more services or resources not found!" });
                }

                var images = items.Select(s => new
                {
                    SoRId = s.SoRId,
                    Images = s.VendorRSPhotos!.Select(f => f.Image).ToList()
                }).ToList();

                return Ok(images);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (log implementation not shown here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("videos/{soRIds}")]
        public async Task<IActionResult> GetVideos([FromRoute] string soRIds)
        {
            if (string.IsNullOrWhiteSpace(soRIds))
            {
                return BadRequest(new { Message = "Please provide valid soRIds." });
            }

            try
            {
                var soRIdArray = soRIds.Split(',').Select(id =>
                {
                    if (int.TryParse(id, out var parsedId))
                    {
                        return parsedId;
                    }
                    throw new ArgumentException($"Invalid soRId: {id}");
                }).ToArray();

                if (soRIdArray.Length < 3)
                {
                    return BadRequest(new { Message = "Please request with at least three Ids!" });
                }

                var items = await _appDbContext.ServiceAndResources
                    .Where(s => soRIdArray.Contains(s.SoRId))
                    .Include(s => s.VendorRSVideos)
                    .ToListAsync();

                if (items.Count != soRIdArray.Length)
                {
                    return NotFound(new { Message = "One or more services or resources not found!" });
                }

                var videos = items.Select(s => new
                {
                    SoRId = s.SoRId,
                    Videos = s.VendorRSVideos!.Select(f => f.Video).ToList()
                }).ToList();

                return Ok(videos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (log implementation not shown here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("manuals/{soRIds}")]
        public async Task<IActionResult> GetManuals([FromRoute] string soRIds)
        {
            if (string.IsNullOrWhiteSpace(soRIds))
            {
                return BadRequest(new { Message = "Please provide valid soRIds." });
            }

            try
            {
                var soRIdArray = soRIds.Split(',').Select(id =>
                {
                    if (int.TryParse(id, out var parsedId))
                    {
                        return parsedId;
                    }
                    throw new ArgumentException($"Invalid soRId: {id}");
                }).ToArray();

                if (soRIdArray.Length < 3)
                {
                    return BadRequest(new { Message = "Please request with at least three Ids!" });
                }

                var items = await _appDbContext.Resources
                    .Where(s => soRIdArray.Contains(s.SoRId))
                    .Include(s => s.ResourceManual)
                    .ToListAsync();

                if (items.Count != soRIdArray.Length)
                {
                    return NotFound(new { Message = "One or more services or resources not found!" });
                }

                var manuals = items.Select(s => new
                {
                    SoRId = s.SoRId,
                    Manuals = s.ResourceManual!.Select(f => f.Manual).ToList()
                }).ToList();

                return Ok(manuals);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (log implementation not shown here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpGet("prices/{soRIds}")]
        public async Task<IActionResult> GetPrices([FromRoute] string soRIds)
        {
            if (string.IsNullOrWhiteSpace(soRIds))
            {
                return BadRequest(new { Message = "Please provide valid soRIds." });
            }

            try
            {
                var soRIdArray = soRIds.Split(',').Select(id =>
                {
                    if (int.TryParse(id, out var parsedId))
                    {
                        return parsedId;
                    }
                    throw new ArgumentException($"Invalid soRId: {id}");
                }).ToArray();

                if (soRIdArray.Length < 3)
                {
                    return BadRequest(new { Message = "Please request with at least three Ids!" });
                }

                var items = await _appDbContext.VendorSRPrices
                    .Where(s => soRIdArray.Contains(s.SoRId))
                    .Include(s => s.Price!)
                    .ThenInclude(p => p.PriceModel!)
                    .ToListAsync();

                items = items.GroupBy(s => s.SoRId).Select(group => group.First()).ToList();

                var foundSoRIds = items.Select(s => s.SoRId).ToList();
                var missingSoRIds = soRIdArray.Except(foundSoRIds).ToList();

                if (missingSoRIds.Any())
                {
                    return NotFound(new { Message = "One or more services or resources not found!", MissingIds = missingSoRIds });
                }

                var Prices = items.Select(s => new
                {
                 SoRId = s.SoRId,
                 Price = (
                                    from vp in _appDbContext.VendorSRPrices
                                    join p in _appDbContext.Prices on vp.PId equals p.Pid
                                    join pm in _appDbContext.PriceModels on p.ModelId equals pm.ModelId
                                    where vp.SoRId == s.SoRId
                                    select new
                                    {
                                        Value = p.BasePrice,
                                        ModelName = pm.ModelName,
                                        Name = p.Pname
                                    }
                                ).ToList(),
                }).ToList();

                return Ok(Prices);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (log implementation not shown here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("locations/{soRIds}")]
        public async Task<IActionResult> GetLocations([FromRoute] string soRIds)
        {
            if (string.IsNullOrWhiteSpace(soRIds))
            {
                return BadRequest(new { Message = "Please provide valid soRIds." });
            }

            try
            {
                var soRIdArray = soRIds.Split(',').Select(id =>
                {
                    if (int.TryParse(id, out var parsedId))
                    {
                        return parsedId;
                    }
                    throw new ArgumentException($"Invalid soRId: {id}");
                }).ToArray();

                if (soRIdArray.Length < 3)
                {
                    return BadRequest(new { Message = "Please request with at least three Ids!" });
                }

                var items = await _appDbContext.ServiceAndResources
                    .Where(s => soRIdArray.Contains(s.SoRId))
                    .Include(s => s.VendorSRLocations)
                    .ToListAsync();

                if (items.Count != soRIdArray.Length)
                {
                    return NotFound(new { Message = "One or more services or resources not found!" });
                }

                var locations = items.Select(s => new
                {
                    SoRId = s.SoRId,
                    Locations = s.VendorSRLocations!.Select(f => new {f.State,f.HouseNo,f.Area,f.Country,f.District}).ToList()
                }).ToList();

                return Ok(locations);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (log implementation not shown here)
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("ratings/{soRIds}")]
        public async Task<IActionResult> GetRatings([FromRoute] string soRIds)
        {
            try
            {
                var soRIdArray = soRIds.Split(',').Select(int.Parse).ToArray();

                if (soRIdArray == null || soRIdArray.Length < 3)
                {
                    return BadRequest(new { Message = "Please request with at least three Ids!" });
                }

                var services = await _appDbContext.ServiceAndResources
                    .Where(s => soRIdArray.Contains(s.SoRId))
                    .ToListAsync();

                if (services.Count != soRIdArray.Length)
                {
                    return NotFound(new { Message = "One or more services or resources not found!" });
                }

                var ratings = services.Select(s => new
                {
                    s.SoRId,
                    s.OverallRate
                }).ToList();

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


    }
}
