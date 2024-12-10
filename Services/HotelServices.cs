using ForMiraiProject.Models;
using ForMiraiProject.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.DTOs;
using ForMiraiProject.Repositories.Interfaces;
using ForMiraiProject.Services.Interfaces;

namespace ForMiraiProject.Services
{
    public interface I_HotelService
    {
        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(Guid id);
        Task<Hotel> CreateHotelAsync(HotelCreateRequest request);
        Task<Hotel?> UpdateHotelAsync(Guid id, HotelUpdateRequest request);
        Task<bool> DeleteHotelAsync(Guid id);
    }

    public class HotelService : I_HotelService
    {
        private readonly I_HotelRepository _hotelRepository;
        private readonly ILogger<HotelService> _logger;

        public HotelService(I_HotelRepository hotelRepository, ILogger<HotelService> logger)
        {
            _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all hotels from the repository.");
                var hotels = await _hotelRepository.GetAllAsync();
                return hotels ?? new List<Hotel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching hotels.");
                throw new InvalidOperationException("Error fetching hotels", ex);
            }
        }

        public async Task<Hotel?> GetHotelByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Fetching hotel by id: {id}");
                return await _hotelRepository.GetHotelWithDetailsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching hotel with id {id}.");
                throw new InvalidOperationException("Error fetching hotel", ex);
            }
        }

        public async Task<Hotel> CreateHotelAsync(HotelCreateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request data cannot be null.");
            }

            try
            {
                _logger.LogInformation("Creating a new hotel.");

                var newHotel = new Hotel
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Address = request.Address,
                    Description = request.Description,
                    City = request.City,
                    Country = request.Country,
                    ZipCode = request.ZipCode,
                    Phone = request.Phone,
                    Email = request.Email,
                    Website = request.Website,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                return await _hotelRepository.AddAsync(newHotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hotel.");
                throw new InvalidOperationException("Error creating hotel", ex);
            }
        }

        public async Task<Hotel?> UpdateHotelAsync(Guid id, HotelUpdateRequest request)
        {
            if (request == null || id != request.Id)
            {
                throw new ArgumentException("Invalid hotel update data.");
            }

            try
            {
                _logger.LogInformation($"Updating hotel with id: {id}");
                var hotelToUpdate = await _hotelRepository.GetHotelWithDetailsAsync(id);
                if (hotelToUpdate == null) return null;

                hotelToUpdate.Name = request.Name ?? hotelToUpdate.Name;
                hotelToUpdate.Address = request.Address ?? hotelToUpdate.Address;
                hotelToUpdate.Description = request.Description ?? hotelToUpdate.Description;
                hotelToUpdate.City = request.City ?? hotelToUpdate.City;
                hotelToUpdate.Country = request.Country ?? hotelToUpdate.Country;
                hotelToUpdate.ZipCode = request.ZipCode ?? hotelToUpdate.ZipCode;
                hotelToUpdate.Phone = request.Phone ?? hotelToUpdate.Phone;
                hotelToUpdate.Email = request.Email ?? hotelToUpdate.Email;
                hotelToUpdate.Website = request.Website ?? hotelToUpdate.Website;
                hotelToUpdate.UpdatedAt = DateTime.UtcNow;

                return await _hotelRepository.UpdateAsync(hotelToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating hotel with id {id}.");
                throw new InvalidOperationException("Error updating hotel", ex);
            }
        }

        public async Task<bool> DeleteHotelAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Deleting hotel with id: {id}");
                var hotelToDelete = await _hotelRepository.GetHotelWithDetailsAsync(id);
                if (hotelToDelete == null) return false;

                await _hotelRepository.DeleteAsync(id);
                _logger.LogInformation($"Hotel with id {id} deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting hotel with id {id}.");
                throw new InvalidOperationException("Error deleting hotel", ex);
            }
        }
    }
}
