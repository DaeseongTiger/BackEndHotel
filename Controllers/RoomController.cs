#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;
using ForMiraiProject.DTOs;

namespace ForMiraiProject.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly I_RoomRepository _roomRepository;

        public RoomController(I_RoomRepository roomRepository)
        {
            _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetAllRooms()
        {
            var rooms = _roomRepository.GetAllRooms();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public ActionResult<Room> GetRoomById(Guid id)
        {
            var room = _roomRepository.GetRoomById(id);
            if (room == null)
            {
                return NotFound(new { message = $"Room with ID {id} not found." });
            }

            return Ok(room);
        }

        [HttpPost]
        public ActionResult<Room> CreateRoom(CreateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newRoom = new Room(request.RoomNumber, request.RoomType)
            {
                PricePerNight = request.PricePerNight,
                MaxOccupancy = request.MaxOccupancy,
                HotelId = request.HotelId
            };

            _roomRepository.CreateRoom(newRoom);
            return CreatedAtAction(nameof(GetRoomById), new { id = newRoom.Id }, newRoom);
        }

        [HttpPut("{id}")]
        public ActionResult<Room> UpdateRoom(Guid id, UpdateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = _roomRepository.GetRoomById(id);
            if (room == null)
                return NotFound(new { message = $"Room with ID {id} not found." });

            room.UpdateDetails(
                WebUtility.HtmlEncode(request.RoomNumber),
                request.PricePerNight,
                WebUtility.HtmlEncode(request.RoomType),
                request.MaxOccupancy
            );

            _roomRepository.UpdateRoom(room);
            return Ok(room);
        }

        [HttpPatch("{id}/availability")]
        public ActionResult<Room> UpdateRoomAvailability(Guid id, bool isAvailable)
        {
            var room = _roomRepository.GetRoomById(id);
            if (room == null)
                return NotFound(new { message = $"Room with ID {id} not found." });

            if (isAvailable)
                room.MarkAsAvailable();
            else
                room.MarkAsUnavailable();

            _roomRepository.UpdateRoom(room);
            return Ok(room);
        }
    }

    public class Room
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RoomNumber { get; set; }
        public decimal PricePerNight { get; set; }
        public string RoomType { get; set; }
        public int MaxOccupancy { get; set; }
        public Guid HotelId { get; set; }

        public Room(string roomNumber, string roomType)
        {
            RoomNumber = roomNumber ?? throw new ArgumentNullException(nameof(roomNumber));
            RoomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
        }

        public void UpdateDetails(string roomNumber, decimal pricePerNight, string roomType, int maxOccupancy)
        {
            RoomNumber = roomNumber;
            PricePerNight = pricePerNight;
            RoomType = roomType;
            MaxOccupancy = maxOccupancy;
        }

        public void MarkAsAvailable()
        {
            // Implementation for marking as available
        }

        public void MarkAsUnavailable()
        {
            // Implementation for marking as unavailable
        }
    }

    public interface I_RoomRepository
    {
        IEnumerable<Room> GetAllRooms();
        Room? GetRoomById(Guid id); // Marked as nullable
        void CreateRoom(Room room);
        void UpdateRoom(Room room);
    }
}
