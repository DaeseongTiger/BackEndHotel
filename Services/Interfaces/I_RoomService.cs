using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;


namespace ForMiraiProject.Services.Interfaces
{
    /// <summary>
    /// Interface สำหรับการจัดการห้องพัก
    /// </summary>
    public interface I_RoomService
    {
        /// <summary>
        /// ดึงห้องที่ว่างสำหรับการจองในช่วงวันที่กำหนด
        /// </summary>
        /// <param name="checkIn">วันที่เช็คอิน</param>
        /// <param name="checkOut">วันที่เช็คเอาท์</param>
        /// <returns>รายการห้องว่าง</returns>
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);

        /// <summary>
        /// ดึงห้องพักทั้งหมดในโรงแรมที่กำหนด
        /// </summary>
        /// <param name="hotelId">รหัสโรงแรม</param>
        /// <returns>รายการห้องพักในโรงแรม</returns>
        Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(Guid hotelId);

        /// <summary>
        /// อัปเดตสถานะห้องพัก (ว่าง / ไม่ว่าง)
        /// </summary>
        /// <param name="roomId">รหัสห้องพัก</param>
        /// <param name="isAvailable">สถานะห้อง (true: ว่าง, false: ไม่ว่าง)</param>
        /// <returns>Task ที่บ่งชี้ผลการอัปเดต</returns>
        Task UpdateRoomAvailabilityAsync(Guid roomId, bool isAvailable);

        /// <summary>
        /// เพิ่มห้องพักใหม่
        /// </summary>
        /// <param name="room">ข้อมูลห้องพักใหม่</param>
        /// <returns>ผลการเพิ่มห้องพัก</returns>
        Task AddRoomAsync(Room room);

        /// <summary>
        /// อัปเดตข้อมูลห้องพักที่มีอยู่
        /// </summary>
        /// <param name="room">ข้อมูลห้องพักที่อัปเดต</param>
        /// <returns>ผลการอัปเดตห้องพัก</returns>
        Task UpdateRoomAsync(Room room);
    }
}
