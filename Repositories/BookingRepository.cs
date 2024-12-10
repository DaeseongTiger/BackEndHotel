using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Data;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;


namespace ForMiraiProject.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, I_BookingRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(AppDbContext context, ILogger<BookingRepository> logger)
            : base(context, logger) // Pass the logger to the base constructor
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Fetch bookings by user ID.
        /// </summary>
        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Attempt to query bookings with invalid User ID.");
                throw new ArgumentException("Invalid User ID");
            }

            try
            {
                var bookings = await _context.Bookings
                    .Where(b => b.UserId == userId)
                    .Include(b => b.Room)
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} bookings for User ID {UserId}.", bookings.Count, userId);
                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching bookings for User ID {UserId}.", userId);
                throw;
            }
        }

        /// <summary>
        /// Fetch bookings by room ID.
        /// </summary>
        public async Task<IEnumerable<Booking>> GetBookingsByRoomIdAsync(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                _logger.LogWarning("Attempt to query bookings with invalid Room ID.");
                throw new ArgumentException("Invalid Room ID");
            }

            try
            {
                return await _context.Bookings
                    .Where(b => b.RoomId == roomId)
                    .Include(b => b.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching bookings for Room ID {RoomId}.", roomId);
                throw;
            }
        }

        /// <summary>
        /// Check if a booking conflicts with existing bookings.
        /// </summary>
        public async Task<bool> IsBookingConflictAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            if (roomId == Guid.Empty)
            {
                _logger.LogWarning("Attempt to check booking conflict with invalid Room ID.");
                throw new ArgumentException("Invalid Room ID");
            }

            if (checkIn >= checkOut)
            {
                _logger.LogWarning("Check-In date is later than or equal to Check-Out date.");
                throw new ArgumentException("Check-In date must be earlier than Check-Out date");
            }

            try
            {
                var conflictExists = await _context.Bookings
                    .AnyAsync(b => b.RoomId == roomId &&
                                   b.CheckInDate < checkOut &&
                                   b.CheckOutDate > checkIn);

                _logger.LogInformation("Booking conflict check for Room ID {RoomId}: {Conflict}.", roomId, conflictExists);
                return conflictExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking booking conflict for Room ID {RoomId}.", roomId);
                throw;
            }
        }

        /// <summary>
        /// Add a new booking with validation.
        /// </summary>
        public  async Task AddAsync(Booking entity)
        {
            if (entity.CheckInDate >= entity.CheckOutDate)
            {
                _logger.LogWarning("Invalid booking dates provided.");
                throw new ArgumentException("Check-Out date must be later than Check-In date");
            }

            if (await IsBookingConflictAsync(entity.RoomId, entity.CheckInDate, entity.CheckOutDate))
            {
                _logger.LogWarning("Booking conflict detected for Room ID {RoomId} during {CheckIn} - {CheckOut}.", entity.RoomId, entity.CheckInDate, entity.CheckOutDate);
                throw new InvalidOperationException("Booking conflict: Room is already booked during this period");
            }

            try
            {
                await base.AddAsync(entity);
                _logger.LogInformation("Booking successfully added for User ID {UserId} and Room ID {RoomId}.", entity.UserId, entity.RoomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a booking for Room ID {RoomId}.", entity.RoomId);
                throw;
            }
        }

        /// <summary>
        /// Verify if a booking is owned by a specific user.
        /// </summary>
        public async Task<bool> IsBookingOwnedByUserAsync(Guid bookingId, Guid userId)
        {
            if (bookingId == Guid.Empty || userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Booking ID or User ID provided.");
                throw new ArgumentException("Invalid Booking ID or User ID");
            }

            try
            {
                var isOwned = await _context.Bookings
                    .AnyAsync(b => b.Id == bookingId && b.UserId == userId);

                _logger.LogInformation("Ownership check for Booking ID {BookingId} and User ID {UserId}: {IsOwned}.", bookingId, userId, isOwned);
                return isOwned;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying ownership for Booking ID {BookingId}.", bookingId);
                throw;
            }
        }

        /// <summary>
        /// Get the total number of bookings.
        /// </summary>
        /// 
        
        public async Task<int> GetTotalBookingCountAsync()
        {
            try
            {
                var totalCount = await _context.Bookings.CountAsync();
                _logger.LogInformation("Total booking count retrieved: {TotalCount}.", totalCount);
                return totalCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving total booking count.");
                throw;
            }
        }
        public async Task<IEnumerable<Booking>> GetPagedBookingsAsync(int pageIndex, int pageSize)
{
    if (pageIndex < 0 || pageSize <= 0)
    {
        _logger.LogWarning("Invalid paging parameters: PageIndex = {PageIndex}, PageSize = {PageSize}.", pageIndex, pageSize);
        throw new ArgumentException("Invalid paging parameters.");
    }

    try
    {
        var bookings = await _context.Bookings
            .OrderBy(b => b.StartDate) // จัดเรียงตามวันที่เริ่ม
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        _logger.LogInformation("Paged bookings retrieved successfully: PageIndex = {PageIndex}, PageSize = {PageSize}.", pageIndex, pageSize);
        return bookings;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while fetching paged bookings.");
        throw;
    }
}

   public async Task<Booking?> GetBookingByIdAsync(Guid bookingId)
{
    if (bookingId == Guid.Empty)
    {
        _logger.LogWarning("Invalid booking ID: {BookingId}", bookingId);
        throw new ArgumentException("Booking ID cannot be empty.");
    }

    try
    {
        var booking = await _context.Bookings
            .Include(b => b.Room) // Include related entities if needed
            .AsNoTracking() // Avoid tracking for read-only operations
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            _logger.LogWarning("Booking not found for ID: {BookingId}", bookingId);
        }
        else
        {
            _logger.LogInformation("Booking retrieved successfully for ID: {BookingId}", bookingId);
        }

        return booking;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while retrieving booking by ID: {BookingId}", bookingId);
        throw;
    }
}
public async Task<Booking?> UpdateBookingAsync(Booking booking)
{
    if (booking == null)
    {
        _logger.LogWarning("Attempted to update a null booking.");
        throw new ArgumentNullException(nameof(booking));
    }

    try
    {
        var existingBooking = await _context.Bookings.FindAsync(booking.Id);
        if (existingBooking == null)
        {
            _logger.LogWarning("Booking not found for update: {BookingId}", booking.Id);
            return null;
        }

        // Update properties
        existingBooking.RoomId = booking.RoomId;
        existingBooking.UserId = booking.UserId;
        existingBooking.StartDate = booking.StartDate;
        existingBooking.EndDate = booking.EndDate;
        existingBooking.SpecialRequests = booking.SpecialRequests;
        existingBooking.BookingStatus = booking.BookingStatus;

        _context.Bookings.Update(existingBooking);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Booking updated successfully: {BookingId}", booking.Id);
        return existingBooking;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while updating booking: {BookingId}", booking.Id);
        throw;
    }
}
public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(Guid userId)
{
    if (userId == Guid.Empty)
    {
        _logger.LogWarning("Invalid user ID provided for fetching bookings.");
        throw new ArgumentException("User ID cannot be empty.", nameof(userId));
    }

    try
    {
        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Room) // รวมข้อมูลห้องถ้าจำเป็น
            .AsNoTracking()
            .ToListAsync();

        _logger.LogInformation("Fetched {Count} bookings for User ID {UserId}.", bookings.Count, userId);
        return bookings;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching bookings for User ID {UserId}.", userId);
        throw;
    }
}


    }
}
