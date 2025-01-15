using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories; 
using Microsoft.Extensions.Logging;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.Repositories.Interfaces;

namespace ForMiraiProject.Services
{
    public class CouponService : ICouponService
    {
        private readonly I_CouponRepository _couponRepository;
        private readonly ILogger<CouponService> _logger;

        public CouponService(I_CouponRepository couponRepository, ILogger<CouponService> logger)
        {
            _couponRepository = couponRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Coupon>> GetAllCouponsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all coupons.");
                return await _couponRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all coupons.");
                throw new Exception("An error occurred while retrieving coupons.", ex);
            }
        }

        public async Task<Coupon> GetCouponByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Fetching coupon with ID: {id}");
                var coupon = await _couponRepository.GetByIdAsync(id);
                if (coupon == null)
                {
                    throw new KeyNotFoundException("Coupon not found.");
                }

                return coupon;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching coupon with ID: {id}");
                throw new Exception("An error occurred while retrieving the coupon.", ex);
            }
        }

        // ลบเมธอด UpdateCouponAsync ซ้ำ
        public async Task UpdateCouponAsync(Guid id, Coupon updatedCoupon)
        {
            try
            {
                if (updatedCoupon == null)
                {
                    throw new ArgumentNullException(nameof(updatedCoupon));
                }

                var existingCoupon = await _couponRepository.GetByIdAsync(id);
                if (existingCoupon == null)
                {
                    throw new KeyNotFoundException("Coupon not found.");
                }

                ValidateCoupon(updatedCoupon);

                CancellationToken cancellationToken = new CancellationToken();

                _logger.LogInformation($"Updating coupon with ID: {id}");
                // ไม่ต้องเก็บค่าผลลัพธ์จาก UpdateAsync เพราะมันไม่คืนค่า
                await _couponRepository.UpdateAsync(id, updatedCoupon, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating coupon with ID: {id}");
                throw new Exception("An error occurred while updating the coupon.", ex);
            }
        }

        public async Task DeleteCouponAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Deleting coupon with ID: {id}");
                var existingCoupon = await _couponRepository.GetByIdAsync(id);
                if (existingCoupon == null)
                {
                    throw new KeyNotFoundException("Coupon not found.");
                }

                await _couponRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting coupon with ID: {id}");
                throw new Exception("An error occurred while deleting the coupon.", ex);
            }
        }

        public async Task<bool> ValidateCouponCodeAsync(string code)
{
    try
    {
        _logger.LogInformation($"Validating coupon code: {code}");
        Coupon? coupon = await _couponRepository.GetByCodeAsync(code);

        if (coupon == null || coupon.ExpirationDate < DateTime.UtcNow || coupon.IsUsed)
        {
            return false;
        }

        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error validating coupon code: {code}");
        throw new Exception("An error occurred while validating the coupon code.", ex);
    }
}


        private void ValidateCoupon(Coupon coupon)
        {
            if (string.IsNullOrWhiteSpace(coupon.Code))
            {
                throw new ArgumentException("Coupon code cannot be empty.");
            }

            if (coupon.DiscountAmount <= 0)
            {
                throw new ArgumentException("Discount amount must be greater than zero.");
            }

            if (coupon.ExpirationDate <= DateTime.UtcNow)
            {
                throw new ArgumentException("Expiration date must be in the future.");
            }
        }

        public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching active coupons.");
                var allCoupons = await _couponRepository.GetAllAsync();
                return allCoupons.Where(coupon => coupon.ExpirationDate > DateTime.UtcNow && !coupon.IsUsed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active coupons.");
                throw new Exception("An error occurred while retrieving active coupons.", ex);
            }
        }

        public async Task<IEnumerable<Coupon>> GetExpiredCouponsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching expired coupons.");
                var allCoupons = await _couponRepository.GetAllAsync();
                return allCoupons.Where(coupon => coupon.ExpirationDate <= DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching expired coupons.");
                throw new Exception("An error occurred while retrieving expired coupons.", ex);
            }
        }

        public async Task CreateCouponAsync(Coupon coupon)
{
    try
    {
        if (coupon == null)
        {
            throw new ArgumentNullException(nameof(coupon), "Coupon cannot be null.");
        }

        // การเรียกใช้เมธอด CreateAsync จาก repository เพื่อสร้างคูปองใหม่
        _logger.LogInformation("Creating a new coupon.");
        await _couponRepository.CreateAsync(coupon);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating coupon.");
        throw new Exception("An error occurred while creating the coupon.", ex);
    }
}

    }
}
