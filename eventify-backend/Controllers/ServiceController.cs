using eventify_backend.Data;
using eventify_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;


namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {

        private readonly AppDbContext _appDbContext;


        // Constructor injection of AppDbContext
        public ServiceController(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;

        }

        // Endpoint to get all service categories

        [HttpGet("/api/[Controller]/categories")]
        public async Task<IActionResult> GetAllServiceCategories()
        {
            try
            {
                // Retrieve all service categories
                var categories = await _appDbContext.ServiceCategories
                    .Select(x => new { x.CategoryId, x.ServiceCategoryName })
                    .ToListAsync();


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

        [HttpGet("/api/[Controller]/{categoryId}")]
        public async Task<IActionResult> GetServiceByCategory([FromRoute] int categoryId)
        {
            try
            {
                // Find the service category based on the provided categoryId
                var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);

                if (serviceCategory == null)
                {
                    // Return a 404 Not Found response if the category doesn't exist
                    return NotFound();
                }

                // Retrieve services related to the specified category
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

                // Return the services along with their relevant information
                return Ok(servicesWithCategories);
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
                // Find the service associated with the specified SoRId
                var service = await _appDbContext.ServiceAndResources.FindAsync(SORId);
                if (service == null)
                {
                    // Return a 404 Not Found response if the service doesn't exist
                    return NotFound();
                }

                // Toggle the IsSuspend flag
                service.IsSuspend = !service.IsSuspend;

                // Save changes to the database
                await _appDbContext.SaveChangesAsync();

                // Retrieve the service category ID for further processing
                var categoryId = await _appDbContext.services
                    .Where(s => s.SoRId == service.SoRId)
                    .Select(s => s.ServiceCategoryId)
                    .FirstOrDefaultAsync();

                // Return the category ID
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
                // Find the service associated with the specified Id
                var service = await _appDbContext.services.FindAsync(Id);
                if (service == null)
                {
                    // Return a 404 Not Found response if the service doesn't exist
                    return NotFound();
                }

                // Save the category ID before deletion
                var deletedCategoryId = service.ServiceCategoryId;

                // Remove the service from the database
                _appDbContext.services.Remove(service);
                await _appDbContext.SaveChangesAsync();

                // Return the deleted category ID
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
                // Join ServiceCategories with services having IsRequestToDelete flag
                var categoriesWithRequestToDelete = await _appDbContext.ServiceCategories
                    .Join(_appDbContext.services.Where(s => s.IsRequestToDelete),
                        category => category.CategoryId,
                        service => service.ServiceCategoryId,
                        (category, service) => new { category.CategoryId, category.ServiceCategoryName })
                    .Distinct()
                    .ToListAsync();

                if (categoriesWithRequestToDelete.Count == 0)
                {
                    // Return an empty list if no categories match the criteria
                    return Ok(categoriesWithRequestToDelete);
                }

                // Return the list of categories with requests to delete
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
                // Retrieve services with the specified category ID and IsRequestToDelete flag
                var services = await _appDbContext.services
                    .Where(s => s.ServiceCategoryId == categoryId && s.IsRequestToDelete)
                    .Select(s => new
                    {
                        SoRId = s.SoRId,
                        ServiceName = s.Name,
                        Rating = s.OverallRate,
                    })
                    .ToListAsync();

                if (services == null || services.Count == 0)
                {
                    // Return an empty list if no services match the criteria
                    return Ok(services);
                }

                // Return the list of services with requests to delete
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
                // Find the service associated with the specified Id
                var service = await _appDbContext.services.FindAsync(Id);
                if (service == null)
                {
                    // Return a 404 Not Found response if the service doesn't exist
                    return NotFound();
                }

                // Mark the service as no longer needing deletion
                service.IsRequestToDelete = false;

                // Save changes to the database
                await _appDbContext.SaveChangesAsync();

                // Calculate the remaining count of services still requesting deletion
                var remainingCount = await _appDbContext.services.CountAsync(s => s.ServiceCategoryId == service.ServiceCategoryId && s.IsRequestToDelete);

                // Return the deleted service's category ID and the remaining count
                return Ok(new { DeletedServiceCategoryId = service.ServiceCategoryId, RemainingCount = remainingCount });
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
                // Find the service associated with the specified Id
                var service = await _appDbContext.services.FindAsync(Id);
                if (service == null)
                {
                    // Return a 404 Not Found response if the service doesn't exist
                    return NotFound();
                }

                // Save the category ID before deletion
                var deletedCategory = service.ServiceCategoryId;

                // Remove the service from the database
                _appDbContext.services.Remove(service);
                await _appDbContext.SaveChangesAsync();

                // Calculate the remaining count of services still requesting deletion
                var remainingCount = await _appDbContext.services.CountAsync(s => s.ServiceCategoryId == deletedCategory && s.IsRequestToDelete);

                // Return the deleted service's category ID and the remaining count
                return Ok(new { DeletedServiceCategoryId = service.ServiceCategoryId, RemainingCount = remainingCount });
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
                // Retrieve service categories associated with the specified vendor Id
                var categories = await _appDbContext.ServiceCategories
                    .Where(sc => sc.Services != null && sc.Services.Any(s => s.VendorId == Id))
                    .Select(x => new { x.CategoryId, x.ServiceCategoryName })
                    .ToListAsync();

                if (categories == null || categories.Count == 0)
                {
                    // Return an empty list if no categories match the criteria
                    return Ok(categories);
                }

                // Return the list of service categories
                return Ok(categories);
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
                // Find the service associated with the specified SoRId
                var service = await _appDbContext.services.FindAsync(SORId);
                if (service == null)
                {
                    // Return a 404 Not Found response if the service doesn't exist
                    return NotFound();
                }

                // Toggle the IsRequestToDelete flag
                service.IsRequestToDelete = !service.IsRequestToDelete;

                // Save changes to the database
                await _appDbContext.SaveChangesAsync();

                // Return the service's category ID
                return Ok(service.ServiceCategoryId);
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
                // Find the service category associated with the specified categoryId
                var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);

                // Find the vendor associated with the specified vendorId
                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (serviceCategory == null || vendor == null)
                {
                    // Return a 404 Not Found response if either the category or vendor doesn't exist
                    return NotFound();
                }

                // Retrieve services related to the specified category and vendor
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

                // Return the list of services along with their relevant information
                return Ok(servicesWithCategories);
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
                // Get the current date and time
                var currentDate = DateTime.Now;

                // Retrieve service categories associated with booked services for the specified vendor Id
                var categories = await _appDbContext.ServiceCategories
                    .Where(sc => sc.Services != null && sc.Services.Any(s => s.EventSRs != null &&
                        s.EventSRs.Any(esr => esr.Event != null && esr.ServiceAndResource != null &&
                            esr.Event.EndDateTime > currentDate && esr.ServiceAndResource.VendorId == Id)))
                    .Select(x => new { x.CategoryId, x.ServiceCategoryName })
                    .ToListAsync();

                if (categories == null || categories.Count == 0)
                {
                    // Return an empty list if no categories match the criteria
                    return Ok(categories);
                }

                // Return the list of service categories
                return Ok(categories);
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
                // Find the service category associated with the specified categoryId
                var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);

                // Find the vendor associated with the specified vendorId
                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (serviceCategory == null || vendor == null)
                {
                    // Return a 404 Not Found response if either the category or vendor doesn't exist
                    return NotFound();
                }

                // Get the current date and time
                var currentDate = DateTime.Now;

                // Retrieve services related to the specified category and vendor
                var services = await _appDbContext.services
                    .Where(s => s.ServiceCategoryId == categoryId &&
                                s.VendorId == vendorId &&
                                s.EventSRs != null &&
                                s.EventSRs.Any(e => e.Event != null && e.Event.EndDateTime > currentDate))
                    .Select(x => new
                    {
                        SoRId = x.SoRId,
                        Service = x.Name,
                        EventDate = x.EventSRs != null ? x.EventSRs.Select(e => e.Event != null ? e.Event.StartDateTime.Date.ToString("yyyy-MM-dd") : DateTime.MinValue.ToString("yyyy-MM-dd")).ToList() : null,
                        EndDate = x.EventSRs != null ? x.EventSRs.Select(e => e.Event != null ? e.Event.EndDateTime.Date.ToString("yyyy-MM-dd") : DateTime.MinValue.ToString("yyyy-MM-dd")).ToList() : null
                    })
                    .ToListAsync();

                // Return the list of services along with their relevant information
                return Ok(services);
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
                // Retrieve service categories associated with booking requests for the specified vendor Id
                var categories = await _appDbContext.ServiceCategories
                    .Where(sc => sc.Services != null && sc.Services.Any(s => s.EventSoRApproves != null && s.VendorId == vendorId && s.EventSoRApproves
                        .Any(esra => esra.IsRequest == true)))
                    .Select(x => new { x.CategoryId, x.ServiceCategoryName })
                    .ToListAsync();

                if (categories == null || categories.Count == 0)
                {
                    // Return an empty list if no categories match the criteria
                    return Ok(categories);
                }

                // Return the list of service categories
                return Ok(categories);
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
                // Find the service category associated with the specified categoryId
                var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);

                // Find the vendor associated with the specified vendorId
                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (serviceCategory == null || vendor == null)
                {
                    // Return a 404 Not Found response if either the category or vendor doesn't exist
                    return NotFound();
                }

                // Get the current date and time
                var currentDate = DateTime.Now;

                // Retrieve services related to the specified category and vendor
                var services = await _appDbContext.services
                    .Where(s => s.ServiceCategoryId == categoryId &&
                                s.VendorId == vendorId &&
                                s.EventSoRApproves != null &&
                                s.EventSoRApproves.Any(e => e.IsRequest == true))
                    .Select(x => new
                    {
                        SoRId = x.SoRId,
                        EventId = x.EventSoRApproves != null ? x.EventSoRApproves.Select(e => e.EventId) : null,
                        Service = x.Name,
                        EventName = x.EventSoRApproves != null ? x.EventSoRApproves.Select(e => e.Event != null ? e.Event.Name : null) : null,
                        EventDate = x.EventSoRApproves != null ? x.EventSoRApproves.Select(e => e.Event != null ? e.Event.StartDateTime.Date.ToString("yyyy-MM-dd") : DateTime.MinValue.ToString("yyyy-MM-dd")).ToList() : null,
                        EndDate = x.EventSoRApproves != null ? x.EventSoRApproves.Select(e => e.Event != null ? e.Event.EndDateTime.Date.ToString("yyyy-MM-dd") : DateTime.MinValue.ToString("yyyy-MM-dd")).ToList() : null
                    })
                    .ToListAsync();

                // Return the list of services along with their relevant information
                return Ok(services);
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
                // Find the EventSoRApprove associated with the specified eventId and soRId
                var eventSorToApprove = await _appDbContext.EventSoRApproves.FindAsync(eventId, soRId);
                if (eventSorToApprove == null)
                {
                    // Return a 404 Not Found response if the EventSoRApprove doesn't exist
                    return NotFound();
                }

                // Create a new EventSR
                var EventSR = new EventSR
                {
                    Id = eventId,
                    SORId = soRId,
                };

                // Add the new EventSR to the database
                await _appDbContext.EventSr.AddAsync(EventSR);

                // Remove the existing EventSoRApprove
                _appDbContext.EventSoRApproves.Remove(eventSorToApprove);

                // Save changes to the database
                await _appDbContext.SaveChangesAsync();

                // Return a successful response
                return Ok();
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
                // Find the EventSoRApprove associated with the specified eventId and soRId
                var eventSorToApprove = await _appDbContext.EventSoRApproves.FindAsync(eventId, soRId);
                if (eventSorToApprove == null)
                {
                    // Return a 404 Not Found response if the EventSoRApprove doesn't exist
                    return NotFound();
                }

                // Toggle the IsRequest flag
                eventSorToApprove.IsRequest = !eventSorToApprove.IsRequest;

                // Save changes to the database
                await _appDbContext.SaveChangesAsync();

                // Return a successful response
                return Ok();
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
                // Retrieve all price models along with their Id and Name
                var priceModels = await _appDbContext.PriceModels
                    .Select(x => new { x.ModelId, x.ModelName })
                    .ToListAsync();

                if (priceModels == null || priceModels.Count == 0)
                {
                    // Return a 404 Not Found response if no price models exist
                    return NotFound();
                }

                // Return the list of price models
                return Ok(priceModels);
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
                if (data == null)
                {
                    // Return a 400 Bad Request response if no data is provided
                    return BadRequest("No data provided.");
                }

                string jsonString = data?.ToString() ?? string.Empty;

                JObject json = JObject.Parse(jsonString);

                var service = new Service
                {
                    Name = json["serviceName"]?.ToString(),
                    Description = json["serviceDescription"]?.ToString(),
                    IsSuspend = false,
                    IsRequestToDelete = false,
                    VendorId = vendorId,
                    ServiceCategoryId = json["serviceCategory"]?.Value<int>() ?? 0,
                    Capacity = json["serviceMaxCapacity"]?.Value<int>() ?? 0,
                };

                _appDbContext.services.Add(service);

                // Save changes to get the SoRId assigned
                await _appDbContext.SaveChangesAsync();

                var featureAndFacility = json["serviceFeatures"] as JArray;
                if (featureAndFacility != null)
                {
                    foreach (var item in featureAndFacility)
                    {
                        var featureOrFacility = new FeatureAndFacility
                        {
                            FacilityName = item["name"]?.ToString(),
                            SoRId = service.SoRId
                        };
                        _appDbContext.FeatureAndFacility.Add(featureOrFacility);
                    }
                }

                var location = json["serviceLocations"] as JArray;

                if (location != null)
                {
                    foreach (var item in location)
                    {
                        var vendorSRLocation = new VendorSRLocation
                        {
                            SoRId = service.SoRId,
                            HouseNo = item["houseNoStreetRoad"]?.ToString(),
                            Area = item["cityTownArea"]?.ToString(),
                            District = item["district"]?.ToString(),
                            Country = item["country"]?.ToString(),
                            State = item["stateProvinceRegion"]?.ToString(),
                        };

                        _appDbContext.VendorSRLocation.Add(vendorSRLocation);

                    }
                }

                var price = json["servicePricePackages"] as JArray;

                if (price != null)
                {
                    foreach (var item in price)
                    {
                        var servicePrice = new Price
                        {
                            Pname = item["packageName"]?.ToString(),
                            BasePrice = item["basePrice"]?.Value<double>() ?? 0,
                            ModelId = item["priceModel"]?.Value<int>() ?? 0
                        };

                        _appDbContext.Prices.Add(servicePrice);
                        await _appDbContext.SaveChangesAsync();


                        var vendorSRPrice = new VendorSRPrice
                        {
                            SoRId = service.SoRId,
                            PId = servicePrice.Pid
                        };

                        _appDbContext.VendorSRPrices.Add(vendorSRPrice);
                    }
                }

                var images = json["images"] as JArray;

                if (images != null)
                {
                    foreach (var image in images)
                    {
                        var vendorSRPhoto = new VendorSRPhoto
                        {
                            SoRId = service.SoRId,
                            Image = image.ToString()
                        };

                        _appDbContext.VendorSRPhoto.Add(vendorSRPhoto);
                    }
                }

                var videos = json["videos"] as JArray;

                if (videos != null)
                {
                    foreach(var video in videos)
                    {
                        var vendorSRVideo = new VendorSRVideo
                        {
                            SoRId = service.SoRId,
                            Video = video.ToString()
                        };

                        _appDbContext.VendorSRVideo.Add(vendorSRVideo);
                    }
                }


                await _appDbContext.SaveChangesAsync();

                // Return the newly added service
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


    }
}
