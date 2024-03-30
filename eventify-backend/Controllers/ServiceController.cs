using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Services;
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

        [HttpGet("/api/[Controller]/categories")]
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
        [HttpGet("/api/[Controller]/{categoryId}")]
        public async Task<IActionResult> GetServiceByCategory([FromRoute] int categoryId)
        {
            try
            {
                // Retrieve services by category from the service layer
                var services = await _serviceService.GetServicesByCategoryId(categoryId);

                if (services == null || services.Count == 0)
                {
                    // Return 404 Not Found if no services found
                    return NotFound();
                }

                // Return the retrieved services
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


        [HttpDelete("/api/[Controller]/{Id}")]
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

        [HttpGet("/api/deleteRequestServices")]
        public async Task<IActionResult> GetCategoriesWithRequestToDelete()
        {
            try
            {
                var categoriesWithRequestToDelete = await _serviceService.GetCategoriesWithRequestToDeleteAsync();

                return Ok(categoriesWithRequestToDelete);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("/api/deleteRequestServices/{categoryId}")]
        public async Task<IActionResult> DeleteRequestServices([FromRoute] int categoryId)
        {
            try
            {
                var services = await _serviceService.GetServicesWithRequestToDeleteAsync(categoryId);

                return Ok(services);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut("/api/deleteRequestServices/{Id}")]
        public async Task<IActionResult> ChangeDeleteRequestState([FromRoute] int Id)
        {
            try
            {
                var result = await _serviceService.ChangeDeleteRequestStateAsync(Id);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpDelete("/api/deleteRequestServices/{Id}")]
        public async Task<IActionResult> ApproveVendorDeleteRequest([FromRoute] int Id)
        {
            try
            {
                var result = await _serviceService.ApproveVendorDeleteRequestAsync(Id);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("/api/[Controller]/categories/{Id}")]
        public async Task<IActionResult> GetAllServiceCategoriesOfVendor(Guid Id)
        {
            try
            {
                var result = await _serviceService.GetAllServiceCategoriesOfVendorAsync(Id);
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
                var result = await _serviceService.RequestToDeleteAsync(SORId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("/api/vendorService/{categoryId}/{vendorId}")]
        public async Task<IActionResult> GetVendorServiceByCategory([FromRoute] int categoryId, Guid vendorId)
        {
            try
            {
                var result = await _serviceService.GetVendorServiceByCategoryAsync(categoryId, vendorId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("/api/bookedService/Categories/{Id}")]
        public async Task<IActionResult> GetServiceCategoriesOfBookedServices(Guid Id)
        {
            try
            {
                var result = await _serviceService.GetServiceCategoriesOfBookedServicesAsync(Id);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("/api/bookedService/{categoryId}/{vendorId}")]
        public async Task<IActionResult> GetBookedServicesOfVendor(int categoryId, Guid vendorId)
        {
            try
            {
                var result = await _serviceService.GetBookedServicesOfVendorAsync(categoryId, vendorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("/api/bookingRequest/{vendorId}")]
        public async Task<IActionResult> GetCategoriesOfBookingRequest(Guid vendorId)
        {
            try
            {
                var result = await _serviceService.GetCategoriesOfBookingRequestAsync(vendorId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("/api/bookingRequest/{categoryId}/{vendorId}")]
        public async Task<IActionResult> GetServicesOfBookingRequest(int categoryId, Guid vendorId)
        {
            try
            {
                var result = await _serviceService.GetServicesOfBookingRequestAsync(categoryId, vendorId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("/api/bookingRequestApprove/{eventId}/{soRId}")]
        public async Task<IActionResult> BookServiceByVendor([FromRoute] int eventId, int soRId)
        {
            try
            {
                var result = await _serviceService.BookServiceByVendorAsync(eventId, soRId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("/api/bookingRequestReject/{eventId}/{soRId}")]
        public async Task<IActionResult> RejectServiceFromVendor([FromRoute] int eventId, int soRId)
        {
            try
            {
                var result = await _serviceService.RejectServiceFromVendorAsync(eventId, soRId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("/api/[Controller]/priceModels")]
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


        [HttpPost("/api/[Controller]/addNew/{vendorId}")]
        public async Task<IActionResult> AddNewService([FromRoute] Guid vendorId, [FromBody] object data)
        {
            try
            {
                await _serviceService.AddNewService(vendorId, data);
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("/api/[Controller]/maxPrice")]
        public async Task<IActionResult> GetMaxPriceOfService()
        {
            try
            {
                var result = await _serviceService.GetMaxPriceOfServiceAsync();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("/api/[Controller]/all")]
        public async Task<IActionResult> GetServicesForClients()
        {
            try
            {
                var result = await _serviceService.GetServicesForClientsAsync();
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("/api/[Controller]/details/{soRId}")]
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
    }
}
