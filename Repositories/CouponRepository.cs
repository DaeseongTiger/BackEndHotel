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
    public class CouponRepository : GenericRepository<Coupon>, I_CouponRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CouponRepository> _logger;

        public CouponRepository(AppDbContext context, ILogger<CouponRepository> logger) : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// ดึงคูปองที่ยังใช้งานได้
        /// </summary>
        public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
        {
            try
            {
                return await _context.Coupons
                    .Where(c => c.IsActive && c.ExpiryDate > DateTime.UtcNow)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active coupons.");
                throw new InvalidOperationException("An error occurred while retrieving active coupons.", ex);
            }
        }

        /// <summary>
        /// ดึงคูปองที่หมดอายุ
        /// </summary>
        public async Task<IEnumerable<Coupon>> GetExpiredCouponsAsync()
        {
            try
            {
                return await _context.Coupons
                    .Where(c => !c.IsActive || c.ExpiryDate <= DateTime.UtcNow)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching expired coupons.");
                throw new InvalidOperationException("An error occurred while retrieving expired coupons.", ex);
            }
        }

        /// <summary>
        /// ดึงคูปองตามรหัส
        /// </summary>
        public async Task<Coupon?> GetCouponByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Coupon code cannot be empty", nameof(code));

            try
            {
                return await _context.Coupons
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching coupon by code {Code}.", code);
                throw new InvalidOperationException("An error occurred while retrieving the coupon.", ex);
            }
        }

        /// <summary>
        /// ตรวจสอบว่าคูปองใช้ได้และไม่ซ้ำซ้อน
        /// </summary>
        public async Task<bool> IsCouponValidAsync(string code, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Coupon code cannot be empty", nameof(code));

            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid User ID", nameof(userId));

            try
            {
                var coupon = await GetCouponByCodeAsync(code);
                if (coupon == null || !coupon.IsActive || coupon.ExpiryDate <= DateTime.UtcNow)
                    return false;

                return !await _context.Bookings
                    .AnyAsync(b => b.UserId == userId && b.CouponId == coupon.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating coupon {Code} for User ID {UserId}.", code, userId);
                throw new InvalidOperationException("An error occurred while validating the coupon.", ex);
            }
        }

        /// <summary>
        /// ตรวจสอบข้อจำกัดการใช้งานคูปอง
        /// </summary>
        public async Task<bool> IsCouponRestrictedAsync(string code, Guid userId)
        {
            // สมมติว่าคูปองมีการจำกัดการใช้งานสำหรับผู้ใช้บางคน
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Coupon code cannot be empty", nameof(code));

            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid User ID", nameof(userId));

            try
            {
                var coupon = await GetCouponByCodeAsync(code);
                return coupon != null && coupon.RestrictedUserIds.Contains(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking coupon restrictions for {Code} and User ID {UserId}.", code, userId);
                throw new InvalidOperationException("An error occurred while checking coupon restrictions.", ex);
            }
        }

        /// <summary>
        /// ปิดใช้งานคูปอง
        /// </summary>
        public async Task<bool> DeactivateCouponAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Coupon code cannot be empty", nameof(code));

            try
            {
                var coupon = await GetCouponByCodeAsync(code);
                if (coupon == null || !coupon.IsActive)
                    return false;

                coupon.IsActive = false;
                _context.Coupons.Update(coupon);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating coupon {Code}.", code);
                throw new InvalidOperationException("An error occurred while deactivating the coupon.", ex);
            }
        }

        /// <summary>
        /// ใช้คูปองและบันทึกสถานะ
        /// </summary>
        public async Task<bool> RedeemCouponAsync(string code, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Coupon code cannot be empty", nameof(code));

            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid User ID", nameof(userId));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var coupon = await GetCouponByCodeAsync(code);
                if (coupon == null || !coupon.IsActive || coupon.ExpiryDate <= DateTime.UtcNow)
                    return false;

                // ตัวอย่าง: บันทึกการใช้คูปอง
                var booking = new Booking
                {
                    UserId = userId,
                    CouponId = coupon.Id,
                    CreatedAt = DateTime.UtcNow,
                    
                };

                await _context.Bookings.AddAsync(booking);
                coupon.IsActive = false; // ปิดการใช้งานคูปอง
                _context.Coupons.Update(coupon);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error redeeming coupon {Code} for User ID {UserId}.", code, userId);
                throw new InvalidOperationException("An error occurred while redeeming the coupon.", ex);
            }
        }

        // Validate Coupon Logic
        private void ValidateCoupon(Coupon coupon)
        {
            if (string.IsNullOrWhiteSpace(coupon.Code))
                throw new ArgumentException("Coupon code cannot be empty.", nameof(coupon.Code));

            if (coupon.DiscountPercentage < 0 || coupon.DiscountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 0 and 100.", nameof(coupon.DiscountPercentage));

            if (coupon.ExpiryDate <= DateTime.UtcNow)
                throw new ArgumentException("Expiry date must be in the future.", nameof(coupon.ExpiryDate));
        }

        public async Task<IEnumerable<Coupon>> GetAllAsync()
{
    try
    {
        return await _context.Coupons
            .AsNoTracking()
            .ToListAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching all coupons.");
        throw new InvalidOperationException("An error occurred while retrieving all coupons.", ex);
    }
}

public async Task CreateAsync(Coupon coupon)
{
    if (coupon == null)
        throw new ArgumentNullException(nameof(coupon));

    try
    {
        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating coupon.");
        throw new InvalidOperationException("An error occurred while creating the coupon.", ex);
    }
}




public async Task<Coupon?> GetByCodeAsync(string code)
{
    if (string.IsNullOrWhiteSpace(code))
        throw new ArgumentException("Coupon code cannot be empty", nameof(code));

    try
    {
        return await _context.Coupons
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching coupon by code {Code}.", code);
        throw new InvalidOperationException("An error occurred while retrieving the coupon.", ex);
    }
}



public async Task<Coupon> UpdateAsync(Guid id, Coupon updatedCoupon, CancellationToken cancellationToken)
{
    var existingCoupon = await _context.Coupons.FindAsync(id);

    if (existingCoupon == null)
    {
        throw new KeyNotFoundException("Coupon not found.");
    }

    existingCoupon.Code = updatedCoupon.Code;
    existingCoupon.DiscountAmount = updatedCoupon.DiscountAmount;
    existingCoupon.ExpirationDate = updatedCoupon.ExpirationDate;
    existingCoupon.IsUsed = updatedCoupon.IsUsed;

    await _context.SaveChangesAsync(cancellationToken);

    // คืนค่าคูปองที่อัปเดตแล้ว
    return existingCoupon;
}





    }
}
