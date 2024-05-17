using eventify_backend.DTOs;
using eventify_backend.Services;
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

        [HttpGet("/api/[Controller]/categories")]
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
        [HttpGet("/api/[Controller]/{categoryId}")]
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

        [HttpPut("/api/[Controller]/{SoRId}")]
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

        [HttpDelete("/api/[Controller]/{Id}")]
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

        [HttpGet("/api/[controller]/deleteRequest")]
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

        [HttpGet("/api/[controller]/deleteRequest/{categoryId}")]
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


        [HttpPut("/api/[controller]/deleteRequest/change/{Id}")]
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

        [HttpDelete("/api/[controller]/deleteRequestApprove/{Id}")]
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

        [HttpGet("/api/[Controller]/categories/{Id}")]
        public async Task<IActionResult> GetAllResourceCategoriesOfVendor(Guid Id)
        {
            try
            {
                var result = await _resourceService.GetAllResourceCategoriesOfVendorAsync(Id);

                if (result == null)
                    return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("/api/vendorResource/{categoryId}/{vendorId}")]
        public async Task<IActionResult> GetVendorResourceByCategory([FromRoute] int categoryId, Guid vendorId)
        {
            try
            {
                var result = await _resourceService.GetVendorResourceByCategoryAsync(categoryId, vendorId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("/api/[Controller]/deleteRequest/{SoRId}")]
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

        [HttpGet("/api/bookedResource/Categories/{Id}")]
        public async Task<IActionResult> GetResourceCategoriesOfBookedResources(Guid Id)
        {
            try
            {
                var result = await _resourceService.GetResourceCategoriesOfBookedResourcesAsync(Id);

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

        [HttpGet("/api/bookedResource/{categoryId}/{vendorId}")]
        public async Task<IActionResult> GetBookedResourcesOfVendor(int categoryId, Guid vendorId)
        {
            try
            {
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

        [HttpGet("/api/bookingRequestResource/{vendorId}")]
        public async Task<IActionResult> GetCategoriesOfBookingRequest(Guid vendorId)
        {
            try
            {
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

        [HttpGet("/api/bookingRequestResource/{categoryId}/{vendorId}")]
        public async Task<IActionResult> GetResourcesOfBookingRequest(int categoryId, Guid vendorId)
        {
            try
            {
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

        [HttpPost("/api/[Controller]/addNew/{vendorId}")]
        public async Task<IActionResult> AddNewResource([FromRoute] Guid vendorId, [FromBody] object data)
        {
            try
            {
                await _resourceService.AddNewResourceAsync(vendorId, data);
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/api/[Controller]/maxPrice/{modelId}")]
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


        [HttpGet("/api/[Controller]/all")]
        public async Task<IActionResult> GetResourcesForClients()
        {
            try
            {
                var result = await _resourceService.GetResourcesForClientsAsync();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
