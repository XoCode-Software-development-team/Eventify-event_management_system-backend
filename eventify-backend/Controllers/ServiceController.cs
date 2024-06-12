
using eventify_backend.DTOs;
using eventify_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {

        private readonly ServiceService _serviceService;


        // Constructor injection of ServiceService
        public ServiceController(ServiceService serviceService)
        {
            this._serviceService = serviceService;

        }

        // Endpoint to get all service categories

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllServiceCategories()
        {
            try
            {
                // Retrieve service categories using the service layer
                List<ServiceCategoryDTO> categories = await _serviceService.GetAllServiceCategories();

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


        // GET: api/service/{categoryId}
        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetServiceByCategory([FromRoute] int categoryId)
        {
            try
            {
                // Retrieve services by category from the service layer
                var services = await _serviceService.GetServicesByCategoryId(categoryId);

                if (services == null || services.Count == 0)
                {
                    // Return 404 Not Found if no services found
                    return NotFound("not found");
                }

                // Return the retrieved services
                return Ok(services);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut("{SoRId}")]
        [Authorize(Policy ="adminPolicy")]
        public async Task<IActionResult> ChangeSuspendState([FromRoute] int SORId)
        {
            try
            {
                var categoryId = await _serviceService.ChangeSuspendStateAsync(SORId);
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
        public async Task<IActionResult> DeleteService([FromRoute] int Id)
        {
            try
            {
                var deletedCategoryId = await _serviceService.DeleteServiceAsync(Id);
                if (deletedCategoryId == null)
                    return NotFound("Service not found.");

                return Ok(deletedCategoryId);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("deleteRequest")]
        [Authorize(Policy ="adminPolicy")]
        public async Task<IActionResult> GetCategoriesWithRequestToDelete()
        {
            try
            {
                var categoriesWithRequestToDelete = await _serviceService.GetCategoriesWithRequestToDeleteAsync();
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
        public async Task<IActionResult> DeleteRequestServices([FromRoute] int categoryId)
        {
            try
            {
                var services = await _serviceService.GetServicesWithRequestToDeleteAsync(categoryId);
                if (services == null)
                    return NotFound("not found");

                return Ok(services);
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
                var result = await _serviceService.ChangeDeleteRequestStateAsync(Id);

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
        [Authorize(Policy = "adminPolicy")]
        public async Task<IActionResult> ApproveVendorDeleteRequest([FromRoute] int Id)
        {
            try
            {
                var result = await _serviceService.ApproveVendorDeleteRequestAsync(Id);

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
        public async Task<IActionResult> GetAllServiceCategoriesOfVendor()
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

                var result = await _serviceService.GetAllServiceCategoriesOfVendorAsync(vendorId);

                if (result == null)
                    return NotFound("not found");

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
                var result = await _serviceService.RequestToDeleteAsync(SORId);
                if (result == 0)
                    return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("/api/vendorService/{categoryId}")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> GetVendorServiceByCategory([FromRoute] int categoryId)
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

                var result = await _serviceService.GetVendorServiceByCategoryAsync(categoryId, vendorId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("/api/bookedService/Categories")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> GetServiceCategoriesOfBookedServices()
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

                var result = await _serviceService.GetServiceCategoriesOfBookedServicesAsync(vendorId);

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


        [HttpGet("/api/bookedService/{categoryId}")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> GetBookedServicesOfVendor(int categoryId)
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

                var result = await _serviceService.GetBookedServicesOfVendorAsync(categoryId, vendorId);
                if (result == null)
                    return NotFound("not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("/api/bookingRequestService")]
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

                var result = await _serviceService.GetCategoriesOfBookingRequestAsync(vendorId);
                if (result == null)
                    return NotFound("not found");


                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("/api/bookingRequestService/{categoryId}")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> GetServicesOfBookingRequest(int categoryId)
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

                var result = await _serviceService.GetServicesOfBookingRequestAsync(categoryId, vendorId);
                if (result == null)
                    return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("/api/bookingRequestApproveService/{eventId}/{soRId}")]
        [Authorize(Policy = "VendorPolicy")]

        public async Task<IActionResult> BookServiceByVendor([FromRoute] int eventId, int soRId)
        {
            try
            {
                var result = await _serviceService.BookServiceByVendorAsync(eventId, soRId);
                if (!result) return NotFound("not found");

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("/api/bookingRequestRejectService/{eventId}/{soRId}")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> RejectServiceFromVendor([FromRoute] int eventId, int soRId)
        {
            try
            {
                var result = await _serviceService.RejectServiceFromVendorAsync(eventId, soRId);

                if (!result) return NotFound("not found");


                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("priceModels")]
        [Authorize]
        public async Task<IActionResult> GetAllPriceModels()
        {
            try
            {
                var result = await _serviceService.GetAllPriceModelsAsync();
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

        [HttpGet("priceModels/available")]
        public async Task<IActionResult> GetAvailablePriceModels()
        {
            try
            {
                var result = await _serviceService.GetAvailablePriceModelsAsync();
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


        [HttpPost("addNew")]
        [Authorize(Policy = "VendorPolicy")]
        public async Task<IActionResult> AddNewService([FromBody] object data)
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


                await _serviceService.AddNewServiceAsync(vendorId, data);
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update/{soRId}")]
        [Authorize(Policy ="vendorPolicy")]
        public async Task<IActionResult> UpdateService([FromRoute] int soRId, [FromBody] object data)
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

                await _serviceService.UpdateServiceAsync(vendorId, soRId, data);
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("maxPrice/{modelId}")]
        public async Task<IActionResult> GetMaxPriceOfService(int modelId)
        {
            try
            {
                var result = await _serviceService.GetMaxPriceOfServiceAsync(modelId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetServicesForClients([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string sortBy, [FromQuery] int? minPrice, [FromQuery] int? maxPrice, [FromQuery] int? modelId, [FromQuery] string categories, [FromQuery] int? rate)
        {
            try
            {
                var result = await _serviceService.GetServicesForClientsAsync(page, pageSize, sortBy, minPrice, maxPrice, modelId, categories, rate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("details/{soRId}")]
        public async Task<IActionResult> GetServiceDetailsForClient(int soRId)
        {
            try
            {
                var result = await _serviceService.GetServiceDetailsForClientAsync(soRId);
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
                var result = await _serviceService.GetRatingCountAsync();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
