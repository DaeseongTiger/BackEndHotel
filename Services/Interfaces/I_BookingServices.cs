using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.ViewModels;
using ForMiraiProject.Utilities;
using ForMiraiProject.Utilities.PagedResult;

namespace ForMiraiProject.Services.Interfaces
{
    public interface I_BookingService
    {
        Task<OperationResultCollect> CreateBookingAsync(BookingRequestViewModel bookingRequest);
        Task<OperationResultCollect> CancelBookingAsync(Guid bookingId);
        Task<BookingResponseViewModel?> GetBookingByIdAsync(Guid bookingId);
        Task<IEnumerable<BookingResponseViewModel>> GetBookingsByUserAsync(Guid userId);
        Task<PagedResultBase<BookingResponseViewModel>> GetPagedBookingsAsync(int pageIndex, int pageSize);

        Task<bool> CheckRoomAvailabilityAsync(Guid roomId, DateTime startDate, DateTime endDate);
        Task<OperationResultCollect> UpdateBookingStatusAsync(Guid bookingId, string status);
        // Method นี้ต้องตรงกับ Service
    }
}
