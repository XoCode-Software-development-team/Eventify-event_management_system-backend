using eventify_backend.Data;
using eventify_backend.DTOs;
using eventify_backend.Models;
using eventify_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace eventify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;

        public AuthenticationController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            try
            {
                if (userObj == null)
                {
                    return BadRequest();
                }

                var tokenApiDto = await _authenticationService.AuthenticationAsync(userObj);

                return Ok(new
                { 
                    Token = tokenApiDto,
                    Message = "Login success!"
                });
            }

            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }

        }

        [HttpPost("clientRegister")]
        public async Task<IActionResult> RegisterClient([FromBody]Client clientObj)
        {
            try
            {
                if (clientObj == null)
                    return BadRequest(new { Message = "Invalid vendor data!" });

                bool result = await _authenticationService.RegisterClientAsync(clientObj);
                if (result)
                {
                    return Ok(new { Message = "Client registered successfully!" });
                }
                else
                {
                    return BadRequest(new { Message = "Client registration failed!" });
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("vendorRegister")]
        public async Task<IActionResult> RegisterVendor([FromBody] Vendor vendorObj)
        {
            try
            {
                if (vendorObj == null)
                    return BadRequest(new { Message = "Invalid vendor data!" });

                bool result = await _authenticationService.RegisterVendorAsync(vendorObj);
                if (result)
                {
                    return Ok(new { Message = "Vendor registered successfully!" });
                }
                else
                {
                    return BadRequest(new { Message = "Vendor registration failed!" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("adminRegister")]
        public async Task<IActionResult> RegisterAdmin()
        {
            try
            {
                var adminObj = new User();

                adminObj.Email = "tharindumanoj2020@gmail.com";
                adminObj.Password = "Admin123*";
                adminObj.Role = "Admin";


                bool result = await _authenticationService.RegisterAdminAsync(adminObj);
                if (result)
                {
                    return Ok(new { Message = "Admin registered successfully!" });
                }
                else
                {
                    return BadRequest(new { Message = "Admin registration failed!" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, $"Internal server error: Admin already exist!");
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenApiDTO tokenApiDTO)
        {
            try
            {
                if (tokenApiDTO is null)
                    return BadRequest(new { Message = "Invalid client request!" });

                var newTokenApiDto = await _authenticationService.RefreshAsync(tokenApiDTO);

                return Ok(newTokenApiDto);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost("SendResetEmail/{email}")]
        public async Task<IActionResult> SendEmail([FromRoute] string email)
        {
            try
            {
                await _authenticationService.SendEmailAsync(email);


                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Email sent!"
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }

        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPaswordDTO resetPaswordDTO)
        {
            var result = await _authenticationService.ResetPasswordAsync(resetPaswordDTO);

            if (result.Success)
            {
                return Ok(new
                {
                    StatusCode = 200,
                    Message = result.Message
                });
            }
            else
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = result.Message
                });
            }
        }

    }
}
