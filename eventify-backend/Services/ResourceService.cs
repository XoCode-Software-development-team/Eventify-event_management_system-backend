using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;

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


    }
}
