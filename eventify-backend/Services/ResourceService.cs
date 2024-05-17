using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace eventify_backend.Services
{
    public class ResourceService
    {
        private readonly AppDbContext _appDbContext;

        public ResourceService(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
            
        }

        public async Task<List<ResourceCategoryDTO>> GetAllResourceCategories()
        {
            try
            {
                // Retrieve all resource categories from the database
                var categories = await _appDbContext.ResourceCategories
                    .Select(x => new ResourceCategoryDTO
                    {
                        CategoryId = x.CategoryId,
                        ResourceCategoryName = x.ResourceCategoryName
                    })
                    .ToListAsync();

                return categories;
            }

            catch (Exception ex)
            {
                // Handle exceptions and return an empty list or rethrow the exception
                throw new Exception($"Error occurred while fetching resource categories: {ex.Message}");
            }
        }

        // Method to get resources by category ID
        public async Task<List<ResourceDTO>> GetResourcesByCategoryId(int categoryId)
        {
            try
            {
                // Retrieve resources related to the specified category
                var resourcesWithCategories = await _appDbContext.Resources
                    .Include(s => s.ResourceCategory)
                    .Where(s => s.ResourceCategoryId == categoryId)
                    .Select(s => new ResourceDTO
                    {
                        SoRId = s.SoRId,
                        Resource = s.Name,
                        Rating = s.OverallRate ?? 0, // Handle nullable rating
                        IsSuspend = s.IsSuspend,
                    })
                    .ToListAsync();

                return resourcesWithCategories;
            }

            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                throw new Exception("Error occurred while fetching resources by category.", ex);
            }
        }

        public async Task<int?> ChangeSuspendStateAsync(int SORId)
        {
            try
            {
                var resource = await _appDbContext.ServiceAndResources.FindAsync(SORId);  // Find the resource by ID

                if (resource == null)
                    return null;

                resource.IsSuspend = !resource.IsSuspend; // Toggle the suspend state
                await _appDbContext.SaveChangesAsync();


                // Get the category ID of the service
                var categoryId = await _appDbContext.Resources
                    .Where(s => s.SoRId == resource.SoRId)
                    .Select(s => s.ResourceCategoryId)
                    .FirstOrDefaultAsync();

                return categoryId;
            }

            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                throw new Exception("An error occurred.", ex);
            }
        }

        public async Task<int?> DeleteResourceAsync(int Id)
        {
            try
            {
                var resource = await _appDbContext.Resources.FindAsync(Id);    // Find the resource by ID

                if (resource == null)
                    return null;

                // Find all PId values associated with the given SoRId
                var pIdsToDelete = _appDbContext.VendorSRPrices
                    .Where(vp => vp.SoRId == resource.SoRId)
                    .Select(vp => vp.PId)
                    .ToList();

                if (pIdsToDelete != null)
                {
                    // Remove all entries from the Price table with PIds found above
                    var pricesToDelete = _appDbContext.Prices.Where(p => pIdsToDelete.Contains(p.Pid));
                    _appDbContext.Prices.RemoveRange(pricesToDelete);
                }

                var deletedCategoryId = resource.ResourceCategoryId;
                _appDbContext.Resources.Remove(resource);               // Remove the resource from the database

                await _appDbContext.SaveChangesAsync();

                return deletedCategoryId; // return category id of deleted resource
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the resource.", ex);
            }
        }

        public async Task<List<ResourceCategoryDTO>> GetCategoriesWithRequestToDeleteAsync()
        {
            try
            {
                // Query to join ResourceCategories with Resources flagged for deletion and select relevant data into ResourceCategoryDTO objects

                var categoriesWithRequestToDelete = await _appDbContext.ResourceCategories
                    .Join(_appDbContext.Resources.Where(s => s.IsRequestToDelete),
                        category => category.CategoryId,
                        resource => resource.ResourceCategoryId,
                        (category, resource) => new ResourceCategoryDTO { CategoryId = category.CategoryId, ResourceCategoryName = category.ResourceCategoryName })
                    .Distinct()
                    .ToListAsync();

                return categoriesWithRequestToDelete;         // Return the list of categories with resources flagged for deletion
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving categories with resources flagged for deletion.", ex);
            }
        }

        public async Task<List<ResourceDTO>> GetResourcesWithRequestToDeleteAsync(int categoryId)
        {
            try
            {
                // Query to retrieve resources within the specified category with requests to delete

                var resources = await _appDbContext.Resources
                    .Where(s => s.ResourceCategoryId == categoryId && s.IsRequestToDelete)
                    .Select(s => new ResourceDTO
                    {
                        SoRId = s.SoRId,
                        Resource = s.Name,
                        Rating = s.OverallRate,
                    })
                    .ToListAsync();

                return resources;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving resources with requests to delete.", ex);
            }
        }

        public async Task<object?> ChangeDeleteRequestStateAsync(int soRId)
        {
            try
            {
                var resource = await _appDbContext.Resources.FindAsync(soRId); // Find resource by soRId
                if (resource == null)
                {
                    return null;
                }

                resource.IsRequestToDelete = false; // Mark the resource as no longer needing deletion
                await _appDbContext.SaveChangesAsync(); // Save changes to the database

                // Calculate the remaining count of resources still requesting deletion
                var remainingCount = await _appDbContext.Resources.CountAsync(s => s.ResourceCategoryId == resource.ResourceCategoryId && s.IsRequestToDelete);

                return new { DeletedResourceCategoryId = resource.ResourceCategoryId, RemainingCount = remainingCount };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while changing delete request state of the resource.", ex);
            }
        }

        public async Task<object?> ApproveVendorDeleteRequestAsync(int soRId)
        {
            try
            {
                var resource = await _appDbContext.Resources.FindAsync(soRId);
                if (resource == null)
                {
                    return null;
                }

                // Find all PId values associated with the given SoRId
                var pIdsToDelete = _appDbContext.VendorSRPrices
                    .Where(vp => vp.SoRId == resource.SoRId)
                    .Select(vp => vp.PId)
                    .ToList();


                if (pIdsToDelete != null)
                {
                    // Remove all entries from the Price table with PIds found above
                    var pricesToDelete = _appDbContext.Prices.Where(p => pIdsToDelete.Contains(p.Pid));
                    _appDbContext.Prices.RemoveRange(pricesToDelete);
                }

                var deletedCategory = resource.ResourceCategoryId; // Save the category ID before deletion
                _appDbContext.Resources.Remove(resource); // Remove the resource from the database
                await _appDbContext.SaveChangesAsync(); // Save changes to the database

                // Calculate the remaining count of resources still requesting deletion
                var remainingCount = await _appDbContext.Resources.CountAsync(s => s.ResourceCategoryId == deletedCategory && s.IsRequestToDelete);

                return new { DeletedResourceCategoryId = resource.ResourceCategoryId, RemainingCount = remainingCount };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving vendor delete request for the resource.", ex);
            }
        }

        public async Task<List<ResourceCategoryDTO>> GetAllResourceCategoriesOfVendorAsync(Guid Id)
        {
            try
            {
                var categories = await _appDbContext.ResourceCategories
                    .Where(sc => sc.Resources != null && sc.Resources.Any(s => s.VendorId == Id))
                    .Select(x => new ResourceCategoryDTO { CategoryId = x.CategoryId, ResourceCategoryName = x.ResourceCategoryName })
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving resource categories associated with the vendor.", ex);
            }
        }

        public async Task<object> GetVendorResourceByCategoryAsync(int categoryId, Guid vendorId)
        {
            try
            {
                // Retrieve the resource category and vendor

                var resourceCategory = _appDbContext.ResourceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);
                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (resourceCategory == null || vendor == null)
                {
                    throw new Exception("Resource category or vendor not found.");
                }

                // Query to retrieve resources within the specified category and vendor

                var resourcesWithCategories = await _appDbContext.Resources
                    .Where(s => s.ResourceCategoryId == categoryId && s.VendorId == vendorId)
                    .Select(s => new
                    {
                        SoRId = s.SoRId,
                        Resource = s.Name,
                        Rating = s.OverallRate,
                        IsRequestToDelete = s.IsRequestToDelete,
                    })
                    .ToListAsync();

                return resourcesWithCategories;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        public async Task<int> RequestToDeleteAsync(int SORId)
        {
            try
            {
                var resource = await _appDbContext.Resources.FindAsync(SORId);
                if (resource == null)
                {
                    throw new Exception("Resource not found.");
                }

                // Toggle the IsRequestToDelete flag
                resource.IsRequestToDelete = !resource.IsRequestToDelete;

                await _appDbContext.SaveChangesAsync();

                return resource.ResourceCategoryId;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        public async Task<object> GetResourceCategoriesOfBookedResourcesAsync(Guid Id)
        {
            try
            {
                // Get the current date and time

                var currentDate = DateTime.Now;

                // Query to retrieve resource categories of booked resources for the specified vendor
                var categories = await _appDbContext.ResourceCategories
                    .Where(sc => sc.Resources != null && sc.Resources.Any(s => s.EventSRs != null &&
                        s.EventSRs.Any(esr => esr.Event != null && esr.ServiceAndResource != null &&
                            esr.Event.EndDateTime > currentDate && esr.ServiceAndResource.VendorId == Id)))
                    .Select(x => new { x.CategoryId, x.ResourceCategoryName })
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        public async Task<object> GetBookedResourcesOfVendorAsync(int categoryId, Guid vendorId)
        {
            try
            {
                // Get the current date and time
                var currentDate = DateTime.Now;

                // Query to retrieve booked resources of the vendor for the specified resource category
                var resources = await _appDbContext.Resources
                    .Where(s => s.ResourceCategoryId == categoryId &&
                                s.VendorId == vendorId &&
                                s.EventSRs != null &&
                                s.EventSRs.Any(e => e.Event != null && e.Event.EndDateTime > currentDate))
                    .SelectMany(s => s.EventSRs!
                        .Where(e => e.Event != null && e.Event.EndDateTime > currentDate)
                        .Select(e => new
                        {
                            SoRId = s.SoRId,
                            Resource = s.Name,
                            EventDate = e.Event!.StartDateTime.Date.ToString("yyyy-MM-dd"),
                            EndDate = e.Event.EndDateTime.Date.ToString("yyyy-MM-dd")
                        }))
                    .ToListAsync();

                return resources;
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
                var categories = await _appDbContext.ResourceCategories
                    .Where(sc => sc.Resources != null && sc.Resources.Any(s => s.EventSoRApproves != null && s.VendorId == vendorId && s.EventSoRApproves
                        .Any(esra => esra.IsApprove == false)))
                    .Select(x => new { x.CategoryId, x.ResourceCategoryName })
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        public async Task<object?> GetResourcesOfBookingRequestAsync(int categoryId, Guid vendorId)
        {
            try
            {
                // Retrieve the resource category and vendor
                var resourceCategory = _appDbContext.ResourceCategories.FirstOrDefault(sc => sc.CategoryId == categoryId);
                var vendor = _appDbContext.Vendors.FirstOrDefault(v => v.UserId == vendorId);

                if (resourceCategory == null || vendor == null)
                {
                    return false;
                }

                var currentDate = DateTime.Now;

                // Query to retrieve resources with booking requests for the specified category and vendor
                var resources = await _appDbContext.Resources
                    .Where(s => s.ResourceCategoryId == categoryId &&
                                s.VendorId == vendorId &&
                                s.EventSoRApproves != null && // Explicit null check for EventSoRApproves
                                s.EventSoRApproves.Any(e => e.IsApprove == false))
                    .SelectMany(s => s.EventSoRApproves!
                        .Where(e => e.IsApprove == false)
                        .Select(e => new
                        {
                            SoRId = s.SoRId,
                            EventId = e.EventId,
                            Resource = s.Name,
                            EventName = e.Event != null ? e.Event.Name : null,
                            EventDate = e.Event != null ? e.Event.StartDateTime.Date.ToString("yyyy-MM-dd") : DateTime.MinValue.ToString("yyyy-MM-dd"),
                            EndDate = e.Event != null ? e.Event.EndDateTime.Date.ToString("yyyy-MM-dd") : DateTime.MinValue.ToString("yyyy-MM-dd")
                        }))
                    .ToListAsync();



                return resources;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        public async Task<bool> BookResourceByVendorAsync(int eventId, int soRId)
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

        public async Task<bool> RejectResourceFromVendorAsync(int eventId, int soRId)
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

        public async Task AddNewResourceAsync(Guid vendorId, object data)
        {
            try
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data), "No data provided.");
                }

                string jsonString = data?.ToString() ?? string.Empty;         // Convert data to JSON string

                JObject json = JObject.Parse(jsonString);         // Parse JSON string to JObject


                // Create new Resource object
                var resource = new Resource
                {
                    Name = json["resourceName"]?.ToString(),
                    Description = json["resourceDescription"]?.ToString(),
                    IsSuspend = false,
                    IsRequestToDelete = false,
                    VendorId = vendorId,
                    ResourceCategoryId = json["resourceCategory"]?.Value<int>() ?? 0,
                };

                if (json["resourceMaxCapacity"] != null && int.TryParse(json["resourceMaxCapacity"]?.ToString(), out int capacity))
                {
                    // Parsing successful, assign the parsed value to Capacity
                    resource.Capacity = capacity;
                }

                // Add resource to the database
                _appDbContext.Resources.Add(resource);

                await _appDbContext.SaveChangesAsync();   // Save changes to the database


                // Handle feature and facility
                var featureAndFacility = json["resourceFeatures"] as JArray;// Extract feature and facility information from the JSON data
                if (featureAndFacility != null)
                {
                    foreach (var item in featureAndFacility)
                    {
                        // Create a new FeatureAndFacility object
                        var featureOrFacility = new FeatureAndFacility
                        {
                            FacilityName = item["name"]?.ToString(),
                            SoRId = resource.SoRId
                        };
                        _appDbContext.FeatureAndFacility.Add(featureOrFacility);
                    }
                }

                var location = json["resourceLocations"] as JArray; // Extract location information from the JSON data

                if (location != null)
                {
                    foreach (var item in location)
                    {
                        var vendorSRLocation = new VendorSRLocation   // Create a new VendorSRLocation object
                        {
                            SoRId = resource.SoRId,
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

                var price = json["resourcePricePackages"] as JArray;// Extract price information from the JSON data

                if (price != null)
                {
                    // Iterate over each price package item
                    foreach (var item in price)
                    {
                        var resourcePrice = new Price
                        {
                            Pname = item["packageName"]?.ToString(),
                            BasePrice = item["basePrice"]?.Value<double>() ?? 0,
                            ModelId = item["priceModel"]?.Value<int>() ?? 0
                        };

                        // Add the price to the database context
                        _appDbContext.Prices.Add(resourcePrice);
                        await _appDbContext.SaveChangesAsync();


                        var vendorSRPrice = new VendorSRPrice   // Create a new VendorSRPrice object
                        {
                            SoRId = resource.SoRId,
                            PId = resourcePrice.Pid
                        };

                        // Add the vendor resource price to the database context
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
                            SoRId = resource.SoRId,
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
                            SoRId = resource.SoRId,
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

        public async Task<object> GetMaxPriceOfResourceAsync(int modelId)
        {
            try
            {
                // Retrieve the maximum resource price for the specified model ID
                var maxResourcePrice = await (
                    from resource in _appDbContext.Resources
                    join vendorSRPrice in _appDbContext.VendorSRPrices on resource.SoRId equals vendorSRPrice.SoRId
                    join price in _appDbContext.Prices on vendorSRPrice.PId equals price.Pid
                    where price.ModelId == modelId
                    select price.BasePrice
                ).MaxAsync();

                return maxResourcePrice;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting max price.", ex);
            }
        }

        public async Task<object> GetResourcesForClientsAsync()
        {
            try
            {
                // Retrieve resources for clients that are not suspended
                var resources = await _appDbContext.Resources
                    .Where(s => s.IsSuspend == false)
                    .Select(s => new
                    {
                        soRId = s.SoRId,
                        name = s.Name,
                        categoryId = s.ResourceCategoryId,
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

                return resources;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting resources", ex);
            }
        }

        public async Task<object> GetResourceDetailsForClientAsync(int soRId)
        {
            try
            {
                // Retrieve resource details for the specified resource ID
                var resource = await _appDbContext.Resources
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
                        serviceCategory = s.ResourceCategory,
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
                        featureAndFacility = s.FeaturesAndFacilities != null ? s.FeaturesAndFacilities.Select(ff => ff.FacilityName) : null,
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
                        location = s.VendorSRLocations != null ? s.VendorSRLocations.Select(vl => new
                        {
                            vl.HouseNo,
                            vl.Area,
                            vl.District,
                            vl.Country,
                            vl.State
                        }).ToList() : null,
                        images = s.VendorRSPhotos != null ? s.VendorRSPhotos.Select(vp => vp.Image) : null,
                        videos = s.VendorRSVideos != null ? s.VendorRSVideos.Select(vv => vv.Video) : null,
                    })
                    .ToListAsync();

                return resource;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred whiel getting resource details", ex);
            }

        }


    }
}
