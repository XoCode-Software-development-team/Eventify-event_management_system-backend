using eventify_backend.Models;
using eventify_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

                var token = await _authenticationService.AuthenticationAsync(userObj);

                return Ok(new
                { 
                    Token = token,
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

    }
}
