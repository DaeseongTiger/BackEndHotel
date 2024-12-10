using ForMiraiProject.DTOs; // Import DTO for HotelCreateRequest and HotelUpdateRequest
using ForMiraiProject.Services.Interfaces; // Service Interface
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization; // For [Authorize]
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;

namespace ForMiraiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly I_HotelService _hotelService;
        private readonly ILogger<HotelController> _logger;

        public HotelController(I_HotelService hotelService, ILogger<HotelController> logger)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/hotel
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Hotel>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var hotels = await _hotelService.GetAllHotelsAsync();
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotels");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/hotel/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Hotel), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetHotelById(Guid id)
        {
            try
            {
                var hotel = await _hotelService.GetHotelByIdAsync(id);
                if (hotel == null)
                {
                    return NotFound("Hotel not found.");
                }
                return Ok(hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotel by id");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/hotel
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Hotel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateHotel([FromBody] HotelCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Hotel creation request cannot be null");
            }

            try
            {
                var hotel = await _hotelService.CreateHotelAsync(request);
                return CreatedAtAction(nameof(GetHotelById), new { id = hotel.Id }, hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hotel");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/hotel/{id}
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Hotel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] HotelUpdateRequest request)
        {
            if (request == null || id != request.Id)
            {
                return BadRequest("Invalid data");
            }

            try
            {
                var updatedHotel = await _hotelService.UpdateHotelAsync(id, request);
                if (updatedHotel == null)
                {
                    return NotFound("Hotel not found.");
                }

                return Ok(updatedHotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hotel");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/hotel/{id}
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            try
            {
                var result = await _hotelService.DeleteHotelAsync(id);
                if (!result)
                {
                    return NotFound("Hotel not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting hotel");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
