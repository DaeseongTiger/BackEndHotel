using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForMiraiProject.Models;
using ForMiraiProject.ViewModels;
using ForMiraiProject.Utilities;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Repositories.Interfaces;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.Utilities.PagedResult;
using ForMiraiProject.Data;

namespace ForMiraiProject.Services
{
    public class BookingService : I_BookingService
    {
        private readonly I_BookingRepository _bookingRepository;
        private readonly ILogger<BookingService> _logger;

        public required AppDbContext _dbContext;

        public BookingService(I_BookingRepository bookingRepository, ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResultCollect> CreateBookingAsync(BookingRequestViewModel bookingRequest)
        {
            if (bookingRequest == null)
                return OperationResultCollect.Failure("Booking request cannot be null.");

            try
            {
                var isRoomAvailable = await CheckRoomAvailabilityAsync(bookingRequest.RoomId, bookingRequest.StartDate, bookingRequest.EndDate);
                if (!isRoomAvailable)
                    return OperationResultCollect.Failure("Room is not available for the requested dates.");

                var newBooking = new Booking
                {
                    UserId = bookingRequest.UserId,
                    RoomId = bookingRequest.RoomId,
                    StartDate = bookingRequest.StartDate,
                    EndDate = bookingRequest.EndDate,
                    SpecialRequests = bookingRequest.SpecialRequests ?? string.Empty,
                    BookingStatus = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    CustomerName = bookingRequest.CustomerName // กำหนดค่าของ CustomerName
                };

                await _bookingRepository.AddAsync(newBooking);

                return OperationResultCollect.Success("Booking created successfully.", newBooking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating a booking.");
                return OperationResultCollect.Failure("An error occurred while creating the booking.");
            }
        }

        public async Task<OperationResultCollect> CancelBookingAsync(Guid bookingId)
        {
            if (bookingId == Guid.Empty)
                return OperationResultCollect.Failure("Booking ID cannot be empty.");

            try
            {
                var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
                if (booking == null)
                    return OperationResultCollect.Failure("Booking not found.");

                booking.BookingStatus = "Cancelled";
                await _bookingRepository.UpdateBookingAsync(booking);

                return OperationResultCollect.Success("Booking cancelled successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cancelling the booking.");
                return OperationResultCollect.Failure("An error occurred while cancelling the booking.");
            }
        }

        public async Task<Booking?> GetBookingByIdAsync(Guid bookingId)
{
    if (bookingId == Guid.Empty)
    {
        return null;
    }

    var booking = await _dbContext.Bookings.FindAsync(bookingId);
    return booking;
}

        public async Task<PagedResultBase<BookingResponseViewModel>> GetPagedBookingsAsync(int pageIndex, int pageSize)
        {
            if (pageIndex < 0 || pageSize <= 0)
                throw new ArgumentException("Invalid pagination parameters.");

            try
            {
                var bookings = await _bookingRepository.GetPagedBookingsAsync(pageIndex, pageSize);
                var totalBookings = await _bookingRepository.GetTotalBookingCountAsync();

                var bookingViewModels = bookings.Select(booking => new BookingResponseViewModel
                {
                    BookingId = booking.Id,
                    RoomId = booking.RoomId,
                    UserId = booking.UserId,
                    StartDate = booking.StartDate,
                    EndDate = booking.EndDate,
                    SpecialRequests = booking.SpecialRequests,
                    BookingStatus = booking.BookingStatus
                });

                return PagedResultBase<BookingResponseViewModel>.Create(
                    bookingViewModels, totalBookings, pageIndex, pageSize
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paged bookings.");
                throw;
            }
        }

        public async Task<IEnumerable<BookingResponseViewModel>> GetBookingsByUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return Enumerable.Empty<BookingResponseViewModel>();

            try
            {
                var bookings = await _bookingRepository.GetBookingsByUserAsync(userId);

                return bookings.Select(b => new BookingResponseViewModel
                {
                    BookingId = b.Id,
                    RoomId = b.RoomId,
                    UserId = b.UserId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    SpecialRequests = b.SpecialRequests,
                    BookingStatus = b.BookingStatus
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching bookings for user.");
                return Enumerable.Empty<BookingResponseViewModel>();
            }
        }

        public async Task<bool> CheckRoomAvailabilityAsync(Guid roomId, DateTime startDate, DateTime endDate)
        {
            if (roomId == Guid.Empty || startDate >= endDate)
                return false;

            return await _bookingRepository.IsBookingConflictAsync(roomId, startDate, endDate);
        }

        public async Task<OperationResultCollect> UpdateBookingStatusAsync(Guid bookingId, string status)
        {
            if (bookingId == Guid.Empty || string.IsNullOrEmpty(status))
                return OperationResultCollect.Failure("Invalid booking ID or status.");

            try
            {
                var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
                if (booking == null)
                    return OperationResultCollect.Failure("Booking not found.");

                booking.BookingStatus = status ?? "Unknown";
                await _bookingRepository.UpdateBookingAsync(booking);

                return OperationResultCollect.Success("Booking status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating booking status.");
                return OperationResultCollect.Failure("An error occurred while updating booking status.");
            }
        }

        public async Task DeleteBookingAsync(Booking booking)
{
    if (booking == null)
        throw new ArgumentNullException(nameof(booking), "Booking cannot be null");

    try
    {
        // ส่ง booking.Id (Guid) แทนการส่ง booking ทั้งหมด
        await _bookingRepository.DeleteBookingAsync(booking.Id);  
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while deleting the booking.");
        throw;
    }
}

public BookingService(AppDbContext dbContext, I_BookingRepository bookingRepository, ILogger<BookingService> logger)
{
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}



     public async Task<bool> DeleteBookingAsync(Guid bookingId)
{
    if (bookingId == Guid.Empty)
        return false;  // คืนค่าผลลัพธ์เมื่อ bookingId เป็นค่าไม่ถูกต้อง

    try
    {
        var booking = await _dbContext.Bookings.FindAsync(bookingId);
        if (booking == null)
            return false;  // คืนค่าผลลัพธ์เมื่อไม่พบ booking

        _dbContext.Bookings.Remove(booking);
        await _dbContext.SaveChangesAsync();

        return true;  // คืนค่าผลลัพธ์เมื่อการลบสำเร็จ
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while deleting the booking.");
        return false;  // คืนค่าผลลัพธ์เมื่อเกิดข้อผิดพลาด
    }
}


 



    }
}
