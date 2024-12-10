using ForMiraiProject.DTOs;
using ForMiraiProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForMiraiProject.Services.Interfaces
{
    public interface I_HotelService
    {
        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(Guid id);
        Task<Hotel> CreateHotelAsync(HotelCreateRequest request);
        Task<Hotel?> UpdateHotelAsync(Guid id, HotelUpdateRequest request);
        Task<bool> DeleteHotelAsync(Guid id);
        Task<IEnumerable<Hotel>> SearchHotelsAsync(string searchTerm);
    }
}