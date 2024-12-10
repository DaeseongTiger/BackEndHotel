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
        /// <returns>รายการคูปองที่ยังใช้งานได้</returns>
        Task<IEnumerable<Coupon>> GetActiveCouponsAsync();

        /// <summary>
        /// ดึงคูปองที่หมดอายุ
        /// </summary>
        /// <returns>รายการคูปองที่หมดอายุ</returns>
        Task<IEnumerable<Coupon>> GetExpiredCouponsAsync();

        /// <summary>
        /// ดึงคูปองตามรหัส
        /// </summary>
        /// <param name="code">รหัสคูปอง</param>
        /// <returns>คูปองที่ตรงกับรหัส หรือ null ถ้าไม่พบ</returns>
        Task<Coupon?> GetCouponByCodeAsync(string code);

        /// <summary>
        /// ตรวจสอบว่าคูปองใช้ได้และไม่ซ้ำซ้อน
        /// </summary>
        /// <param name="code">รหัสคูปอง</param>
        /// <param name="userId">รหัสผู้ใช้</param>
        /// <returns>true ถ้าคูปองใช้งานได้, false ถ้าไม่ใช่</returns>
        Task<bool> IsCouponValidAsync(string code, Guid userId);

        /// <summary>
        /// ตรวจสอบข้อจำกัดการใช้งานคูปอง
        /// </summary>
        /// <param name="code">รหัสคูปอง</param>
        /// <param name="userId">รหัสผู้ใช้</param>
        /// <returns>true ถ้าคูปองมีข้อจำกัดกับผู้ใช้นี้, false ถ้าไม่ใช่</returns>
        Task<bool> IsCouponRestrictedAsync(string code, Guid userId);

        /// <summary>
        /// ปิดใช้งานคูปอง
        /// </summary>
        /// <param name="code">รหัสคูปอง</param>
        /// <returns>true ถ้าปิดใช้งานสำเร็จ, false ถ้าไม่สำเร็จ</returns>
        Task<bool> DeactivateCouponAsync(string code);

        /// <summary>
        /// ใช้คูปองและบันทึกสถานะ
        /// </summary>
        /// <param name="code">รหัสคูปอง</param>
        /// <param name="userId">รหัสผู้ใช้</param>
        /// <returns>true ถ้าการใช้คูปองสำเร็จ, false ถ้าไม่สำเร็จ</returns>
        Task<bool> RedeemCouponAsync(string code, Guid userId);
    }
}
