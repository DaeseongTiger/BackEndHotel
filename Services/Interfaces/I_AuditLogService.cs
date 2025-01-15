using ForMiraiProject.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForMiraiProject.Services.Interfaces
{
    public interface I_AuditLogService
    {
        // บันทึกการทำ Audit Log ใหม่
        Task CreateAuditLogAsync(AuditLog auditLog);

        // ดึงข้อมูล Audit Log ตาม ID
        Task<AuditLog> GetAuditLogByIdAsync(Guid auditLogId);

        // ดึงข้อมูล Audit Logs ทั้งหมด (สามารถใส่เงื่อนไขในการค้นหาได้)
        Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync();

        // ดึงข้อมูล Audit Logs โดย UserId (สามารถกรองตามผู้ใช้)
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserIdAsync(Guid userId);

        // ดึงข้อมูล Audit Logs ระหว่างช่วงเวลา (เริ่มต้น - สิ้นสุด)
        Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate);

        // ลบ Audit Log ตาม ID
        Task<bool> DeleteAuditLogAsync(Guid auditLogId);

        // ลบ Audit Logs ที่เก่ากว่า X วัน
        Task<bool> DeleteOldAuditLogsAsync(int daysOld);

        // บันทึกการทำ Audit Log พร้อมข้อมูลที่เกี่ยวข้อง (สำหรับการอัปเดตข้อม

        Task CreateAuditLogForUpdateAsync(
        Guid userId,
        string tableName,
        string fieldName,
        string oldValue,
        string newValue,
        string actionType,
        string additionalInfo);
    }
}
