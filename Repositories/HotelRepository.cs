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
    public class HotelRepository : GenericRepository<Hotel>, I_HotelRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HotelRepository> _logger;

        public HotelRepository(AppDbContext context, ILogger<HotelRepository> logger)
            : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Fetch all active hotels.
        /// </summary>
        /// <returns>A list of active hotels.</returns>
        public async Task<IEnumerable<Hotel>> GetActiveHotelsAsync()
        {
            _logger.LogInformation("Fetching all active hotels.");
            return await _context.Hotels
                .Where(h => h.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Search hotels by keyword (name or address).
        /// </summary>
        /// <param name="keyword">Search keyword.</param>
        /// <returns>A list of hotels matching the search term.</returns>
        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                _logger.LogWarning("Search keyword is null or empty.");
                throw new ArgumentException("Search keyword cannot be null or empty.", nameof(keyword));
            }

            _logger.LogInformation("Searching hotels with keyword: {Keyword}.", keyword);
            return await _context.Hotels
                .Where(h => h.IsActive &&
                            (EF.Functions.Like(h.Name, $"%{keyword}%") ||
                             EF.Functions.Like(h.Address, $"%{keyword}%")))
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Search hotels with pagination.
        /// </summary>
        /// <param name="keyword">Search keyword.</param>
        /// <param name="pageIndex">Page index (zero-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>A paginated list of hotels matching the search term.</returns>
        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(string keyword, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Search keyword cannot be null or empty.", nameof(keyword));
            if (pageIndex < 0)
                throw new ArgumentException("Page index cannot be negative.", nameof(pageIndex));
            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

            _logger.LogInformation("Paginated search for hotels with keyword: {Keyword}, page: {PageIndex}.", keyword, pageIndex);
            return await _context.Hotels
                .Where(h => h.IsActive &&
                            (EF.Functions.Like(h.Name, $"%{keyword}%") ||
                             EF.Functions.Like(h.Address, $"%{keyword}%")))
                .AsNoTracking()
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Get hotel details with related data (rooms and amenities).
        /// </summary>
        /// <param name="id">Hotel ID.</param>
        /// <returns>A hotel with related details or null if not found.</returns>
        public async Task<Hotel?> GetHotelWithDetailsAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Hotel ID cannot be empty.", nameof(id));

            _logger.LogInformation("Fetching details for hotel with ID: {Id}.", id);
            return await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.HotelAmenities)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        /// <summary>
        /// Add a new hotel.
        /// </summary>
        /// <param name="entity">The hotel entity.</param>
        /// <returns>The added hotel.</returns>
        public virtual   async  Task<Hotel> AddAsync(Hotel entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Hotel entity cannot be null.");

            _logger.LogInformation("Adding new hotel: {Name}.", entity.Name);
            await _context.Hotels.AddAsync(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Hotel with ID: {Id} added successfully.", entity.Id);
            return entity;
        }

        /// <summary>
        /// Delete a hotel by ID.
        /// </summary>
        /// <param name="id">Hotel ID.</param>
        /// <returns>True if deletion was successful, otherwise false.</returns>
        public async Task<bool> DeleteHotelAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Hotel ID cannot be empty.", nameof(id));

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                _logger.LogWarning("Hotel with ID: {Id} not found for deletion.", id);
                return false;
            }

            _logger.LogInformation("Deleting hotel with ID: {Id}.", id);
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Hotel with ID: {Id} deleted successfully.", id);
            return true;
        }

          public async Task<Hotel> UpdateAsync(Hotel hotel)
        {
            if (hotel == null)
                throw new ArgumentNullException(nameof(hotel));

            var existingHotel = await _context.Hotels.FindAsync(hotel.Id);
            if (existingHotel == null)
                throw new InvalidOperationException($"Hotel with ID {hotel.Id} does not exist.");

            // Update fields
            existingHotel.Name = hotel.Name;
            existingHotel.Description = hotel.Description;
            existingHotel.Address = hotel.Address;
            existingHotel.City = hotel.City;
            existingHotel.Country = hotel.Country;
            existingHotel.ZipCode = hotel.ZipCode;
            existingHotel.Phone = hotel.Phone;
            existingHotel.Email = hotel.Email;
            existingHotel.Website = hotel.Website;
            existingHotel.IsActive = hotel.IsActive;
            existingHotel.UpdatedAt = DateTime.UtcNow;

            _context.Hotels.Update(existingHotel);
            await _context.SaveChangesAsync();

            return existingHotel;
        }
        public async Task<Hotel?> GetHotelByIdAsync(Guid id)
{
    if (id == Guid.Empty)
    {
        _logger.LogWarning("Invalid hotel ID provided.");
        throw new ArgumentException("Hotel ID cannot be empty.", nameof(id));
    }

    try
    {
        _logger.LogInformation("Fetching hotel details for ID {Id}.", id);
        return await _context.Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching hotel with ID {Id}.", id);
        throw;
    }
}

    }
}
