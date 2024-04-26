using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;

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
                var service = await _appDbContext.ServiceAndResources.FindAsync(SORId);  // Find the service by ID

                if (service == null)
                    return null;

                service.IsSuspend = !service.IsSuspend; // Toggle the suspend state
                await _appDbContext.SaveChangesAsync();


                // Get the category ID of the service
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
                var service = await _appDbContext.Services.FindAsync(Id);    // Find the service by ID

                if (service == null)
                    return null;

                // Find all PId values associated with the given SoRId
                var pIdsToDelete = _appDbContext.VendorSRPrices
                    .Where(vp => vp.SoRId == service.SoRId)
                    .Select(vp => vp.PId)
                    .ToList();

                if (pIdsToDelete != null)
                {
                    // Remove all entries from the Price table with PIds found above
                    var pricesToDelete = _appDbContext.Prices.Where(p => pIdsToDelete.Contains(p.Pid));
                    _appDbContext.Prices.RemoveRange(pricesToDelete);
                }

                var deletedCategoryId = service.ServiceCategoryId;
                _appDbContext.Services.Remove(service);               // Remove the service from the databas

                await _appDbContext.SaveChangesAsync();

                return deletedCategoryId; // return category id of deleted service
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
                // Query to join ServiceCategories with Services flagged for deletion and select relevant data into ServiceCategoryDTO objects

                var categoriesWithRequestToDelete = await _appDbContext.ServiceCategories
                    .Join(_appDbContext.Services.Where(s => s.IsRequestToDelete),
                        category => category.CategoryId,
                        service => service.ServiceCategoryId,
                        (category, service) => new ServiceCategoryDTO { CategoryId = category.CategoryId, ServiceCategoryName = category.ServiceCategoryName })
                    .Distinct()
                    .ToListAsync();

                return categoriesWithRequestToDelete;         // Return the list of categories with services flagged for deletion
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
                // Query to retrieve services within the specified category with requests to delete

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


        public async Task<object?> ChangeDeleteRequestStateAsync(int soRId)
        {
            try
            {
                var service = await _appDbContext.Services.FindAsync(soRId); // Find service by soRId
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


        public async Task<object?> ApproveVendorDeleteRequestAsync(int soRId)
        {
            try
            {
                var service = await _appDbContext.Services.FindAsync(soRId);
                if (service == null)
                {
                    return null;
                }

                // Find all PId values associated with the given SoRId
                var pIdsToDelete = _appDbContext.VendorSRPrices
                    .Where(vp => vp.SoRId == service.SoRId)
                    .Select(vp => vp.PId)
                    .ToList();


                if (pIdsToDelete != null)
                {
                    // Remove all entries from the Price table with PIds found above
                    var pricesToDelete = _appDbContext.Prices.Where(p => pIdsToDelete.Contains(p.Pid));
                    _appDbContext.Prices.RemoveRange(pricesToDelete);
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
                // Retrieve the service category and vendor

                var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);
                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (serviceCategory == null || vendor == null)
                {
                    throw new Exception("Service category or vendor not found.");
                }

                // Query to retrieve services within the specified category and vendor

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
                // Get the current date and time

                var currentDate = DateTime.Now;

                // Query to retrieve service categories of booked services for the specified vendor
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
                // Get the current date and time
                var currentDate = DateTime.Now;

                // Query to retrieve booked services of the vendor for the specified service category
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
                // Query to retrieve categories of booking requests for the vendor
                var categories = await _appDbContext.ServiceCategories
                    .Where(sc => sc.Services != null && sc.Services.Any(s => s.EventSoRApproves != null && s.VendorId == vendorId && s.EventSoRApproves
                        .Any(esra => esra.IsApprove == false)))
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
                // Retrieve the service category and vendor
                var serviceCategory = _appDbContext.ServiceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);
                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (serviceCategory == null || vendor == null)
                {
                    return false;
                }

                var currentDate = DateTime.Now;

                // Query to retrieve services with booking requests for the specified category and vendor
                var services = await _appDbContext.Services
                    .Where(s => s.ServiceCategoryId == categoryId &&
                                s.VendorId == vendorId &&
                                s.EventSoRApproves != null &&
                                s.EventSoRApproves.Any(e => e.IsApprove == false))
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
                // Find the event-SOR approval record
                var eventSorToApprove = await _appDbContext.EventSoRApproves.FindAsync(eventId, soRId);
                if (eventSorToApprove == null)
                {
                    return false;
                }

                eventSorToApprove.IsApprove = true;

                // Create a new event-SR record
                var eventSR = new EventSR
                {
                    Id = eventId,
                    SORId = soRId,
                };

                // Add the new event-SR record to the context
                await _appDbContext.EventSr.AddAsync(eventSR);
                //_appDbContext.EventSoRApproves.Remove(eventSorToApprove);
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
                // Find the event-SOR approval record
                var eventSorToApprove = await _appDbContext.EventSoRApproves.FindAsync(eventId, soRId);
                if (eventSorToApprove == null)
                {
                    return false;
                }

                eventSorToApprove.IsApprove = !eventSorToApprove.IsApprove;    // Toggle the approval status

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
                // Retrieve all price models from the database
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


        public async Task AddNewServiceAsync(Guid vendorId, object data)
        {
            try
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data), "No data provided.");
                }

                string jsonString = data?.ToString() ?? string.Empty;         // Convert data to JSON string

                JObject json = JObject.Parse(jsonString);         // Parse JSON string to JObject


                // Create new Service object
                var service = new Service
                {
                    Name = json["serviceName"]?.ToString(),
                    Description = json["serviceDescription"]?.ToString(),
                    IsSuspend = false,
                    IsRequestToDelete = false,
                    VendorId = vendorId,
                    ServiceCategoryId = json["serviceCategory"]?.Value<int>() ?? 0,
                };

                if (json["serviceMaxCapacity"] != null && int.TryParse(json["serviceMaxCapacity"]?.ToString(), out int capacity))
                {
                    // Parsing successful, assign the parsed value to Capacity
                    service.Capacity = capacity;
                }

                // Add service to the database
                _appDbContext.Services.Add(service);

                await _appDbContext.SaveChangesAsync();   // Save changes to the database


                // Handle feature and facility
                var featureAndFacility = json["serviceFeatures"] as JArray;// Extract feature and facility information from the JSON data
                if (featureAndFacility != null)
                {
                    foreach (var item in featureAndFacility)
                    {
                        // Create a new FeatureAndFacility object
                        var featureOrFacility = new FeatureAndFacility
                        {
                            FacilityName = item["name"]?.ToString(),
                            SoRId = service.SoRId
                        };
                        _appDbContext.FeatureAndFacility.Add(featureOrFacility);
                    }
                }

                var location = json["serviceLocations"] as JArray; // Extract location information from the JSON data

                if (location != null)
                {
                    foreach (var item in location)
                    {
                        var vendorSRLocation = new VendorSRLocation   // Create a new VendorSRLocation object
                        {
                            SoRId = service.SoRId,
                            HouseNo = item["houseNoStreetRoad"]?.ToString(),
                            Area = item["cityTownArea"]?.ToString(),
                            District = item["district"]?.ToString(),
                            Country = item["country"]?.ToString(),
                            State = item["stateProvinceRegion"]?.ToString(),
                        };

                        // Add the location to the database context
                        _appDbContext.VendorSRLocation.Add(vendorSRLocation);

                    }
                }

                var price = json["servicePricePackages"] as JArray;// Extract price information from the JSON data

                if (price != null)
                {
                    // Iterate over each price package item
                    foreach (var item in price)
                    {
                        var servicePrice = new Price
                        {
                            Pname = item["packageName"]?.ToString(),
                            BasePrice = item["basePrice"]?.Value<double>() ?? 0,
                            ModelId = item["priceModel"]?.Value<int>() ?? 0
                        };

                        // Add the price to the database context
                        _appDbContext.Prices.Add(servicePrice);
                        await _appDbContext.SaveChangesAsync();


                        var vendorSRPrice = new VendorSRPrice   // Create a new VendorSRPrice object
                        {
                            SoRId = service.SoRId,
                            PId = servicePrice.Pid
                        };

                        // Add the vendor service price to the database context
                        _appDbContext.VendorSRPrices.Add(vendorSRPrice);
                    }
                }

                // Extract image information from the JSON data
                var images = json["images"] as JArray;

                if (images != null)
                {
                    foreach (var image in images)
                    {
                        // Create a new VendorSRPhoto object
                        var vendorSRPhoto = new VendorSRPhoto
                        {
                            SoRId = service.SoRId,
                            Image = image.ToString()
                        };

                        _appDbContext.VendorSRPhoto.Add(vendorSRPhoto);
                    }
                }

                // Extract video information from the JSON data
                var videos = json["videos"] as JArray;

                if (videos != null)
                {
                    foreach (var video in videos)
                    {
                        var vendorSRVideo = new VendorSRVideo
                        {
                            SoRId = service.SoRId,
                            Video = video.ToString()
                        };

                        // Add the video to the database context
                        _appDbContext.VendorSRVideo.Add(vendorSRVideo);
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


        public async Task<object> GetMaxPriceOfServiceAsync(int modelId)
        {
            try
            {
                // Retrieve the maximum service price for the specified model ID
                var maxServicePrice = await (
                    from service in _appDbContext.Services
                    join vendorSRPrice in _appDbContext.VendorSRPrices on service.SoRId equals vendorSRPrice.SoRId
                    join price in _appDbContext.Prices on vendorSRPrice.PId equals price.Pid
                    where price.ModelId == modelId
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
                // Retrieve services for clients that are not suspended
                var services = await _appDbContext.Services
                    .Where(s => s.IsSuspend==false)
                    .Select(s => new
                    {
                        soRId = s.SoRId,
                        name = s.Name,
                        categoryId = s.ServiceCategoryId,
                        rating = new
                        {
                            rate = s.OverallRate,
                            count = s.ReviewAndRating != null ? s.ReviewAndRating.Select(r => r.EventId).Count() : 0,

                        },
                        vendor = s.Vendor != null ? s.Vendor.CompanyName : null,
                        description = s.Description,
                        price = (
                                    from vp in _appDbContext.VendorSRPrices
                                    join p in _appDbContext.Prices on vp.PId equals p.Pid
                                    join pm in _appDbContext.PriceModels on p.ModelId equals pm.ModelId
                                    where vp.SoRId == s.SoRId
                                    select new
                                    {
                                        value = p.BasePrice,
                                        priceModelName = pm.ModelName,
                                        id = pm.ModelId
                                    }
                                ).ToList(),
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
                // Retrieve service details for the specified service ID
                var service = await _appDbContext.Services
                    .Where(s => s.SoRId == soRId)
                    .Select(s => new
                    {
                        soRId = s.SoRId,
                        name = s.Name,
                        vendor = new
                        {
                            vendorId = s.VendorId,
                            companyName = s.Vendor != null ? s.Vendor.CompanyName : null,
                        },
                        capacity = s.Capacity,
                        serviceCategory = s.ServiceCategory,
                        description = s.Description,
                        reviewAndRating = s.ReviewAndRating != null ? s.ReviewAndRating
                            .Where(rr => rr.SoRId == s.SoRId)
                            .Select(rr => new
                            {
                                avatar = rr.Event != null && rr.Event.Client != null ? rr.Event.Client.ProfilePic : null,
                                fname = rr.Event != null && rr.Event.Client != null ? rr.Event.Client.FirstName : null,
                                lname = rr.Event != null && rr.Event.Client != null ? rr.Event.Client.LastName : null,
                                comment = rr.Comment,
                                rate = rr.Ratings
                            })
                            .ToList() : null,
                        featureAndFacility =s.FeaturesAndFacilities != null ? s.FeaturesAndFacilities.Select(ff => ff.FacilityName): null,
                        price = (
                                    from vp in _appDbContext.VendorSRPrices
                                    join p in _appDbContext.Prices on vp.PId equals p.Pid
                                    join pm in _appDbContext.PriceModels on p.ModelId equals pm.ModelId
                                    where vp.SoRId == s.SoRId
                                    select new
                                    {
                                        value = p.BasePrice,
                                        model = pm.ModelName,
                                        modelId = pm.ModelId,
                                        name = p.Pname
                                    }
                                ).ToList(),
                        location =s.VendorSRLocations != null ? s.VendorSRLocations.Select(vl => new
                        {
                            vl.HouseNo,vl.Area,vl.District,vl.Country,vl.State
                        }).ToList() : null,
                        images = s.VendorRSPhotos != null ? s.VendorRSPhotos.Select(vp => vp.Image): null,
                        videos = s.VendorRSVideos != null ? s.VendorRSVideos.Select(vv => vv.Video) : null,
                    })
                    .ToListAsync();

                return service;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred whiel getting service details", ex);
            }

        }


        public async Task UpdateServiceAsync(Guid vendorId, int soRId, object data)
        {
            try
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data), "No data provided.");
                }

                string jsonString = data?.ToString() ?? string.Empty;

                JObject json = JObject.Parse(jsonString);

                var service = await _appDbContext.Services
                    .Include(s => s.FeaturesAndFacilities)
                    .Include(s => s.VendorSRPrices)
                    .Include(s => s.VendorSRLocations)
                    .Include(s => s.VendorRSPhotos)
                    .Include(s => s.VendorRSVideos)
                    .FirstOrDefaultAsync(s => s.SoRId == soRId && s.VendorId == vendorId);

                if (service == null)
                {
                    throw new ArgumentException("Service not found.");
                }

                // Update service properties with data
                service.Name = json["serviceName"]?.ToString();
                service.Description = json["serviceDescription"]?.ToString();
                service.ServiceCategoryId = json["serviceCategory"]?.Value<int>() ?? 0;

                if (json["serviceMaxCapacity"] != null && int.TryParse(json["serviceMaxCapacity"]?.ToString(), out int capacity))
                {
                    // Parsing successful, assign the parsed value to Capacity
                    service.Capacity = capacity;
                } else
                {
                    service.Capacity = 0;
                }
                // Clear existing related entities
                // Remove all entries related to features and facilities of the service
                if (service.FeaturesAndFacilities != null)
                {
                    _appDbContext.FeatureAndFacility.RemoveRange(service.FeaturesAndFacilities);

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
                            service.FeaturesAndFacilities.Add(featureOrFacility);
                        }
                    }
                }

                // Remove all price entries related to the service
                if (service.VendorSRPrices != null)
                {
                    _appDbContext.VendorSRPrices.RemoveRange(service.VendorSRPrices);

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

                            await _appDbContext.Prices.AddAsync(servicePrice);
                            await _appDbContext.SaveChangesAsync();

                            var vendorSRPrice = new VendorSRPrice
                            {
                                SoRId = service.SoRId,
                                PId = servicePrice.Pid
                            };

                            service.VendorSRPrices.Add(vendorSRPrice);
                        }
                    }
                }

                // Remove all location entries related to the service
                if (service.VendorSRLocations != null)
                {
                    _appDbContext.VendorSRLocation.RemoveRange(service.VendorSRLocations);

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

                            service.VendorSRLocations.Add(vendorSRLocation);
                        }
                    }
                }

                // Remove all photo entries related to the service
                if (service.VendorRSPhotos != null)
                {
                    _appDbContext.VendorSRPhoto.RemoveRange(service.VendorRSPhotos);

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

                            service.VendorRSPhotos.Add(vendorSRPhoto);
                        }
                    }
                }

                // Remove all video entries related to the service
                if (service.VendorRSVideos != null)
                {
                    _appDbContext.VendorSRVideo.RemoveRange(service.VendorRSVideos);

                    var videos = json["videos"] as JArray;
                    if (videos != null)
                    {
                        foreach (var video in videos)
                        {
                            var vendorSRVideo = new VendorSRVideo
                            {
                                SoRId = service.SoRId,
                                Video = video.ToString()
                            };

                            service.VendorRSVideos.Add(vendorSRVideo);
                        }
                    }
                }


                // Save changes to the database
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
    }
}
