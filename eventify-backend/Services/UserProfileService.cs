using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Helpers;
using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using System.Numerics;

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

        public async Task<bool> UpdateAvatarAsync(Guid userId, string imageUrl)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    throw new Exception("User not found!");

                user.ProfilePic = imageUrl;
                await _appDbContext.SaveChangesAsync();

                return true; // Indicate success
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new Exception($"Error updating avatar: {ex.Message}");
            }
        }

        public async Task<string> DeleteUserImageAsync(Guid userId)
        {
            try
            {
                var user = _appDbContext.Users.FirstOrDefault(u => u.UserId == userId);

                if (user == null)
                    throw new Exception("User not found!");

                var imageUrl = user.ProfilePic;

                user.ProfilePic = "";
                await _appDbContext.SaveChangesAsync();

                return imageUrl;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<UserDetailsDTO> GetUserDetailsAsync(Guid userId)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    throw new Exception("User not found!");

                UserDetailsDTO userDetails = new UserDetailsDTO();

                userDetails.Email = user.Email;
                userDetails.PhoneNumber = user.Phone;
                userDetails.HouseNo = user.HouseNo;
                userDetails.Street = user.Street;
                userDetails.Road = user.Road;
                userDetails.City = user.City;

                if (user.Role == "Admin")
                {
                    return userDetails;
                }
                else if (user.Role == "Client")
                {
                    var client = await _appDbContext.Clients.FirstOrDefaultAsync(u => u.UserId == userId);

                    if (client == null)
                        throw new Exception("Client not found!");

                    userDetails.FirstName = client.FirstName;
                    userDetails.LastName = client.LastName;

                    return userDetails;

                }
                else if (user.Role == "Vendor")
                {
                    var vendor = await _appDbContext.Vendors.FirstOrDefaultAsync(u => u.UserId == userId);

                    if (vendor == null)
                        throw new Exception("Vendor not found!");

                    userDetails.CompanyName = vendor.CompanyName;
                    userDetails.ContactPersonName = vendor.ContactPersonName;

                    return userDetails;
                }
                else
                {
                    throw new Exception("Invalid user role!");

                }

            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
        }

        public async Task<bool> UpdateUserDetailsAsync(Guid userId, String role, UserDetailsDTO userDetails)
        {
            try
            {
                if (role == "Client")
                {
                    var client = await _appDbContext.Clients.FirstOrDefaultAsync(u => u.UserId == userId);

                    if (client == null)
                        throw new Exception("Invalid user!");

                    client.FirstName = userDetails.FirstName;
                    client.LastName = userDetails.LastName;
                    client.Email = userDetails.Email;
                    client.Phone = userDetails.PhoneNumber;
                    client.City = userDetails.City;
                    client.Street = userDetails.Street;
                    client.Road = userDetails.Road;
                    client.HouseNo = userDetails.HouseNo;

                    await _appDbContext.SaveChangesAsync();

                    return true;


                }
                else if (role == "Vendor")
                {
                    var vendor = await _appDbContext.Vendors.FirstOrDefaultAsync(u => u.UserId == userId);

                    if (vendor == null)
                        throw new Exception("Invalid user!");

                    vendor.CompanyName = userDetails.CompanyName;
                    vendor.ContactPersonName = userDetails.ContactPersonName;
                    vendor.Email = userDetails.Email;
                    vendor.Phone = userDetails.PhoneNumber;
                    vendor.City = userDetails.City;
                    vendor.Street = userDetails.Street;
                    vendor.Road = userDetails.Road;
                    vendor.HouseNo = userDetails.HouseNo;

                    await _appDbContext.SaveChangesAsync();

                    return true;


                }
                else if (role == "Admin")
                {
                    var admin = await _appDbContext.Vendors.FirstOrDefaultAsync(u => u.UserId == userId);

                    if (admin == null)
                        throw new Exception("Invalid user!");

                    admin.Email = userDetails.Email;
                    admin.Phone = userDetails.PhoneNumber;
                    admin.City = userDetails.City;
                    admin.Street = userDetails.Street;
                    admin.Road = userDetails.Road;
                    admin.HouseNo = userDetails.HouseNo;

                    await _appDbContext.SaveChangesAsync();

                    return true;

                }
                else
                {
                    return false;
                }

            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }

        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            using (var transaction = await _appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                    if (user == null)
                    {
                        // Log the attempt to delete a non-existent user
                        return false;
                    }

                    switch (user.Role)
                    {
                        case "Client":
                            var client = await _appDbContext.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
                            if (client != null)
                            {
                                _appDbContext.Clients.Remove(client);
                            }
                            break;

                        case "Vendor":
                            var vendor = await _appDbContext.Vendors.FirstOrDefaultAsync(v => v.UserId == userId);
                            if (vendor != null)
                            {
                                _appDbContext.Vendors.Remove(vendor);
                            }
                            break;

                        case "Admin":
                            _appDbContext.Users.Remove(user);
                            break;

                        default:
                            return false;
                    }

                    await _appDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    // Log the exception message
                    return false;
                }
            }
        }


        public async Task<(bool Success, string Message)> UpdatePasswordAsync(Guid userId, UpdatePasswordDTO updatePasswordDTO)
        {
            try
            {
                var user = await _appDbContext.Users.FindAsync(userId);

                if (user == null)
                    return (false, "User not found!");

                if (!PasswordHasher.VerifyPassword(updatePasswordDTO.CurrentPassword,user.Password))
                    return (false, "Current password is incorrect!");

                if (updatePasswordDTO.NewPassword != updatePasswordDTO.ConfirmPassword)
                    return (false, "New password and confirm password do not match!");

                user.Password = PasswordHasher.HashPassword(updatePasswordDTO.NewPassword);
                _appDbContext.Entry(user).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();

                return (true, "Password updated successfully!");
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
