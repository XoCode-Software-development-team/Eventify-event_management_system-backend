using eventify_backend.DTOs;
using eventify_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly ResourceService _resourceService;

        // Constructor with Dependency Injection for ResourceService
        public ResourceController(ResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        // Endpoint to get all service categories

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllResourceCategories()
        {
            try
            {
                // Retrieve resource categories using the service layer
                List<ResourceCategoryDTO> categories = await _resourceService.GetAllResourceCategories();

                // If no categories found, return NotFound
                if (categories == null || categories.Count == 0)
                {
                    return NotFound(false);
                }

                // Return the list of categories
                return Ok(categories);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        // GET: api/resource/{categoryId}
        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetResourceByCategory([FromRoute] int categoryId)
        {
            try
            {
                // Retrieve resources by category from the service layer
                var services = await _resourceService.GetResourcesByCategoryId(categoryId);

                if (services == null || services.Count == 0)
                {
                    // Return 404 Not Found if no resources found
                    return NotFound("not found");
                }

                // Return the retrieved resources
                return Ok(services);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{SoRId}")]
        [Authorize(Policy = "adminPolicy")]
        public async Task<IActionResult> ChangeSuspendState([FromRoute] int SORId)
        {
            try
            {
                var categoryId = await _resourceService.ChangeSuspendStateAsync(SORId);
                if (categoryId == null)
                    return NotFound();

                return Ok(categoryId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{Id}")]
        [Authorize(Policy = "adminPolicy")]
        public async Task<IActionResult> DeleteResource([FromRoute] int Id)
        {
            try
            {
                var deletedCategoryId = await _resourceService.DeleteResourceAsync(Id);
                if (deletedCategoryId == null)
                    return NotFound("Resource not found.");

                return Ok(deletedCategoryId);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("deleteRequest")]
        [Authorize(Policy = "adminPolicy")]
        public async Task<IActionResult> GetCategoriesWithRequestToDelete()
        {
            try
            {
                var categoriesWithRequestToDelete = await _resourceService.GetCategoriesWithRequestToDeleteAsync();
                if (categoriesWithRequestToDelete == null)
                    return NotFound("not found");

                return Ok(categoriesWithRequestToDelete);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("deleteRequest/{categoryId}")]
        [Authorize(Policy = "adminPolicy")]
        public async Task<IActionResult> DeleteRequestResources([FromRoute] int categoryId)
        {
            try
            {
                var resources = await _resourceService.GetResourcesWithRequestToDeleteAsync(categoryId);
                if (resources == null)
                    return NotFound("not found");

                return Ok(resources);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut("deleteRequest/change/{Id}")]
        [Authorize(Policy = "adminPolicy")]
        public async Task<IActionResult> ChangeDeleteRequestState([FromRoute] int Id)
        {
            try
            {
                var result = await _resourceService.ChangeDeleteRequestStateAsync(Id);

                if (result == null)
                    return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("deleteRequestApprove/{Id}")]
        [Authorize(Policy = "adminPolicy")]
        public async Task<IActionResult> ApproveVendorDeleteRequest([FromRoute] int Id)
        {
            try
            {
                var result = await _resourceService.ApproveVendorDeleteRequestAsync(Id);

                if (result == null)
                    return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("vendor/categories/all")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> GetAllResourceCategoriesOfVendor()
        {
            try
            {
                // Extract vendorId from the JWT token
                var vendorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (vendorIdClaim == null)
                {
                    return Unauthorized(new { Message = "Vendor ID is missing in the token." });
                }

                var vendorId = Guid.Parse(vendorIdClaim.Value);

                var result = await _resourceService.GetAllResourceCategoriesOfVendorAsync(vendorId);

                if (result == null)
                    return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("/api/vendorResource/{categoryId}")]
        [Authorize(Policy = "VendorPolicy")]

        public async Task<IActionResult> GetVendorResourceByCategory([FromRoute] int categoryId)
        {
            try
            {
                // Extract vendorId from the JWT token
                var vendorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (vendorIdClaim == null)
                {
                    return Unauthorized(new { Message = "Vendor ID is missing in the token." });
                }

                var vendorId = Guid.Parse(vendorIdClaim.Value);

                var result = await _resourceService.GetVendorResourceByCategoryAsync(categoryId, vendorId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("deleteRequest/{SoRId}")]
        [Authorize(Policy = "vendorPolicy")]
        public async Task<IActionResult> RequestToDelete([FromRoute] int SORId)
        {
            try
            {
                var result = await _resourceService.RequestToDeleteAsync(SORId);
                if (result == 0)
                    return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("/api/bookedResource/Categories")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> GetResourceCategoriesOfBookedResources()
        {
            try
            {
                // Extract vendorId from the JWT token
                var vendorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (vendorIdClaim == null)
                {
                    return Unauthorized(new { Message = "Vendor ID is missing in the token." });
                }

                var vendorId = Guid.Parse(vendorIdClaim.Value);

                var result = await _resourceService.GetResourceCategoriesOfBookedResourcesAsync(vendorId);

                if (result == null)
                {
                    return NotFound("not found");
                }

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/api/bookedResource/{categoryId}")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> GetBookedResourcesOfVendor(int categoryId)
        {
            try
            {
                // Extract vendorId from the JWT token
                var vendorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (vendorIdClaim == null)
                {
                    return Unauthorized(new { Message = "Vendor ID is missing in the token." });
                }

                var vendorId = Guid.Parse(vendorIdClaim.Value);

                var result = await _resourceService.GetBookedResourcesOfVendorAsync(categoryId, vendorId);
                if (result == null)
                    return NotFound("not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/api/bookingRequestResource")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> GetCategoriesOfBookingRequest()
        {
            try
            {
                // Extract vendorId from the JWT token
                var vendorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (vendorIdClaim == null)
                {
                    return Unauthorized(new { Message = "Vendor ID is missing in the token." });
                }

                var vendorId = Guid.Parse(vendorIdClaim.Value);

                var result = await _resourceService.GetCategoriesOfBookingRequestAsync(vendorId);
                if (result == null)
                    return NotFound("not found");


                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/api/bookingRequestResource/{categoryId}")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> GetResourcesOfBookingRequest(int categoryId)
        {
            try
            {
                // Extract vendorId from the JWT token
                var vendorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (vendorIdClaim == null)
                {
                    return Unauthorized(new { Message = "Vendor ID is missing in the token." });
                }

                var vendorId = Guid.Parse(vendorIdClaim.Value);

                var result = await _resourceService.GetResourcesOfBookingRequestAsync(categoryId, vendorId);
                if (result == null)
                    return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("/api/bookingRequestApproveResource/{eventId}/{soRId}")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> BookResourceByVendor([FromRoute] int eventId, int soRId)
        {
            try
            {
                var result = await _resourceService.BookResourceByVendorAsync(eventId, soRId);
                if (!result) return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("/api/bookingRequestRejectResource/{eventId}/{soRId}")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> RejectResourceFromVendor([FromRoute] int eventId, int soRId)
        {
            try
            {
                var result = await _resourceService.RejectResourceFromVendorAsync(eventId, soRId);

                if (!result) return NotFound("not found");


                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("addNew")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> AddNewResource([FromBody] object data)
        {
            try
            {
                // Extract vendorId from the JWT token
                var vendorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (vendorIdClaim == null)
                {
                    return Unauthorized(new { Message = "Vendor ID is missing in the token." });
                }

                var vendorId = Guid.Parse(vendorIdClaim.Value);

                await _resourceService.AddNewResourceAsync(vendorId, data);
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("maxPrice/{modelId}")]
        public async Task<IActionResult> GetMaxPriceOfResource(int modelId)
        {
            try
            {
                var result = await _resourceService.GetMaxPriceOfResourceAsync(modelId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetResourcesForClients([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string sortBy, [FromQuery] int? minPrice, [FromQuery] int? maxPrice, [FromQuery] int? modelId, [FromQuery] string categories, [FromQuery] int? rate)
        {
            try
            {
                var result = await _resourceService.GetResourcesForClientsAsync(page, pageSize, sortBy, minPrice, maxPrice, modelId, categories, rate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("details/{soRId}")]
        public async Task<IActionResult> GetResourceDetailsForClient(int soRId)
        {
            try
            {
                var result = await _resourceService.GetResourceDetailsForClientAsync(soRId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update/{soRId}")]
        [Authorize(Policy = "vendorPolicy")]
        public async Task<IActionResult> UpdateResource([FromRoute] int soRId, [FromBody] object data)
        {
            try
            {
                // Extract vendorId from the JWT token
                var vendorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (vendorIdClaim == null)
                {
                    return Unauthorized(new { Message = "Vendor ID is missing in the token." });
                }

                var vendorId = Guid.Parse(vendorIdClaim.Value);

                await _resourceService.UpdateResourceAsync(vendorId, soRId, data);
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("priceModels/available")]
        public async Task<IActionResult> GetAvailablePriceModels()
        {
            try
            {
                var result = await _resourceService.GetAvailablePriceModelsAsync();
                if (result == null || result.Count == 0)
                {
                    return NotFound(); // No price models found
                }

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("ratingcount")]
        public async Task<IActionResult> GetRatingCount(int soRId)
        {
            try
            {
                var result = await _resourceService.GetRatingCountAsync();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
