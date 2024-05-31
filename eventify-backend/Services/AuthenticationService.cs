using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Helpers;
using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public async Task<string> AuthenticationAsync(User userObj)
        {
            try
            {
                // Fetch the user from the database using email
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userObj.Email);

                if (user == null)
                    throw new Exception("User not found!");

                // Check for null passwords and verify
                if (user.Password != null && userObj.Password != null)
                {
                    if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
                        throw new Exception("Password is incorrect!");
                }
                else
                {
                    throw new Exception("Password is missing.");
                }

                // Create token claims
                var tokenClam = new TokenClamDTO
                {
                    Id = user.UserId.ToString(),
                    Role = user.Role,
                };

                // Check role and fetch corresponding client or vendor information
                if (user.Role == "Client")
                {
                    var client = await _appDbContext.Clients.FirstOrDefaultAsync(c => c.UserId == user.UserId);
                    if (client != null)
                    {
                        tokenClam.Name = $"{client.FirstName} {client.LastName}";
                        var token = CreateJwtToken(tokenClam);
                        return token;
                    }
                    throw new Exception("Failed to generate token for client.");
                }
                else if (user.Role == "Vendor")
                {
                    var vendor = await _appDbContext.Vendors.FirstOrDefaultAsync(v => v.UserId == user.UserId);
                    if (vendor != null)
                    {
                        tokenClam.Name = vendor.CompanyName;
                        var token = CreateJwtToken(tokenClam);
                        return token;
                    }
                    throw new Exception("Failed to generate token for vendor.");
                }
                else
                {
                    throw new Exception("Invalid user role.");
                }
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                throw new Exception($"Error occurred while logging in user: {ex.Message}");
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

        private string CreateJwtToken(TokenClamDTO tokenClaim)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("VeryVeryVeryVeryVerySecreatKey>>>>>>>.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role,tokenClaim.Role),
                new Claim("name",tokenClaim.Name),
                new Claim("id",tokenClaim.Id)
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var token = JwtTokenHandler.CreateToken(tokenDescriptor);
            return JwtTokenHandler.WriteToken(token);

        }

    }
}
