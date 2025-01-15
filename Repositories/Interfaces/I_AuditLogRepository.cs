using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories;

namespace ForMiraiProject.Repositories
{
    public interface I_AuditLogRepository
    {
        Task AddAuditLogAsync(AuditLog auditLog);  // เพิ่มการทำงานที่ต้องใช้ async
        Task<List<AuditLog>> GetAuditLogsAsync();  // ค้นหาข้อมูล AuditLog ทั้งหมด
        Task<AuditLog?> GetAuditLogByIdAsync(Guid id);  // ค้นหาด้วย Id
        Task<List<AuditLog>?> GetAuditLogsByUserIdAsync(Guid userId);  // ค้นหาด้วย UserId
        Task<List<AuditLog>?> GetAuditLogsByActionTypeAsync(string actionType);  // ค้นหาด้วย ActionType
        Task SaveAsync();  // วิธีบันทึกข้อมูล
    }
}
