using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;

namespace ForMiraiProject.Repositories.Interfaces
{
    /// <summary>
    /// Interface สำหรับการจัดการข้อมูลโรงแรมในฐานข้อมูล
    /// </summary>
    public interface I_HotelRepository : I_GenericRepository<Hotel>
    {
        /// <summary>
        /// ดึงรายชื่อโรงแรมที่ยังเปิดใช้งานอยู่
        /// </summary>
        /// <returns>รายการโรงแรมที่เปิดใช้งาน</returns>
        Task<IEnumerable<Hotel>> GetActiveHotelsAsync();

        /// <summary>
        /// ค้นหาโรงแรมตามคำค้นหา (ชื่อหรือคำอธิบาย) พร้อมการแบ่งหน้า
        /// </summary>
        /// <param name="keyword">คำค้นหา</param>
        /// <param name="pageIndex">หน้าที่ต้องการ (เริ่มจาก 0)</param>
        /// <param name="pageSize">จำนวนข้อมูลต่อหน้า</param>
        /// <returns>รายการโรงแรมที่ตรงกับคำค้นหา</returns>
        Task<IEnumerable<Hotel>> SearchHotelsAsync(string keyword, int pageIndex, int pageSize);

        /// <summary>
        /// ดึงข้อมูลโรงแรมพร้อมความสัมพันธ์ (ห้องพัก สิ่งอำนวยความสะดวก)
        /// </summary>
        /// <param name="id">รหัสโรงแรม (Guid)</param>
        /// <returns>โรงแรมพร้อมข้อมูลความสัมพันธ์ หากไม่พบจะส่งคืน null</returns>
        Task<Hotel?> GetHotelWithDetailsAsync(Guid id);

        /// <summary>
        /// ลบข้อมูลโรงแรม
        /// </summary>
        /// <param name="id">รหัสโรงแรม (Guid)</param>
        /// <returns>ผลลัพธ์ว่าลบสำเร็จหรือไม่</returns>
        Task<bool> DeleteHotelAsync(Guid id);

        /// <summary>
        /// เพิ่มข้อมูลโรงแรมใหม่
        /// </summary>
        /// <param name="entity">ข้อมูลโรงแรมที่ต้องการเพิ่ม</param>
        /// <returns>โรงแรมที่เพิ่มสำเร็จ</returns>
         Task<Hotel> AddAsync(Hotel entity);

        /// <summary>
        /// อัปเดตข้อมูลโรงแรม
        /// </summary>
        /// <param name="hotel">ข้อมูลโรงแรมที่แก้ไข</param>
        /// <returns>โรงแรมที่แก้ไขสำเร็จ</returns>
         Task<Hotel> UpdateAsync(Hotel hotel);

        Task<Hotel?> GetHotelByIdAsync(Guid id);

        // เพิ่มเมธอด GetAllAsync ใน interface
        Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken = default);

    }
}
