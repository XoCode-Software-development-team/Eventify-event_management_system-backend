using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System.Text.Json;
using System.Xml.Linq;

namespace eventify_backend.Services
{
    public class ServiceService
    {
        private readonly AppDbContext _appDbContext;

        public ServiceService(AppDbContext appDbContext)
        {

            this._appDbContext = appDbContext;

        }


        public async Task<List<ServiceCategoryDTO>> GetAllServiceCategories()
        {
            try
            {
                // Retrieve all service categories from the database
                var categories = await _appDbContext.ServiceCategories
                    .Select(x => new ServiceCategoryDTO
                    {
                        CategoryId = x.CategoryId,
                        ServiceCategoryName = x.ServiceCategoryName
                    })
                    .ToListAsync();

                return categories;
            }

            catch (Exception ex)
            {
                // Handle exceptions and return an empty list or rethrow the exception
                throw new Exception($"Error occurred while fetching service categories: {ex.Message}");
            }
        }


        // Method to get services by category ID
        public async Task<List<ServiceDTO>> GetServicesByCategoryId(int categoryId)
        {
            try
            {
                // Retrieve services related to the specified category
                var servicesWithCategories = await _appDbContext.Services
                    .Include(s => s.ServiceCategory)
                    .Where(s => s.ServiceCategoryId == categoryId)
                    .Select(s => new ServiceDTO
                    {
                        SoRId = s.SoRId,
                        Service = s.Name,
                        Rating = s.OverallRate ?? 0, // Handle nullable rating
                        IsSuspend = s.IsSuspend,
                    })
                    .ToListAsync();

                return servicesWithCategories;
            }

            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                throw new Exception("Error occurred while fetching services by category.", ex);
            }
        }


