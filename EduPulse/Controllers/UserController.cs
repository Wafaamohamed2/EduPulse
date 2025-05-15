using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduPulse.Models;
using EduPulse.Models.DTOs;
using EduPulse.Models.Services;
using EduPulse.Models.Users;
using Serilog;
using System.Security.Claims;

namespace EduPulse.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly SW_Entity _context;
        private readonly NotificationService _notificationService;

        public UserController(SW_Entity context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }
        
        
        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    ModelState,
                    "Validation errors",
                    400));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ApiResponse<object>(
                    false,
                    null,
                    "Invalid user ID",
                    401));
            }

            var user = await _context.Students.FirstOrDefaultAsync(u => u.Id == userId) as IUser
                ?? await _context.Teachers.FirstOrDefaultAsync(u => u.Id == userId) as IUser
                ?? await _context.Parents.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    null,
                    "User not found",
                    404));
            }

            // تحديث البيانات
            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Email))
            {
                // التحقق من أن الإيميل الجديد مش مستخدم من قبل
                if (await _context.Students.AnyAsync(u => u.Email == dto.Email && u.Id != userId) ||
                    await _context.Teachers.AnyAsync(u => u.Email == dto.Email && u.Id != userId) ||
                    await _context.Parents.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
                {
                    return BadRequest(new ApiResponse<object>(
                        false,
                        null,
                        "Email is already in use",
                        400));
                }
                user.Email = dto.Email.ToLower();
            }
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrEmpty(dto.ProfilePictureUrl))
                user.ProfilePictureUrl = dto.ProfilePictureUrl;

            try
            {
                await _context.SaveChangesAsync();

                
                await _notificationService.SendProfileUpdatedNotification(user.Email);

                return Ok(new ApiResponse<object>(
                    true,
                    null,
                    "Profile updated successfully",
                    200));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating profile for user {UserId}", userId);
                return StatusCode(500, new ApiResponse<object>(
                    false,
                    null,
                    $"Error updating profile: {ex.Message}",
                    500));
            }
        }

        // used to register the device token for push notifications
        [HttpPut("update-device")]
        [Authorize]
        public async Task<IActionResult> UpdateDevice([FromBody] UpdateDeviceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    ModelState,
                    "Validation errors",
                    400));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ApiResponse<object>(
                    false,
                    null,
                    "Invalid user ID",
                    401));
            }

            var device = await _context.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.IsActive == true);

            if (device == null)
            {
                device = new UserDevice { UserId = userId, DeviceToken = dto.DeviceToken, IsActive = true };
                _context.UserDevices.Add(device);
            }
            else
            {
                device.DeviceToken = dto.DeviceToken;
            }

            try
            {
                await _context.SaveChangesAsync();

                // (اختياري) إشعار عبر Firebase لو عايزة
                // await _notificationService.SendDeviceUpdatedNotification(userId, dto.DeviceToken);

                return Ok(new ApiResponse<object>(
                    true,
                    null,
                    "Device updated successfully",
                    200));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating device for user {UserId}", userId);
                return StatusCode(500, new ApiResponse<object>(
                    false,
                    null,
                    $"Error updating device: {ex.Message}",
                    500));
            }
        }
    }
}