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
                throw new Exception($"Error occurred while fetching service categories: {ex.Message}");
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
                throw new Exception("Error occurred while fetching services by category.", ex);
            }
        }


    }
}
