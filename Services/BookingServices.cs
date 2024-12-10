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

namespace ForMiraiProject.Services
{
    public class BookingService : I_BookingService
    {
        private readonly I_BookingRepository _bookingRepository;
        private readonly ILogger<BookingService> _logger;

        

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
                // Validate room availability
                var isRoomAvailable = await CheckRoomAvailabilityAsync(bookingRequest.RoomId, bookingRequest.StartDate, bookingRequest.EndDate);
                if (!isRoomAvailable)
                    return OperationResultCollect.Failure("Room is not available for the requested dates.");

                // Create new booking
                var newBooking = new Booking
                {
                    UserId = bookingRequest.UserId,
                    RoomId = bookingRequest.RoomId,
                    StartDate = bookingRequest.StartDate,
                    EndDate = bookingRequest.EndDate,
                    SpecialRequests = bookingRequest.SpecialRequests ?? string.Empty,
                    BookingStatus = "Pending",
                    CreatedAt = DateTime.UtcNow
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

        public async Task<BookingResponseViewModel?> GetBookingByIdAsync(Guid bookingId)
        {
            if (bookingId == Guid.Empty)
                return null;

            try
            {
                var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
                if (booking == null)
                    return null;

                return new BookingResponseViewModel
                {
                    BookingId = booking.Id,
                    RoomId = booking.RoomId,
                    UserId = booking.UserId,
                    StartDate = booking.StartDate,
                    EndDate = booking.EndDate,
                    SpecialRequests = booking.SpecialRequests,
                    BookingStatus = booking.BookingStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving booking details.");
                return null;
            }
        }

        public async Task<PagedResultBase<BookingResponseViewModel>> GetPagedBookingsAsync(int pageIndex, int pageSize)
{
    if (pageIndex < 0 || pageSize <= 0)
        throw new ArgumentException("Invalid pagination parameters.");

    try
    {
        // ดึงข้อมูล Booking
        var bookings = await _bookingRepository.GetPagedBookingsAsync(pageIndex, pageSize);
        var totalBookings = await _bookingRepository.GetTotalBookingCountAsync();

        // แปลง Booking เป็น BookingResponseViewModel
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
        throw; // ให้โยนข้อผิดพลาดออกไปเพื่อจัดการในระดับสูงขึ้น
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
    }
}
