﻿using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.Json;

namespace eventify_backend.Services
{
    public class ResourceService
    {
        private readonly AppDbContext _appDbContext;
        private readonly NotificationService _notificationService;

        public ResourceService(AppDbContext appDbContext, NotificationService notificationService)
        {
            this._appDbContext = appDbContext;
            _notificationService = notificationService;
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
                var resource = await _appDbContext.ServiceAndResources.FindAsync(SORId);

                if (resource == null)
                    return null;

                resource.IsSuspend = !resource.IsSuspend; // Toggle the suspend state
                await _appDbContext.SaveChangesAsync();

                string resourceName = resource?.Name ?? "resource"; // Use a default name if resource is null
                string suspensionState = resource!.IsSuspend ? "suspended" : "unsuspended";
                var message = $"{resourceName} is {suspensionState}";

                var userId = resource.VendorId;

                await _notificationService.CreateIndividualNotification(message, userId);

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

                string resourceName = resource?.Name ?? "resource"; // Use a default name if resource is null
                var message = $"{resourceName} is deleted by administrator";

                var userId = resource!.VendorId;

                await _notificationService.CreateIndividualNotification(message, userId);

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



                //notification

                string resourceName = resource?.Name ?? "Resource";
                var message = $"{resourceName} delete request rejected by administrator";

                var userId = resource!.VendorId;

                await _notificationService.CreateIndividualNotification(message, userId);


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


                //notification

                string resourceName = resource?.Name ?? "resource";
                var message = $"{resourceName} delete request approved by administrator";

                var userId = resource!.VendorId;

                await _notificationService.CreateIndividualNotification(message, userId);

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

                //notification

                if (resource.IsRequestToDelete)
                {
                    string resourceName = resource?.Name ?? "resource";
                    // Correct LINQ query to fetch the company name based on SORId
                    var companyName = await _appDbContext.Vendors
                        .Where(v => v.ServiceAndResources!.Any(s => s.SoRId == SORId))
                        .Select(v => v.CompanyName)
                        .FirstOrDefaultAsync();

                    var message = $"{resourceName} request to delete by {companyName}";

                    var adminId = await _appDbContext.Users.Where(u => u.Role=="Admin").Select(u => u.UserId).FirstOrDefaultAsync();

                    await _notificationService.CreateIndividualNotification(message, adminId);
                }

                return resource!.ResourceCategoryId;
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
                            EndDate = e.Event.EndDateTime.Date.ToString("yyyy-MM-dd"),
                            EventId = e.Event.EventId,
                            EventName = e.Event.Name
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

                //notification

                // Fetch event details
                var eventDetails = await _appDbContext.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
                if (eventDetails == null)
                {
                    throw new Exception($"Event not found with EventId: {eventId}");
                }

                // Fetch company name
                var companyName = await _appDbContext.ServiceAndResources
                    .Where(v => v.SoRId == soRId)
                    .Select(v => v.Vendor!.CompanyName)
                    .FirstOrDefaultAsync();

                // Construct notification message
                var message = $"{companyName} accepted the resource for {eventDetails.Name}";

                // Notify client
                await _notificationService.CreateIndividualNotification(message, eventDetails.ClientId);

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

                //notification

                // Fetch event details
                var eventDetails = await _appDbContext.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
                if (eventDetails == null)
                {
                    throw new Exception($"Event not found with EventId: {eventId}");
                }

                // Fetch company name
                var companyName = await _appDbContext.ServiceAndResources
                    .Where(v => v.SoRId == soRId)
                    .Select(v => v.Vendor!.CompanyName)
                    .FirstOrDefaultAsync();

                // Construct notification message
                var message = $"{companyName} rejected the resource for {eventDetails.Name}";

                // Notify client
                await _notificationService.CreateIndividualNotification(message, eventDetails.ClientId);

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
                    OverallRate = 0
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

                // Extract user manual information from the JSON data
                var manuals = json["manuals"] as JArray;

                if (manuals != null)
                {
                    foreach (var file in manuals)
                    {
                        var resourceManual = new ResourceManual
                        {
                            SoRId = resource.SoRId,
                            Manual = file.ToString()
                        };

                        // Add the video to the database context
                        _appDbContext.ResourceManual.Add(resourceManual);
                    }
                }

                // Similar handling for other entities
                await _appDbContext.SaveChangesAsync();

                await _notificationService.CreateNotificationAsync(resource.SoRId, vendorId,"resource");
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

        public async Task<object> GetResourcesForClientsAsync(int page, int pageSize, string sortBy, int? minPrice, int? maxPrice, int? modelId, string categories, int? rate)
        {
            try
            {
                // Base query for resources
                var resourcesQuery = _appDbContext.Resources
                    .Where(s => s.IsSuspend == false)
                    .AsQueryable();

                // Join with VendorSRPrices and Prices for filtering by price and modelId
                if (minPrice.HasValue && maxPrice.HasValue && modelId.HasValue)
                {
                    resourcesQuery = (from s in resourcesQuery
                                      join vp in _appDbContext.VendorSRPrices on s.SoRId equals vp.SoRId
                                      join p in _appDbContext.Prices on vp.PId equals p.Pid
                                      where p.BasePrice >= minPrice.Value
                                         && p.BasePrice <= maxPrice.Value
                                         && p.ModelId == modelId.Value
                                      select s)
                                    .Distinct();
                }

                // Filter by categories if provided
                if (!string.IsNullOrEmpty(categories))
                {
                    var categoryIds = categories.Split(',').Select(int.Parse).ToList();
                    resourcesQuery = resourcesQuery.Where(s => categoryIds.Contains(s.ResourceCategoryId));
                }

                // Filter by rate if provided
                if (rate.HasValue)
                {
                    resourcesQuery = resourcesQuery.Where(s => s.OverallRate >= rate.Value);
                }

                // Apply sorting
                resourcesQuery = sortBy switch
                {
                    "sNameAZ" => resourcesQuery.OrderBy(s => s.Name),
                    "sNameZA" => resourcesQuery.OrderByDescending(s => s.Name),
                    "RateLH" => resourcesQuery.OrderBy(s => s.OverallRate),
                    "RateHL" => resourcesQuery.OrderByDescending(s => s.OverallRate),
                    _ => resourcesQuery
                };

                // Get paginated results
                var resources = await resourcesQuery
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Select(s => new
                    {
                        soRId = s.SoRId,
                        name = s.Name,
                        categoryId = s.ResourceCategoryId,
                        rating = new
                        {
                            rate = s.OverallRate,
                            count = s.ReviewAndRating != null ? s.ReviewAndRating.Count() : 0,
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
                    })
                    .ToListAsync();

                // Get total item count for pagination
                var totalItems = await resourcesQuery.CountAsync();

                return new
                {
                    data = resources,
                    totalItems = totalItems
                };
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
                        resourceCategory = s.ResourceCategory,
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
                        Manuals = s.ResourceManual != null ? s.ResourceManual.Select(rm => rm.Manual) : null
                    })
                    .AsSplitQuery()
                    .ToListAsync();

                return resource;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting resource details", ex);
            }
        }


