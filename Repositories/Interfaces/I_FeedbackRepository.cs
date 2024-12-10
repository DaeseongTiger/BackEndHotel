using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;
using ForMiraiProject.Services.Interfaces; // สำหรับ I_FeedbackService
using ForMiraiProject.DTOs; 

namespace ForMiraiProject.Repositories.Interfaces
{
    public interface I_FeedbackRepository : I_GenericRepository<Feedback>
    {
        Task<IEnumerable<Feedback>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Feedback>> GetFeedbackByUserIdAsync(Guid userId);            // ดึงข้อเสนอแนะตามผู้ใช้
        Task<IEnumerable<Feedback>> GetFeedbackByHotelIdAsync(Guid hotelId);          // ดึงข้อเสนอแนะตามโรงแรม
        Task<bool> IsFeedbackExistsAsync(Guid userId, Guid hotelId);                  // ตรวจสอบว่าผู้ใช้ได้ให้ข้อเสนอแนะแล้ว
    }
}
