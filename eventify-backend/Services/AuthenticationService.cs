﻿using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Helpers;
using eventify_backend.Models;
using eventify_backend.UtilityService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace eventify_backend.Services
{
    public class AuthenticationService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthenticationService(AppDbContext appDbContext, IConfiguration configuration, IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<TokenApiDTO> AuthenticationAsync(User userObj)
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

                var tokenClam = await CreateUserTokenClaim(user);

                user.Token = CreateJwtToken(tokenClam);
                user.RefreshToken = CreateRefreshToken();
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
                await _appDbContext.SaveChangesAsync();

                return new TokenApiDTO()
                {
                    AccessToken = user.Token,
                    RefreshToken = user.RefreshToken
                };
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                throw new Exception($"{ex.Message}");
            }
        }

        public async Task<TokenApiDTO> RefreshAsync(TokenApiDTO tokenApiDto)
        {
            try
            {
                string accessToken = tokenApiDto.AccessToken;
                string refreshToken = tokenApiDto.RefreshToken;

                var principal = GetPrincipalFromExpiredToken(accessToken);

                var id = principal.FindFirst("id")?.Value;

                if (id == null)
                    throw new Exception("Error!");

                var userId = Guid.Parse(id);

                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    throw new Exception("Error!");

                if (user.RefreshTokenExpiryTime <= DateTime.Now)
                    throw new Exception("Invalid request!");

                var tokenClam = await CreateUserTokenClaim(user);

                var newAccessToken = CreateJwtToken(tokenClam);
                var newRefreshToken = CreateRefreshToken();

                user.RefreshToken=newRefreshToken;
                await _appDbContext.SaveChangesAsync();

                return new TokenApiDTO()
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                };
            }

            catch (Exception ex)
            {
                throw new Exception($"Error occured while genereate new token: {ex.Message}");
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
                throw new Exception($"{ex.Message}");
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

        public async Task<bool> RegisterAdminAsync(User userObj)
        {
            try
            {
                // Check if the email address exists
                if (await CheckEmailAddressExistAsync(userObj.Email))
                {
                    throw new Exception("Email address already exists!");
                }

                if (userObj.Password != null)
                {
                    // Check password strength
                    var pass = CheckPasswordStrength(userObj.Password);
                    if (!string.IsNullOrEmpty(pass))
                        throw new Exception(pass.ToString());

                    if (userObj.Password != null)
                        userObj.Password = PasswordHasher.HashPassword(userObj.Password);

                    await _appDbContext.Users.AddAsync(userObj);
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
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credentials
            };

            var token = JwtTokenHandler.CreateToken(tokenDescriptor);
            return JwtTokenHandler.WriteToken(token);

        }

        private String CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = _appDbContext.Users.
                Any(a => a.RefreshToken == refreshToken);

            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("VeryVeryVeryVeryVerySecreatKey>>>>>>>.....");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is invalid Token");
            return principal;
        }

        private async Task<TokenClamDTO> CreateUserTokenClaim(User user)
        {
            try
            {
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
                    } else
                    {
                        throw new Exception("Client not found!");

                    }

                }
                else if (user.Role == "Vendor")
                {
                    var vendor = await _appDbContext.Vendors.FirstOrDefaultAsync(v => v.UserId == user.UserId);
                    if (vendor != null && vendor.CompanyName != null)
                    {
                        tokenClam.Name = vendor.CompanyName;

                    } else
                    {
                        throw new Exception("Vendor not found!");
                    }
                }
                else if (user.Role == "Admin")
                {
                    tokenClam.Name = "Administrator";
                }
                else
                {
                    throw new Exception("Invalid user role.");
                }

                return tokenClam;
            }

            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }

        public async Task SendEmailAsync(string email)
        {
            try
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(a => a.Email == email);
                if (user == null)
                    throw new Exception("Email doen't exist!");


                var tokenBytes = RandomNumberGenerator.GetBytes(64);
                var emailToken = Convert.ToBase64String(tokenBytes);
                user.ResetPasswordToken = emailToken;
                user.ResetPasswordTokenExpiryTime = DateTime.Now.AddMinutes(15);

                var resetEmail = new Email(email, "Reset password!!", ResetPasswordEmailBody.EmailStringBody(email, emailToken));

                _emailService.SendEmail(resetEmail);
                _appDbContext.Entry(user).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }

        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPaswordDTO resetPaswordDTO)
        {
            var newToken = resetPaswordDTO.EmailToken.Replace(" ", "+");
            var user = await _appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(a => a.Email == resetPaswordDTO.Email);

            if (user is null)
                return (false, "Email doesn't exist!");

            var tokenCode = user.ResetPasswordToken;
            DateTime emailTokenExpiry = user.ResetPasswordTokenExpiryTime;

            if (tokenCode != newToken || emailTokenExpiry < DateTime.Now)
                return (false, "Invalid reset link");

            user.Password = PasswordHasher.HashPassword(resetPaswordDTO.NewPassword);
            _appDbContext.Entry(user).State = EntityState.Modified;
            await _appDbContext.SaveChangesAsync();

            return (true, "Password reset successfully!");
        }

    }
}
