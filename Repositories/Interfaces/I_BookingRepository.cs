using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;

namespace ForMiraiProject.Repositories.Interfaces
{
    public interface I_BookingRepository : I_GenericRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId); 
        Task<int> GetTotalBookingCountAsync();
        Task<IEnumerable<Booking>> GetPagedBookingsAsync(int pageIndex, int pageSize); // เพิ่มฟังก์ชันนี้
        Task<IEnumerable<Booking>> GetBookingsByRoomIdAsync(Guid roomId);
        Task<bool> IsBookingConflictAsync(Guid roomId, DateTime checkIn, DateTime checkOut);

        Task<Booking?> GetBookingByIdAsync(Guid bookingId);

        Task<Booking?> UpdateBookingAsync(Booking booking);

        Task<IEnumerable<Booking>> GetBookingsByUserAsync(Guid userId);


        
    }
}
