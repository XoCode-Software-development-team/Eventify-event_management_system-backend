using eventify_backend.Data;
using eventify_backend.DTOs;
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


    }
}
