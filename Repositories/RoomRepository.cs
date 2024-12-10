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
    public class RoomRepository : GenericRepository<Room>, I_RoomRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RoomRepository> _logger;

        public RoomRepository(AppDbContext context, ILogger<RoomRepository> logger) : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ดึงห้องว่างสำหรับการจอง
        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut)
            {
                _logger.LogWarning("Invalid booking dates provided.");
                throw new ArgumentException("Check-In date must be earlier than Check-Out date");
            }

            _logger.LogInformation("Fetching available rooms for dates {CheckIn} to {CheckOut}.", checkIn, checkOut);
            return await _context.Rooms
                .Where(r => r.IsAvailable &&
                            !_context.Bookings.Any(b =>
                                b.RoomId == r.Id &&
                                b.CheckInDate < checkOut &&
                                b.CheckOutDate > checkIn))
                .AsNoTracking()
                .ToListAsync();
        }

        // ดึงห้องพักตามโรงแรม
        public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(Guid hotelId)
        {
            if (hotelId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Hotel ID provided.");
                throw new ArgumentException("Invalid Hotel ID");
            }

            _logger.LogInformation("Fetching rooms for hotel ID {HotelId}.", hotelId);
            return await _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .AsNoTracking()
                .ToListAsync();
        }

        // อัปเดตสถานะห้องว่าง
        public async Task UpdateRoomAvailabilityAsync(Guid roomId, bool isAvailable)
        {
            if (roomId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Room ID provided for availability update.");
                throw new ArgumentException("Invalid Room ID");
            }

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
            {
                _logger.LogWarning("Room with ID {RoomId} not found.", roomId);
                throw new KeyNotFoundException("Room not found");
            }

            room.IsAvailable = isAvailable;
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Room availability updated for Room ID {RoomId}.", roomId);
        }

        // ตรวจสอบราคาห้อง (Validation)
        public  async Task AddAsync(Room entity)
        {
            if (entity.PricePerNight < 0)
            {
                _logger.LogWarning("Attempted to add a room with negative price.");
                throw new ArgumentException("Room price cannot be negative");
            }

            _logger.LogInformation("Adding a new room with details: {Room}.", entity);
            await base.AddAsync(entity);
        }

        public  async Task UpdateAsync(Room entity)
{
    if (entity.PricePerNight < 0)
        throw new ArgumentException("Room price cannot be negative");

    await base.UpdateAsync(entity); // ใช้พฤติกรรมพื้นฐานจาก GenericRepository
}
    }
}
