using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;

namespace ForMiraiProject.Services
{
    public interface ICouponService
    {
        /// <summary>
        /// ดึงข้อมูลคูปองทั้งหมด
        /// </summary>
        /// <returns>รายการคูปอง</returns>
        Task<IEnumerable<Coupon>> GetAllCouponsAsync();

        /// <summary>
        /// ดึงข้อมูลคูปองด้วย ID
        /// </summary>
        /// <param name="id">รหัสคูปอง</param>
        /// <returns>คูปองที่ตรงกับ ID</returns>
        Task<Coupon> GetCouponByIdAsync(Guid id);

        /// <summary>
        /// สร้างคูปองใหม่
        /// </summary>
        /// <param name="newCoupon">รายละเอียดคูปองใหม่</param>
        /// <returns>คูปองที่สร้างขึ้น</returns>
        Task CreateCouponAsync(Coupon coupon);

        /// <summary>
        /// อัปเดตข้อมูลคูปอง
        /// </summary>
        /// <param name="id">รหัสคูปอง</param>
        /// <param name="updatedCoupon">ข้อมูลคูปองที่แก้ไข</param>
        Task UpdateCouponAsync(Guid id, Coupon updatedCoupon);

        /// <summary>
        /// ลบคูปองด้วย ID
        /// </summary>
        /// <param name="id">รหัสคูปอง</param>
        Task DeleteCouponAsync(Guid id);

        /// <summary>
        /// ตรวจสอบว่ารหัสคูปองยังใช้งานได้หรือไม่
        /// </summary>
        /// <param name="code">รหัสคูปอง</param>
        /// <returns>สถานะความถูกต้องของคูปอง</returns>
        Task<bool> ValidateCouponCodeAsync(string code);

        Task<IEnumerable<Coupon>> GetActiveCouponsAsync();
        Task<IEnumerable<Coupon>> GetExpiredCouponsAsync();

        
    }
}