        public async Task UpdateResourceAsync(Guid vendorId, int soRId, object data)
        {
            try
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data), "No data provided.");
                }

                string jsonString = data?.ToString() ?? string.Empty;

                JObject json = JObject.Parse(jsonString);

                var resource = await _appDbContext.Resources
                    .Include(s => s.FeaturesAndFacilities)
                    .Include(s => s.VendorSRPrices)
                    .Include(s => s.VendorSRLocations)
                    .Include(s => s.VendorRSPhotos)
                    .Include(s => s.VendorRSVideos)
                    .Include(s => s.ResourceManual)
                    .FirstOrDefaultAsync(s => s.SoRId == soRId && s.VendorId == vendorId);

                if (resource == null)
                {
                    throw new ArgumentException("Resource not found.");
                }

                // Update resource properties with data
                resource.Name = json["resourceName"]?.ToString();
                resource.Description = json["resourceDescription"]?.ToString();
                resource.ResourceCategoryId = json["resourceCategory"]?.Value<int>() ?? 0;

                if (json["resourceMaxCapacity"] != null && int.TryParse(json["resourceMaxCapacity"]?.ToString(), out int capacity))
                {
                    // Parsing successful, assign the parsed value to Capacity
                    resource.Capacity = capacity;
                }
                else
                {
                    resource.Capacity = 0;
                }
                // Clear existing related entities
                // Remove all entries related to features and facilities of the resource
                if (resource.FeaturesAndFacilities != null)
                {
                    _appDbContext.FeatureAndFacility.RemoveRange(resource.FeaturesAndFacilities);

                    var featureAndFacility = json["resourceFeatures"] as JArray;

                    if (featureAndFacility != null)
                    {
                        foreach (var item in featureAndFacility)
                        {
                            var featureOrFacility = new FeatureAndFacility
                            {
                                FacilityName = item["name"]?.ToString(),
                                SoRId = resource.SoRId
                            };
                            resource.FeaturesAndFacilities.Add(featureOrFacility);
                        }
                    }
                }

                // Remove all price entries related to the resource
                if (resource.VendorSRPrices != null)
                {
                    _appDbContext.VendorSRPrices.RemoveRange(resource.VendorSRPrices);

                    var price = json["resourcePricePackages"] as JArray;
                    if (price != null)
                    {
                        foreach (var item in price)
                        {
                            var resourcePrice = new Price
                            {
                                Pname = item["packageName"]?.ToString(),
                                BasePrice = item["basePrice"]?.Value<double>() ?? 0,
                                ModelId = item["priceModel"]?.Value<int>() ?? 0
                            };

                            await _appDbContext.Prices.AddAsync(resourcePrice);
                            await _appDbContext.SaveChangesAsync();

                            var vendorSRPrice = new VendorSRPrice
                            {
                                SoRId = resource.SoRId,
                                PId = resourcePrice.Pid
                            };

                            resource.VendorSRPrices.Add(vendorSRPrice);
                        }
                    }
                }

                // Remove all location entries related to the resource
                if (resource.VendorSRLocations != null)
                {
                    _appDbContext.VendorSRLocation.RemoveRange(resource.VendorSRLocations);

                    var location = json["resourceLocations"] as JArray;
                    if (location != null)
                    {
                        foreach (var item in location)
                        {
                            var vendorSRLocation = new VendorSRLocation
                            {
                                SoRId = resource.SoRId,
                                HouseNo = item["houseNoStreetRoad"]?.ToString(),
                                Area = item["cityTownArea"]?.ToString(),
                                District = item["district"]?.ToString(),
                                Country = item["country"]?.ToString(),
                                State = item["stateProvinceRegion"]?.ToString(),
                            };

                            resource.VendorSRLocations.Add(vendorSRLocation);
                        }
                    }
                }

                // Remove all photo entries related to the resource
                if (resource.VendorRSPhotos != null)
                {
                    _appDbContext.VendorSRPhoto.RemoveRange(resource.VendorRSPhotos);

                    var images = json["images"] as JArray;
                    if (images != null)
                    {
                        foreach (var image in images)
                        {
                            var vendorSRPhoto = new VendorSRPhoto
                            {
                                SoRId = resource.SoRId,
                                Image = image.ToString()
                            };

                            resource.VendorRSPhotos.Add(vendorSRPhoto);
                        }
                    }
                }

                // Remove all video entries related to the resource
                if (resource.VendorRSVideos != null)
                {
                    _appDbContext.VendorSRVideo.RemoveRange(resource.VendorRSVideos);

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

                            resource.VendorRSVideos.Add(vendorSRVideo);
                        }
                    }
                }

                // Remove all manual entries related to the resource
                if (resource.ResourceManual != null)
                {
                    _appDbContext.ResourceManual.RemoveRange(resource.ResourceManual);

                    var manuals = json["manuals"] as JArray;
                    if (manuals != null)
                    {
                        foreach (var manual in manuals)
                        {
                            var resourceManual = new ResourceManual
                            {
                                SoRId = resource.SoRId,
                                Manual = manual.ToString()
                            };

                            resource.ResourceManual.Add(resourceManual);
                        }
                    }
                }


                // Save changes to the database
                await _appDbContext.SaveChangesAsync();

                await _notificationService.AddUpdatedNotificationAsync(vendorId, soRId);

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
        public async Task<List<PriceModelDto>> GetAvailablePriceModelsAsync()
        {
            try
            {
                // Using LINQ join to ensure we get PriceModels that are referenced
                var priceModels = await (from pm in _appDbContext.PriceModels
                                         join p in _appDbContext.Prices on pm.ModelId equals p.ModelId
                                         join vp in _appDbContext.VendorSRPrices on p.Pid equals vp.PId
                                         join s in _appDbContext.Resources on vp.SoRId equals s.SoRId
                                         select new PriceModelDto
                                         {
                                             ModelId = pm.ModelId,
                                             ModelName = pm.ModelName
                                         })
                                         .Distinct()
                                         .ToListAsync();

                return priceModels;
            }
            catch (Exception ex)
            {
                // Log exception details here for debugging purposes
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        public async Task<Dictionary<int, int>> GetRatingCountAsync()
        {
            var ratingCount = new Dictionary<int, int>();

            ratingCount[4] = await _appDbContext.Resources.CountAsync(s => s.OverallRate >= 4);
            ratingCount[3] = await _appDbContext.Resources.CountAsync(s => s.OverallRate >= 3);
            ratingCount[2] = await _appDbContext.Resources.CountAsync(s => s.OverallRate >= 2);
            ratingCount[1] = await _appDbContext.Resources.CountAsync(s => s.OverallRate >= 1);
            ratingCount[0] = await _appDbContext.Resources.CountAsync(s => s.OverallRate >= 0);

            return ratingCount;
        }
    }
}