        public async Task<int?> ChangeSuspendStateAsync(int SORId)
        {
            try
            {
                var service = await _appDbContext.ServiceAndResources.FindAsync(SORId);
                if (service == null)
                    return null;

                service.IsSuspend = !service.IsSuspend;
                await _appDbContext.SaveChangesAsync();

                var categoryId = await _appDbContext.Services
                    .Where(s => s.SoRId == service.SoRId)
                    .Select(s => s.ServiceCategoryId)
                    .FirstOrDefaultAsync();

                return categoryId;
            }

            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                throw new Exception("An error occurred.", ex);
            }
        }


        public async Task<int?> DeleteServiceAsync(int Id)
        {
            try
            {
                var service = await _appDbContext.Services.FindAsync(Id);
                if (service == null)
                    return null;

                var deletedCategoryId = service.ServiceCategoryId;
                _appDbContext.Services.Remove(service);
                await _appDbContext.SaveChangesAsync();

                return deletedCategoryId;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the service.", ex);
            }
        }


        public async Task<List<ServiceCategoryDTO>> GetCategoriesWithRequestToDeleteAsync()
        {
            try
            {
                var categoriesWithRequestToDelete = await _appDbContext.ServiceCategories
                    .Join(_appDbContext.Services.Where(s => s.IsRequestToDelete),
                        category => category.CategoryId,
                        service => service.ServiceCategoryId,
                        (category, service) => new ServiceCategoryDTO { CategoryId = category.CategoryId, ServiceCategoryName = category.ServiceCategoryName })
                    .Distinct()
                    .ToListAsync();

                return categoriesWithRequestToDelete;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving categories with services flagged for deletion.", ex);
            }
        }


        public async Task<List<ServiceDTO>> GetServicesWithRequestToDeleteAsync(int categoryId)
        {
            try
            {
                var services = await _appDbContext.Services
                    .Where(s => s.ServiceCategoryId == categoryId && s.IsRequestToDelete)
                    .Select(s => new ServiceDTO
                    {
                        SoRId = s.SoRId,
                        Service = s.Name,
                        Rating = s.OverallRate,
                    })
                    .ToListAsync();

                return services;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving services with requests to delete.", ex);
            }
        }


        public async Task<object?> ChangeDeleteRequestStateAsync(int Id)
        {
            try
            {
                var service = await _appDbContext.Services.FindAsync(Id);
                if (service == null)
                {
                    return null;
                }

                service.IsRequestToDelete = false; // Mark the service as no longer needing deletion
                await _appDbContext.SaveChangesAsync(); // Save changes to the database

                // Calculate the remaining count of services still requesting deletion
                var remainingCount = await _appDbContext.Services.CountAsync(s => s.ServiceCategoryId == service.ServiceCategoryId && s.IsRequestToDelete);

                return new { DeletedServiceCategoryId = service.ServiceCategoryId, RemainingCount = remainingCount };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while changing delete request state of the service.", ex);
            }
        }


        public async Task<object?> ApproveVendorDeleteRequestAsync(int Id)
        {
            try
            {
                var service = await _appDbContext.Services.FindAsync(Id);
                if (service == null)
                {
                    return null;
                }

                var deletedCategory = service.ServiceCategoryId; // Save the category ID before deletion
                _appDbContext.Services.Remove(service); // Remove the service from the database
                await _appDbContext.SaveChangesAsync(); // Save changes to the database

                // Calculate the remaining count of services still requesting deletion
                var remainingCount = await _appDbContext.Services.CountAsync(s => s.ServiceCategoryId == deletedCategory && s.IsRequestToDelete);

                return new { DeletedServiceCategoryId = service.ServiceCategoryId, RemainingCount = remainingCount };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving vendor delete request for the service.", ex);
            }
        }


        public async Task<List<ServiceCategoryDTO>> GetAllServiceCategoriesOfVendorAsync(Guid Id)
        {
            try
            {
                var categories = await _appDbContext.ServiceCategories
                    .Where(sc => sc.Services != null && sc.Services.Any(s => s.VendorId == Id))
                    .Select(x => new ServiceCategoryDTO { CategoryId = x.CategoryId, ServiceCategoryName = x.ServiceCategoryName })
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving service categories associated with the vendor.", ex);
            }
        }
            

        public async Task<int> RequestToDeleteAsync(int SORId)
        {
            try
            {
                var service = await _appDbContext.Services.FindAsync(SORId);
                if (service == null)
                {
                    throw new Exception("Service not found.");
                }

                // Toggle the IsRequestToDelete flag
                service.IsRequestToDelete = !service.IsRequestToDelete;

                await _appDbContext.SaveChangesAsync();

                return service.ServiceCategoryId;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }


        public async Task<object> GetVendorServiceByCategoryAsync(int categoryId, Guid vendorId)
        {
            try
            {
                var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);
                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (serviceCategory == null || vendor == null)
                {
                    throw new Exception("Service category or vendor not found.");
                }

                var servicesWithCategories = await _appDbContext.Services
                    .Where(s => s.ServiceCategoryId == categoryId && s.VendorId == vendorId)
                    .Select(s => new
                    {
                        SoRId = s.SoRId,
                        Service = s.Name,
                        Rating = s.OverallRate,
                        IsRequestToDelete = s.IsRequestToDelete,
                    })
                    .ToListAsync();

                return servicesWithCategories;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }


        public async Task<object> GetServiceCategoriesOfBookedServicesAsync(Guid Id)
        {
            try
            {
                var currentDate = DateTime.Now;

                var categories = await _appDbContext.ServiceCategories
                    .Where(sc => sc.Services != null && sc.Services.Any(s => s.EventSRs != null &&
                        s.EventSRs.Any(esr => esr.Event != null && esr.ServiceAndResource != null &&
                            esr.Event.EndDateTime > currentDate && esr.ServiceAndResource.VendorId == Id)))
                    .Select(x => new { x.CategoryId, x.ServiceCategoryName })
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }


        public async Task<object> GetBookedServicesOfVendorAsync(int categoryId, Guid vendorId)
        {
            try
            {
                var currentDate = DateTime.Now;

                var services = await _appDbContext.Services
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

                return services;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }


        public async Task<object> GetCategoriesOfBookingRequestAsync(Guid vendorId)
        {
            try
            {
                var categories = await _appDbContext.ServiceCategories
                    .Where(sc => sc.Services != null && sc.Services.Any(s => s.EventSoRApproves != null && s.VendorId == vendorId && s.EventSoRApproves
                        .Any(esra => esra.IsRequest == true)))
                    .Select(x => new { x.CategoryId, x.ServiceCategoryName })
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }


        public async Task<object?> GetServicesOfBookingRequestAsync(int categoryId, Guid vendorId)
        {
            try
            {
                var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);
                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (serviceCategory == null || vendor == null)
                {
                    return false;
                }

                var currentDate = DateTime.Now;

                var services = await _appDbContext.Services
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

                return services;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }


        public async Task<bool> BookServiceByVendorAsync(int eventId, int soRId)
        {
            try
            {
                var eventSorToApprove = await _appDbContext.EventSoRApproves.FindAsync(eventId, soRId);
                if (eventSorToApprove == null)
                {
                    return false;
                }

                var eventSR = new EventSR
                {
                    Id = eventId,
                    SORId = soRId,
                };

                await _appDbContext.EventSr.AddAsync(eventSR);
                _appDbContext.EventSoRApproves.Remove(eventSorToApprove);
                await _appDbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }


        public async Task<bool> RejectServiceFromVendorAsync(int eventId, int soRId)
        {
            try
            {
                var eventSorToApprove = await _appDbContext.EventSoRApproves.FindAsync(eventId, soRId);
                if (eventSorToApprove == null)
                {
                    return false;
                }

                eventSorToApprove.IsRequest = !eventSorToApprove.IsRequest;
                await _appDbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }


        public async Task<List<PriceModelDto>> GetAllPriceModelsAsync()
        {
            try
            {
                var priceModels = await _appDbContext.PriceModels
                    .Select(x => new PriceModelDto { ModelId = x.ModelId, ModelName = x.ModelName })
                    .ToListAsync();

                return priceModels;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }


        public async Task AddNewService(Guid vendorId, object data)
        {
            try
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data), "No data provided.");
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

                _appDbContext.Services.Add(service);

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

                // Similar handling for other entities
                await _appDbContext.SaveChangesAsync();
            }
            catch (ArgumentNullException)
            {
                throw; // Rethrow to maintain original behavior
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Invalid JSON format.", nameof(data), ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred.", ex);
            }
        }


        public async Task<object> GetMaxPriceOfServiceAsync()
        {
            try
            {
                var maxServicePrice = await (
                    from service in _appDbContext.Services
                    join vendorSRPrice in _appDbContext.VendorSRPrices on service.SoRId equals vendorSRPrice.SoRId
                    join price in _appDbContext.Prices on vendorSRPrice.PId equals price.Pid
                    select price.BasePrice
                ).MaxAsync();

                return maxServicePrice;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting max price.", ex);
            }
        }


        public async Task<object> GetServicesForClientsAsync()
        {
            try
            {
                var services = await _appDbContext.Services
                    .Select(s => new
                    {
                        soRId = s.SoRId,
                        name = s.Name,
                        rating = new
                        {
                            rate = s.OverallRate,
                            count = s.ReviewAndRating != null ? s.ReviewAndRating.Select(r => r.EventId).Count() : 0,

                        },
                        vendor = s.Vendor != null ? s.Vendor.CompanyName : null,
                        description = s.Description,
                        Images = s.VendorRSPhotos != null ? s.VendorRSPhotos.Select(photo => photo.Image).ToList() : new List<string?>()

                    }).ToListAsync();

                return services;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting services", ex);
            }
        }


        public async Task<object> GetServiceDetailsForClientAsync(int soRId)
        {
            try
            {
                var service = await _appDbContext.Services
                    .Select(s => new
                    {
                        name = s.Name,
                        soRId = soRId,
                        vendor = new
                        {
                            vendorId = s.VendorId,
                            companyName = s.Vendor != null ? s.Vendor.CompanyName : null,
                        },
                        capacity = s.Capacity,
                        description = s.Description,
                        reviewAndRating = s.ReviewAndRating
                            .Select(rr => new
                            {
                                avatar = rr.Event != null && rr.Event.Client != null ? rr.Event.Client.ProfilePic : null,
                                fname = rr.Event != null && rr.Event.Client != null ? rr.Event.Client.FirstName : null,
                                lname = rr.Event != null && rr.Event.Client != null ? rr.Event.Client.LastName : null, 

                            })
                    })
                    .ToListAsync();

                return service;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred whiel getting service details", ex);
            }

        }
    }
}
