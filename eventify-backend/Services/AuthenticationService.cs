using eventify_backend.Data;
using eventify_backend.Helpers;
using eventify_backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace eventify_backend.Services
{
    public class AuthenticationService
    {
        private readonly AppDbContext _appDbContext;

        public AuthenticationService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AuthenticationAsync(User userObj)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userObj.Email);

                if (user == null)
                    throw new Exception("User not found!");

                if (user.Password != null && userObj.Password != null)
                {

                    if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
                        throw new Exception("Password is incorrect!");
                }
            }

            catch (Exception ex)
            {
                throw new Exception($"Error occurred while login user: {ex.Message}");
            }

        }

        public async Task<bool> RegisterClientAsync(Client clientObj)
        {
            try
            {
                // Check if the email address exists
                if (await CheckEmailAddressExistAsync(clientObj.Email))
                {
                    throw new Exception("Email address already exists!");
                }

                if (clientObj.Password != null)
                {
                    // Check password strength
                    var pass = CheckPasswordStrength(clientObj.Password);
                    if (!string.IsNullOrEmpty(pass))
                        throw new Exception(pass.ToString());

                    if (clientObj.Password != null)
                        clientObj.Password = PasswordHasher.HashPassword(clientObj.Password);

                    await _appDbContext.Clients.AddAsync(clientObj);
                    await _appDbContext.SaveChangesAsync();

                    return true;
                }
                return false;
            }

            catch (Exception ex)
            {
                throw new Exception($"Error occurred while Registering user: {ex.Message}");
            }
        }

        public async Task<bool> RegisterVendorAsync(Vendor vendorObj)
        {
            try
            {
                // Check if the company name exists
                if (await CheckCompanyNameExistAsync(vendorObj.CompanyName))
                {
                    throw new Exception("Company name already exists!"); // Throwing an exception instead of returning BadRequest
                }
                // Check if the email address exists
                if (await CheckEmailAddressExistAsync(vendorObj.Email))
                {
                    throw new Exception("Email address already exists!");
                }

                if (vendorObj.Password != null)
                {
                    // Check password strength
                    var pass = CheckPasswordStrength(vendorObj.Password);
                    if (!string.IsNullOrEmpty(pass))
                        throw new Exception(pass.ToString());


                    vendorObj.Password = PasswordHasher.HashPassword(vendorObj.Password);

                    await _appDbContext.Vendors.AddAsync(vendorObj);
                    await _appDbContext.SaveChangesAsync();

                    return true; // Registration succeeded
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                throw new Exception($"Error occurred while Registering vendor: {ex.Message}");
            }
        }

        private Task<bool> CheckCompanyNameExistAsync(string? companyName)
            => _appDbContext.Vendors.AnyAsync(v => v.CompanyName == companyName);

        private Task<bool> CheckEmailAddressExistAsync(string? emailAddress)
            => _appDbContext.Users.AnyAsync(u => u.Email == emailAddress);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
                sb.Append("Minimum password lenght should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be alphannumeric" + Environment.NewLine);
            if (!Regex.IsMatch(password, "[`,~,!,@,#,$,%,^,&,*,(,),_,-,+,=,{,[,},},|,\\,:,;,\",',<,,,>,.,?,/]"))
                sb.Append("Password should contain special characters" + Environment.NewLine);
            return sb.ToString();
        }

    }
}
