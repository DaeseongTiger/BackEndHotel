using System;

namespace ForMiraiProject.DTOs
{
    public class BookingDTO
    {
        public Guid BookingId { get; set; }  // รหัสการจอง
        public Guid UserId { get; set; }  // รหัสผู้จอง
        public DateTime CheckInDate { get; set; }  // วันที่เช็คอิน
        public DateTime CheckOutDate { get; set; }  // วันที่เช็คเอาท์
        public int NumberOfGuests { get; set; }  // จำนวนผู้เข้าพัก
        public Guid RoomId { get; set; }  // รหัสห้องพักที่จอง
        public decimal TotalPrice { get; set; }  // ราคาทั้งหมดของการจอง
        public string SpecialRequests { get; set; }  // คำขอพิเศษ (เช่น เตียงเสริม)
    }
}