using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;

namespace ForMiraiProject.Repositories.Interfaces
{
    public interface I_ReviewRepository : I_GenericRepository<Review>
{
    Task<IEnumerable<Review>> GetReviewsByHotelIdAsync(Guid hotelId);    // ดึงรีวิวตามโรงแรม
    Task<Review?> GetReviewByUserAsync(Guid userId, Guid hotelId);        // ดึงรีวิวของผู้ใช้ที่โรงแรม
    Task<bool> IsReviewExistsAsync(Guid userId, Guid hotelId);           // ตรวจสอบว่าผู้ใช้รีวิวโรงแรมนี้แล้ว
}
}
