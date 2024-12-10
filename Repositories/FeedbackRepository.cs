using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Data;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;

namespace ForMiraiProject.Repositories
{
    public class FeedbackRepository : GenericRepository<Feedback>, I_FeedbackRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FeedbackRepository> _logger;

        public FeedbackRepository(AppDbContext context, ILogger<FeedbackRepository> logger) : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ดึงข้อเสนอแนะตามผู้ใช้
        public async Task<IEnumerable<Feedback>> GetFeedbackByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid User ID provided for fetching feedback.");
                throw new ArgumentException("Invalid User ID", nameof(userId));
            }

            try
            {
                var feedbacks = await _context.Feedbacks
                    .Where(f => f.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("{Count} feedback(s) retrieved for User ID {UserId}.", feedbacks.Count, userId);
                return feedbacks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching feedback for User ID {UserId}.", userId);
                throw;
            }
        }

        // ดึงข้อเสนอแนะตามโรงแรม
        public async Task<IEnumerable<Feedback>> GetFeedbackByHotelIdAsync(Guid hotelId)
        {
            if (hotelId == Guid.Empty)
                throw new ArgumentException("Invalid Hotel ID");

            return await _context.Feedbacks // ใช้ 'Feedbacks' ที่ถูกต้อง
                .Where(f => f.HotelId == hotelId)
                .AsNoTracking()
                .ToListAsync();
        }

        // ตรวจสอบว่าผู้ใช้ได้ให้ข้อเสนอแนะแล้ว
        public async Task<bool> IsFeedbackExistsAsync(Guid userId, Guid hotelId)
        {
            if (userId == Guid.Empty || hotelId == Guid.Empty)
            {
                _logger.LogWarning("Invalid User ID or Hotel ID provided for feedback existence check.");
                throw new ArgumentException("Invalid User ID or Hotel ID");
            }

            try
            {
                var exists = await _context.Feedbacks // ใช้ 'Feedbacks' ที่ถูกต้อง
                    .AnyAsync(f => f.UserId == userId && f.HotelId == hotelId);

                _logger.LogInformation("Feedback existence check for User ID {UserId} and Hotel ID {HotelId}: {Exists}.", userId, hotelId, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking feedback existence for User ID {UserId} and Hotel ID {HotelId}.", userId, hotelId);
                throw;
            }
        }

        // เพิ่ม Validation เมื่อเพิ่มข้อเสนอแนะใหม่
        public async Task AddAsync(Feedback entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Content)) // เปลี่ยนเป็น Content ตามที่โมเดล Feedback กำหนด
            {
                _logger.LogWarning("Attempted to add feedback with an empty content.");
                throw new ArgumentException("Feedback content cannot be empty", nameof(entity.Content));
            }

            if (await IsFeedbackExistsAsync(entity.UserId, entity.HotelId))
            {
                _logger.LogWarning("Duplicate feedback attempt for User ID {UserId} and Hotel ID {HotelId}.", entity.UserId, entity.HotelId);
                throw new InvalidOperationException("You have already provided feedback for this hotel.");
            }

            await base.AddAsync(entity);
            _logger.LogInformation("Feedback added successfully for User ID {UserId} and Hotel ID {HotelId}.", entity.UserId, entity.HotelId);
        }

        // อัปเดตข้อเสนอแนะ
        public  async Task UpdateAsync(Feedback entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Content)) // เปลี่ยนเป็น Content ตามที่โมเดล Feedback กำหนด
            {
                _logger.LogWarning("Attempted to update feedback with an empty content.");
                throw new ArgumentException("Feedback content cannot be empty", nameof(entity.Content));
            }

            await base.UpdateAsync(entity);
            _logger.LogInformation("Feedback updated successfully for Feedback ID {FeedbackId}.", entity.Id);
        }
    }
}
