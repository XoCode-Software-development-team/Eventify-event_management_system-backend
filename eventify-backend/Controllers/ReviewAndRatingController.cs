using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewAndRatingController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;


        public ReviewAndRatingController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        [HttpPost("Add")]
        [Authorize]
        public async Task<IActionResult> AddReviewAndRating(ReviewAndRatingDTO reviewAndRatingDTO)
        {
            try
            {
                if (reviewAndRatingDTO == null)
                {
                    return BadRequest("Review and rating data is required.");
                }

                var reviewAndRating = new ReviewAndRating()
                {
                    EventId = reviewAndRatingDTO.EventId,
                    SoRId = reviewAndRatingDTO.SoRId,
                    Ratings = reviewAndRatingDTO.Rate,
                    Comment = reviewAndRatingDTO.Review,
                    TimeSpan = DateTime.UtcNow // Correct usage of DateTime.UtcNow
                };

                _appDbContext.ReviewAndRatings.Add(reviewAndRating);
                await _appDbContext.SaveChangesAsync();

                var service = await _appDbContext.ServiceAndResources.FindAsync(reviewAndRatingDTO.SoRId);
                if (service != null)
                {
                    var ratings = await _appDbContext.ReviewAndRatings
                        .Where(r => r.SoRId == reviewAndRatingDTO.SoRId)
                        .Select(r => r.Ratings)
                        .ToListAsync();

                    service.OverallRate = (float)Math.Round(ratings.Average(), 1);

                    await _appDbContext.SaveChangesAsync();
                }



                return Ok(new { Message = "Review and rating added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "An error occurred while adding the review and rating.", Details = ex.Message });
            }
        }



        [HttpGet("top")]
        [Authorize]
        public async Task<IActionResult> GetTopRatedVendors()
        {
            try
            {
                var topVendors = await _appDbContext.Vendors
                    .Select(v => new
                    {
                        Vendor = v,
                        AverageRating = v.ServiceAndResources!.Average(s => s.OverallRate)
                    })
                    .OrderByDescending(v => v.AverageRating)
                    .Take(5)
                    .ToListAsync();

                if (topVendors == null || topVendors.Count == 0)
                {
                    return NotFound("No vendors found.");
                }

                var result = topVendors.Select(v => new
                {
                    v.Vendor.CompanyName,
                    v.Vendor.ProfilePic,
                    AverageRating = v.AverageRating.HasValue ? v.AverageRating.Value : 0.0
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }
        }





    }
}
