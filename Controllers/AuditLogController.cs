using ForMiraiProject.Models;
using ForMiraiProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForMiraiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly I_AuditLogService _auditLogService;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(I_AuditLogService auditLogService, ILogger<AuditLogController> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        // บันทึก Audit Log ใหม่
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAuditLogAsync([FromBody] AuditLog auditLog)
        {
            if (auditLog == null)
            {
                _logger.LogWarning("Invalid audit log attempt: audit log object is null.");
                return BadRequest("Audit log details cannot be null.");
            }

            try
            {
                await _auditLogService.CreateAuditLogAsync(auditLog);
                return Ok(new { Message = "Audit log created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the audit log.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        // ดูข้อมูล Audit Log โดย ID
        [HttpGet]
        [Route("details/{auditLogId}")]
        public async Task<IActionResult> GetAuditLogByIdAsync(Guid auditLogId)
        {
            if (auditLogId == Guid.Empty)
            {
                return BadRequest("Invalid audit log ID.");
            }

            try
            {
                var auditLog = await _auditLogService.GetAuditLogByIdAsync(auditLogId);
                if (auditLog == null)
                {
                    return NotFound("Audit log not found.");
                }

                return Ok(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching audit log details for ID: {auditLogId}");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ดูข้อมูล Audit Logs ทั้งหมด
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllAuditLogsAsync()
        {
            try
            {
                var auditLogs = await _auditLogService.GetAllAuditLogsAsync();
                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all audit logs.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ค้นหา Audit Logs โดย UserId
        [HttpGet]
        [Route("user/{userId}")]
        public async Task<IActionResult> GetAuditLogsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var auditLogs = await _auditLogService.GetAuditLogsByUserIdAsync(userId);
                if (!auditLogs.Any())
                {
                    return NotFound("No audit logs found for this user.");
                }

                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching audit logs for user ID: {userId}");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ค้นหา Audit Logs ในช่วงเวลา
        [HttpGet]
        [Route("date-range")]
        public async Task<IActionResult> GetAuditLogsByDateRangeAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest("Invalid date range.");
            }

            try
            {
                var auditLogs = await _auditLogService.GetAuditLogsByDateRangeAsync(startDate, endDate);
                if (!auditLogs.Any())
                {
                    return NotFound("No audit logs found for the given date range.");
                }

                return Ok(auditLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching audit logs for date range: {startDate} to {endDate}");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ลบ Audit Log ตาม ID
        [HttpDelete]
        [Route("delete/{auditLogId}")]
        public async Task<IActionResult> DeleteAuditLogAsync(Guid auditLogId)
        {
            if (auditLogId == Guid.Empty)
            {
                return BadRequest("Invalid audit log ID.");
            }

            try
            {
                var result = await _auditLogService.DeleteAuditLogAsync(auditLogId);
                if (result)
                {
                    _logger.LogInformation($"Audit log with ID: {auditLogId} has been successfully deleted.");
                    return Ok(new { Message = "Audit log deleted successfully." });
                }

                return NotFound("Audit log not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting audit log with ID: {auditLogId}");
                return StatusCode(500, "Internal server error.");
            }
        }

        // ลบ Audit Logs เก่ากว่า X วัน
        [HttpDelete]
        [Route("delete-old/{daysOld}")]
        public async Task<IActionResult> DeleteOldAuditLogsAsync(int daysOld)
        {
            if (daysOld <= 0)
            {
                return BadRequest("Invalid number of days.");
            }

            try
            {
                var result = await _auditLogService.DeleteOldAuditLogsAsync(daysOld);
                if (result)
                {
                    _logger.LogInformation($"Audit logs older than {daysOld} days have been successfully deleted.");
                    return Ok(new { Message = "Old audit logs deleted successfully." });
                }

                return NotFound("No old audit logs found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting audit logs older than {daysOld} days.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
