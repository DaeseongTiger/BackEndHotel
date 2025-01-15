using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForMiraiProject.Data;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;

namespace ForMiraiProject.Repositories
{
    public class AuditLogRepository : I_AuditLogRepository
    {
        private readonly AppDbContext _dbContext;

        public AuditLogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // เพิ่ม AuditLog ลงในฐานข้อมูล
        public async Task AddAuditLogAsync(AuditLog auditLog)
        {
            await _dbContext.AuditLogs.AddAsync(auditLog);
            await SaveAsync();  // บันทึกข้อมูลหลังจากเพิ่ม
        }

        // ค้นหาทุก AuditLog
        public async Task<List<AuditLog>> GetAuditLogsAsync()
        {
            return await _dbContext.AuditLogs.ToListAsync();
        }

        // ค้นหา AuditLog โดยใช้ Id
        public async Task<AuditLog?> GetAuditLogByIdAsync(Guid id)
        {
            return await _dbContext.AuditLogs
                                   .Where(a => a.Id == id)
                                   .FirstOrDefaultAsync();
        }

        // ค้นหา AuditLog โดยใช้ UserId
        public async Task<List<AuditLog>?> GetAuditLogsByUserIdAsync(Guid userId)
        {
            return await _dbContext.AuditLogs
                                   .Where(a => a.UserId == userId)
                                   .ToListAsync();
        }

        // ค้นหา AuditLog โดยใช้ ActionType
        public async Task<List<AuditLog>?> GetAuditLogsByActionTypeAsync(string actionType)
        {
            return await _dbContext.AuditLogs
                                   .Where(a => a.ActionType == actionType)
                                   .ToListAsync();
        }

        // บันทึกการเปลี่ยนแปลงทั้งหมดที่เกิดขึ้นใน DbContext
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
