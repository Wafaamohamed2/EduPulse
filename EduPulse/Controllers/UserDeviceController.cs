using EduPulse.Models;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduPulse.Models.DTOs;

namespace EduPulse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDeviceController : ControllerBase
    {
        private readonly SW_Entity _context;
        public UserDeviceController(SW_Entity context)
        {
            _context = context;
        }

        // POST: api/UserDevice/Register
        [HttpPost("Register")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApiResponse<object>>> RegisterDevice([FromBody] UserDeviceDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.DeviceToken))
                {
                    return BadRequest(new ApiResponse<object>(
                    success: false,
                    data: null,
                    message: "Invalid device token",
                    statusCode: 400));
                
                }
                    

                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new ApiResponse<object>(

                       success: false,
                       data: null,
                       message: "Invalid user ID",
                       statusCode: 401
                    ));
                }

                var existingDevice = await _context.UserDevices
                    .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceToken == dto.DeviceToken && d.IsActive);

                if (existingDevice == null)
                {
                    var device = new UserDevice
                    {
                        UserId = userId,
                        DeviceToken = dto.DeviceToken
                    };

                    _context.UserDevices.Add(device);
                    await _context.SaveChangesAsync();
                }

                return Ok(new ApiResponse<object>(
                
                    success: true,
                    data: null,
                    message: "Device registered successfully",
                    statusCode: 200
                      ));
            }
            catch 
            {
                return StatusCode(500, new ApiResponse<object>
                
                    (
                    success: false,
                    data: null,
                    message: "An error occurred while registering the device",
                    statusCode: 500
                ));
            }
        }

        // POST: api/UserDevice/Unregister
        [HttpPost("Unregister")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApiResponse<object>>> UnregisterDevice([FromBody] UserDeviceDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.DeviceToken))
                {
                    return BadRequest(new ApiResponse<object>
                    (
                        success: false,
                        data: null,
                        message: "Invalid device token",
                        statusCode: 400
                    ));
                }

                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    (
                        success: false,
                        data: null,
                        message: "Invalid user ID",
                        statusCode: 401

                    ));
                }

                var device = await _context.UserDevices
                    .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceToken == dto.DeviceToken && d.IsActive);

                if (device == null)
                {
                    return NotFound(new ApiResponse<object>
                    (success: false,
                    data: null,
                    message: "Device not found",
                    statusCode: 404
                    ));
                }

                _context.UserDevices.Remove(device);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                (
                    success: true,
                data: null,
                message: "Device unregistered successfully",
                statusCode: 200
                ));
            }
            catch 
            {
                return StatusCode(500, new ApiResponse<object>
                (
                    success: false,
                    data: null,
                    message: "An error occurred while unregistering the device",
                    statusCode: 500

                ));
            }
        }
    }

   
}
