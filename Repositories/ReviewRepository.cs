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
    public class ReviewRepository : GenericRepository<Review>, I_ReviewRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(AppDbContext context, ILogger<ReviewRepository> logger) : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Review>> GetReviewsByHotelIdAsync(Guid hotelId)
        {
            if (hotelId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Hotel ID provided for fetching reviews.");
                throw new ArgumentException("Invalid Hotel ID", nameof(hotelId));
            }

            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.HotelId == hotelId)
                    .Include(r => r.User) // โหลดข้อมูลผู้ใช้ที่รีวิว
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("{Count} review(s) retrieved for Hotel ID {HotelId}.", reviews.Count, hotelId);
                return reviews;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reviews for Hotel ID {HotelId}.", hotelId);
                throw;
            }
        }

public async Task<Review?> GetReviewByUserAsync(Guid userId, Guid hotelId)
{
    if (userId == Guid.Empty || hotelId == Guid.Empty)
        throw new ArgumentException("Invalid User ID or Hotel ID");

    var review = await _context.Reviews
        .Where(r => r.UserId == userId && r.HotelId == hotelId)
        .Include(r => r.User)  // โหลดข้อมูล User ที่เกี่ยวข้อง
        .Include(r => r.Hotel) // โหลดข้อมูล Hotel ที่เกี่ยวข้อง
        .AsNoTracking() // ไม่ต้องติดตามการเปลี่ยนแปลง (ทำให้ประหยัดทรัพยากร)
        .FirstOrDefaultAsync(); // ดึงข้อมูลรีวิวที่ตรงกับเงื่อนไขหรือคืนค่า null หากไม่พบ

    return review; // คืนค่า review หรือ null
}


        public async Task<bool> IsReviewExistsAsync(Guid userId, Guid hotelId)
        {
            if (userId == Guid.Empty || hotelId == Guid.Empty)
            {
                _logger.LogWarning("Invalid User ID or Hotel ID provided for review existence check.");
                throw new ArgumentException("Invalid User ID or Hotel ID");
            }

            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.HotelId == hotelId);
        }

        public async Task<bool> HasUserReviewedHotelAsync(Guid userId, Guid hotelId)
        {
            if (userId == Guid.Empty || hotelId == Guid.Empty)
            {
                throw new ArgumentException("Invalid User ID or Hotel ID");
            }
            return await _context.Reviews.AnyAsync(r => r.UserId == userId && r.HotelId == hotelId);
        }

        public  async Task AddAsync(Review entity)
        {
            if (entity.Rating < 1 || entity.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            if (string.IsNullOrWhiteSpace(entity.Comment))
                throw new ArgumentException("Comment cannot be empty");

            if (await IsReviewExistsAsync(entity.UserId, entity.HotelId))
                throw new InvalidOperationException("You have already reviewed this hotel");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await base.AddAsync(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error adding review for User ID {UserId} and Hotel ID {HotelId}.", entity.UserId, entity.HotelId);
                throw;
            }
        }

        public  async Task UpdateAsync(Review entity)
        {
            if (entity.Rating < 1 || entity.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            if (string.IsNullOrWhiteSpace(entity.Comment))
                throw new ArgumentException("Comment cannot be empty");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await base.UpdateAsync(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating review for Review ID {ReviewId}.", entity.Id);
                throw;
            }
        }

        public async Task CreateReviewAsync(Review review)
{
    // ตรวจสอบก่อนว่ารีวิวนี้มีอยู่แล้วหรือไม่ (optional step)
    var existingReview = await _context.Reviews
                                       .FirstOrDefaultAsync(r => r.HotelId == review.HotelId && r.UserId == review.UserId);

    if (existingReview != null)
    {
        throw new InvalidOperationException("The user has already reviewed this hotel.");
    }

    // เพิ่มรีวิวใหม่ลงในฐานข้อมูล
    await _context.Reviews.AddAsync(review);
    await _context.SaveChangesAsync();
}

public async Task DeleteReviewAsync(Guid reviewId)
{
    // ค้นหาผ่าน ReviewId เพื่อหาว่ารีวิวที่ต้องการลบมีอยู่ในฐานข้อมูลหรือไม่
    var review = await _context.Reviews.FindAsync(reviewId);

    if (review == null)
    {
        throw new KeyNotFoundException("Review not found.");
    }

    // ลบรีวิวจากฐานข้อมูล
    _context.Reviews.Remove(review);
    await _context.SaveChangesAsync();
}


public async Task UpdateReviewAsync(Review review)
{
    // ค้นหารีวิวที่ต้องการอัปเดต
    var existingReview = await _context.Reviews.FindAsync(review.Id);

    if (existingReview == null)
    {
        throw new KeyNotFoundException("Review not found.");
    }

    // อัปเดตข้อมูลของรีวิว
    existingReview.Rating = review.Rating;
    existingReview.Comment = review.Comment;
    existingReview.UpdatedAt = DateTime.UtcNow;

    // บันทึกการเปลี่ยนแปลงในฐานข้อมูล
    _context.Reviews.Update(existingReview);
    await _context.SaveChangesAsync();
}

    }
}
