﻿using eventify_backend.DTOs;
using eventify_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly ResourceService _resourceService;

        // Constructor with Dependency Injection for ResourceService
        public ResourceController(ResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        // Endpoint to get all service categories

        [HttpGet("/api/[Controller]/categories")]
        public async Task<IActionResult> GetAllResourceCategories()
        {
            try
            {
                // Retrieve resource categories using the service layer
                List<ResourceCategoryDTO> categories = await _resourceService.GetAllResourceCategories();

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

        // GET: api/resource/{categoryId}
        [HttpGet("/api/[Controller]/{categoryId}")]
        public async Task<IActionResult> GetResourceByCategory([FromRoute] int categoryId)
        {
            try
            {
                // Retrieve resources by category from the service layer
                var services = await _resourceService.GetResourcesByCategoryId(categoryId);

                if (services == null || services.Count == 0)
                {
                    // Return 404 Not Found if no resources found
                    return NotFound("not found");
                }

                // Return the retrieved resources
                return Ok(services);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
