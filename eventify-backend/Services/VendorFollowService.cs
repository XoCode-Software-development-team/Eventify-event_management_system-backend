using eventify_backend.Data;
using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Services
{
    public class VendorFollowService
    {
        private readonly AppDbContext _appDbContext;

        public VendorFollowService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> IsFollowAsync(int soRId, Guid userId)
        {
            try
            {
                // Find the VendorId associated with the given soRId
                var vendorId = await _appDbContext.ServiceAndResources
                                                  .Where(v => v.SoRId == soRId)
                                                  .Select(v => v.VendorId)
                                                  .FirstOrDefaultAsync();

                // Check if the user is following the vendor
                var isFollow = await _appDbContext.VendorFollows
                                                  .AnyAsync(x => x.ClientId == userId && x.VendorId == vendorId);

                return isFollow;
            }
            catch (Exception ex)
            {
                // Handle specific exceptions or log them for debugging
                throw new ApplicationException($"Error checking follow status: {ex.Message}", ex);
            }
        }

        public async Task<string> ToggleFollowAsync(int soRId, Guid userId)
        {
            try
            {
                var vendorId = await _appDbContext.ServiceAndResources
                    .Where(s => s.SoRId == soRId)
                    .Select(s => s.VendorId)
                    .FirstOrDefaultAsync();

                var isFollow = _appDbContext.VendorFollows
                    .Any(x => x.ClientId.Equals(userId) && x.VendorId == vendorId);

                if (isFollow)
                {
                    var vendorFollow = await _appDbContext.VendorFollows
                        .FirstOrDefaultAsync(x => x.ClientId.Equals(userId) && x.VendorId == vendorId);

                    if (vendorFollow != null)
                    {
                        _appDbContext.VendorFollows.Remove(vendorFollow);
                        await _appDbContext.SaveChangesAsync();
                    }

                    return "Vendor unfollowed successfully!";
                }
                else
                {
                    VendorFollow vendorFollow = new VendorFollow()
                    {
                        VendorId = vendorId,
                        ClientId = userId,
                    };

                    _appDbContext.VendorFollows.Add(vendorFollow);
                    await _appDbContext.SaveChangesAsync();

                    return "Vendor followed successfully!";
                }
            }
            catch (Exception ex)
            {
                // Handle specific exceptions or log them for debugging
                throw new ApplicationException($"Error toggling follow status: {ex.Message}", ex);
            }
        }



    }
}
