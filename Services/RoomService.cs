using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;
using ForMiraiProject.Services.Interfaces;

namespace ForMiraiProject.Services
{
    public class RoomService : I_RoomService
    {
        private readonly I_RoomRepository _roomRepository;
        private readonly ILogger<RoomService> _logger;

        public RoomService(I_RoomRepository roomRepository, ILogger<RoomService> logger)
        {
            _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// ดึงห้องที่ว่างสำหรับการจอง
        /// </summary>
        /// <param name="checkIn">วันที่เช็คอิน</param>
        /// <param name="checkOut">วันที่เช็คเอาท์</param>
        /// <returns>ห้องว่างสำหรับการจอง</returns>
        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            try
            {
                if (checkIn >= checkOut)
                {
                    _logger.LogWarning("Invalid booking dates provided: CheckIn: {CheckIn}, CheckOut: {CheckOut}", checkIn, checkOut);
                    throw new ArgumentException("Check-In date must be earlier than Check-Out date");
                }

                _logger.LogInformation("Fetching available rooms for dates {CheckIn} to {CheckOut}.", checkIn, checkOut);
                var availableRooms = await _roomRepository.GetAvailableRoomsAsync(checkIn, checkOut);
                return availableRooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching available rooms.");
                throw new ApplicationException("An error occurred while processing the request. Please try again later.");
            }
        }

        /// <summary>
        /// ดึงห้องพักในโรงแรมที่กำหนด
        /// </summary>
        /// <param name="hotelId">รหัสโรงแรม</param>
        /// <returns>รายการห้องพักในโรงแรม</returns>
        public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(Guid hotelId)
        {
            try
            {
                if (hotelId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid Hotel ID provided: {HotelId}", hotelId);
                    throw new ArgumentException("Invalid Hotel ID");
                }

                _logger.LogInformation("Fetching rooms for hotel ID {HotelId}.", hotelId);
                var rooms = await _roomRepository.GetRoomsByHotelIdAsync(hotelId);
                return rooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching rooms for hotel ID {HotelId}.", hotelId);
                throw new ApplicationException("An error occurred while fetching the rooms. Please try again later.");
            }
        }

        /// <summary>
        /// อัปเดตสถานะห้องพัก
        /// </summary>
        /// <param name="roomId">รหัสห้องพัก</param>
        /// <param name="isAvailable">สถานะห้อง (ว่าง / ไม่ว่าง)</param>
        /// <returns>ผลการอัปเดตสถานะห้อง</returns>
        public async Task UpdateRoomAvailabilityAsync(Guid roomId, bool isAvailable)
        {
            try
            {
                if (roomId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid Room ID provided for availability update: {RoomId}", roomId);
                    throw new ArgumentException("Invalid Room ID");
                }

                _logger.LogInformation("Updating availability for room ID {RoomId} to {IsAvailable}.", roomId, isAvailable);
                await _roomRepository.UpdateRoomAvailabilityAsync(roomId, isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating room availability for room ID {RoomId}.", roomId);
                throw new ApplicationException("An error occurred while updating room availability. Please try again later.");
            }
        }

        /// <summary>
        /// เพิ่มห้องพักใหม่
        /// </summary>
        /// <param name="room">ข้อมูลห้องพักใหม่</param>
        /// <returns>ผลการเพิ่มห้องพัก</returns>
        public async Task AddRoomAsync(Room room)
        {
            try
            {
                if (room == null)
                {
                    _logger.LogWarning("Attempted to add a null room.");
                    throw new ArgumentNullException(nameof(room), "Room data cannot be null.");
                }

                if (room.PricePerNight < 0)
                {
                    _logger.LogWarning("Attempted to add a room with negative price: {Price}.", room.PricePerNight);
                    throw new ArgumentException("Room price cannot be negative.");
                }

                _logger.LogInformation("Adding a new room: {Room}.", room);
                await _roomRepository.AddAsync(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new room.");
                throw new ApplicationException("An error occurred while adding the room. Please try again later.");
            }
        }

        /// <summary>
        /// อัปเดตข้อมูลห้องพัก
        /// </summary>
        /// <param name="room">ข้อมูลห้องพักที่อัปเดต</param>
        /// <returns>ผลการอัปเดตห้องพัก</returns>
        public async Task UpdateRoomAsync(Room room)
        {
            try
            {
                if (room == null)
                {
                    _logger.LogWarning("Attempted to update a null room.");
                    throw new ArgumentNullException(nameof(room), "Room data cannot be null.");
                }

                if (room.PricePerNight < 0)
                {
                    _logger.LogWarning("Attempted to update room with negative price: {Price}.", room.PricePerNight);
                    throw new ArgumentException("Room price cannot be negative.");
                }

                _logger.LogInformation("Updating room with ID {RoomId}: {Room}.", room.Id, room);
                await _roomRepository.UpdateAsync(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating room with ID {RoomId}.", room.Id);
                throw new ApplicationException("An error occurred while updating the room. Please try again later.");
            }
        }
    }
}
