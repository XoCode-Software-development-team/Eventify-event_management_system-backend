using eventify_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Services
{
    public class UserProfileService
    {
        private readonly AppDbContext _appDbContext;

        public UserProfileService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<string> GetUserAvatarAsync(Guid userId)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

                if (user == null)
                {
                    throw new Exception("Invalid user");
                }

                return user.ProfilePic;
            }

            catch (Exception ex) 
            {
                throw new Exception(ex.ToString());
            }

        }
    }
}
