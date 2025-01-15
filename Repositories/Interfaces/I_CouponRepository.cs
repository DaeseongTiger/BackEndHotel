using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;

namespace ForMiraiProject.Repositories.Interfaces
{
    public interface I_CouponRepository : I_GenericRepository<Coupon>
    {
        /// <summary>
        /// ดึงคูปองที่ยังใช้งานได้
        /// </summary>
        Task<IEnumerable<Coupon>> GetActiveCouponsAsync();

        /// <summary>
        /// ดึงคูปองที่หมดอายุ
        /// </summary>
        Task<IEnumerable<Coupon>> GetExpiredCouponsAsync();

        /// <summary>
        /// ดึงคูปองตามรหัส
        /// </summary>
        Task<Coupon?> GetCouponByCodeAsync(string code);

        /// <summary>
        /// ตรวจสอบว่าคูปองใช้ได้และไม่ซ้ำซ้อน
        /// </summary>
        Task<bool> IsCouponValidAsync(string code, Guid userId);

        /// <summary>
        /// ตรวจสอบข้อจำกัดการใช้งานคูปอง
        /// </summary>
        Task<bool> IsCouponRestrictedAsync(string code, Guid userId);

        /// <summary>
        /// ปิดใช้งานคูปอง
        /// </summary>
        Task<bool> DeactivateCouponAsync(string code);

        /// <summary>
        /// ใช้คูปองและบันทึกสถานะ
        /// </summary>
        Task<bool> RedeemCouponAsync(string code, Guid userId);

        Task<IEnumerable<Coupon>> GetAllAsync();
        Task CreateAsync(Coupon coupon);
        Task<Coupon?> GetByCodeAsync(string code);

        /// <summary>
        /// อัปเดตคูปองที่มีอยู่ และคืนค่าคูปองที่ได้รับการอัปเดต
        /// </summary>
        Task<Coupon> UpdateAsync(Guid id, Coupon updatedCoupon, CancellationToken cancellationToken);
    }
}
