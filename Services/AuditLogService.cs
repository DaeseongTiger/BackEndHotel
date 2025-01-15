using ForMiraiProject.Models;
using ForMiraiProject.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForMiraiProject.Data;
using Microsoft.EntityFrameworkCore;
using ForMiraiProject.Repositories;

namespace ForMiraiProject.Services
{
    public class AuditLogService : I_AuditLogService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuditLogService> _logger;
        private readonly I_AuditLogRepository _auditLogRepository; // เพิ่มการประกาศ repository

        public AuditLogService(AppDbContext context, ILogger<AuditLogService> logger, I_AuditLogRepository auditLogRepository)
        {
            _context = context;
            _logger = logger;
            _auditLogRepository = auditLogRepository; // กำหนดค่า repository
        }

        // สร้าง Audit Log ใหม่
        public async Task CreateAuditLogAsync(AuditLog auditLog)
        {
            try
            {
                if (auditLog == null)
                {
                    throw new ArgumentNullException(nameof(auditLog), "Audit log cannot be null.");
                }

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Audit log created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the audit log.");
                throw new Exception("Error occurred while creating the audit log.", ex);
            }
        }

        // ดึงข้อมูล Audit Log ตาม ID
        public async Task<AuditLog> GetAuditLogByIdAsync(Guid auditLogId)
{
    try
    {
        if (auditLogId == Guid.Empty)
        {
            throw new ArgumentException("Invalid audit log ID.", nameof(auditLogId));
        }

        var auditLog = await _context.AuditLogs.FindAsync(auditLogId);

        if (auditLog == null)
        {
            // คืนค่าผลลัพธ์เริ่มต้นหรือโยนข้อผิดพลาด
            throw new KeyNotFoundException("ไม่พบ Audit Log ที่ต้องการ");
        }

        return auditLog;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"เกิดข้อผิดพลาดขณะดึงข้อมูล Audit Log สำหรับ ID: {auditLogId}");
        throw new Exception("เกิดข้อผิดพลาดขณะดึงข้อมูล Audit Log", ex);
    }
}
        // ดึงข้อมูล Audit Logs ทั้งหมด
        public async Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync()
        {
            try
            {
                var auditLogs = await _context.AuditLogs.ToListAsync();
                return auditLogs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all audit logs.");
                throw new Exception("Error occurred while fetching all audit logs.", ex);
            }
        }

        // ดึงข้อมูล Audit Logs ตาม UserId
        public async Task<IEnumerable<AuditLog>> GetAuditLogsByUserIdAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid user ID.", nameof(userId));
                }

                var auditLogs = await _context.AuditLogs
                    .Where(log => log.UserId == userId)
                    .ToListAsync();

                return auditLogs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching audit logs for user ID: {userId}");
                throw new Exception("Error occurred while fetching audit logs for the user.", ex);
            }
        }

        // ดึงข้อมูล Audit Logs ตามช่วงเวลา
        public async Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate == default || endDate == default)
                {
                    throw new ArgumentException("Invalid date range.");
                }

                var auditLogs = await _context.AuditLogs
                    .Where(log => log.Timestamp >= startDate && log.Timestamp <= endDate)
                    .ToListAsync();

                return auditLogs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching audit logs for date range: {startDate} to {endDate}");
                throw new Exception("Error occurred while fetching audit logs for the given date range.", ex);
            }
        }

        // ลบ Audit Log ตาม ID
        public async Task<bool> DeleteAuditLogAsync(Guid auditLogId)
        {
            try
            {
                if (auditLogId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid audit log ID.", nameof(auditLogId));
                }

                var auditLog = await _context.AuditLogs.FindAsync(auditLogId);
                if (auditLog == null)
                {
                    _logger.LogWarning($"Audit log with ID: {auditLogId} not found.");
                    return false;
                }

                _context.AuditLogs.Remove(auditLog);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting audit log with ID: {auditLogId}");
                throw new Exception("Error occurred while deleting the audit log.", ex);
            }
        }

        // ลบ Audit Logs เก่ากว่า X วัน
        public async Task<bool> DeleteOldAuditLogsAsync(int daysOld)
        {
            try
            {
                if (daysOld <= 0)
                {
                    throw new ArgumentException("Number of days must be greater than zero.", nameof(daysOld));
                }

                var thresholdDate = DateTime.UtcNow.AddDays(-daysOld);
                var oldAuditLogs = _context.AuditLogs
                    .Where(log => log.Timestamp < thresholdDate)
                    .ToList();

                if (oldAuditLogs.Any())
                {
                    _context.AuditLogs.RemoveRange(oldAuditLogs);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting audit logs older than {daysOld} days.");
                throw new Exception("Error occurred while deleting old audit logs.", ex);
            }
        }

        // สร้าง Audit Log สำหรับการอัพเดท
        public async Task CreateAuditLogForUpdateAsync(
            Guid id,
            string actionType,
            string userId,
            string description,
            string newValue,
            string oldValue,
            string status)
        {
            Guid userGuid = Guid.Parse(userId); // แปลง userId เป็น Guid
            // สร้าง AuditLog object
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(), // สร้าง ID ใหม่
                ActionType = actionType, // ประเภทของการทำงาน เช่น "update"
                UserId = userGuid, // รหัสผู้ใช้ที่กระทำการ
                Description = description, // คำอธิบายของการกระทำ
                NewValue = newValue, // ค่าปัจจุบันหลังการอัปเดต
                OldValue = oldValue, // ค่าก่อนหน้านี้
                Status = status, // สถานะ เช่น "success" หรือ "failure"
                CreatedAt = DateTime.UtcNow // เวลาที่บันทึกการกระทำนี้
            };

            try
            {
                // บันทึก Audit Log ลงในฐานข้อมูล
                await _auditLogRepository.AddAuditLogAsync(auditLog);
                await _auditLogRepository.SaveAsync(); // แน่ใจว่าได้บันทึกการเปลี่ยนแปลง
            }
            catch (Exception ex)
            {
                // จัดการข้อผิดพลาด
                Console.Error.WriteLine($"Error while saving audit log: {ex.Message}");
                throw new InvalidOperationException("Error while creating audit log.", ex);
            }
        }
    }
}
