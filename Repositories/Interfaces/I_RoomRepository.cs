using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;

namespace ForMiraiProject.Repositories.Interfaces
{
    /// <summary>
    /// Interface สำหรับจัดการข้อมูลของ Room
    /// </summary>
    public interface I_RoomRepository : I_GenericRepository<Room>
    {
        /// <summary>
        /// ดึงห้องว่างสำหรับการจองในช่วงวันที่กำหนด
        /// </summary>
        /// <param name="checkIn">วันที่เช็คอิน</param>
        /// <param name="checkOut">วันที่เช็คเอาท์</param>
        /// <returns>รายการห้องว่าง</returns>
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);

        /// <summary>
        /// ดึงข้อมูลห้องพักทั้งหมดในโรงแรมที่กำหนด
        /// </summary>
        /// <param name="hotelId">รหัสโรงแรม</param>
        /// <returns>รายการห้องพักในโรงแรม</returns>
        Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(Guid hotelId);

        /// <summary>
        /// อัปเดตสถานะความว่างของห้องพัก
        /// </summary>
        /// <param name="roomId">รหัสห้องพัก</param>
        /// <param name="isAvailable">สถานะความว่าง (true: ว่าง, false: ไม่ว่าง)</param>
        /// <returns>Task</returns>
        Task UpdateRoomAvailabilityAsync(Guid roomId, bool isAvailable);
    }
}
